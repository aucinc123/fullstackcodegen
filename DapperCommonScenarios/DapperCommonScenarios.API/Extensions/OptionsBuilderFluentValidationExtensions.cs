using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Politely borrowed from: https://andrewlock.net/adding-validation-to-strongly-typed-configuration-objects-in-dotnet-6/
namespace DapperCommonScenarios.API.Extensions
{
    public static class FluentValidationOptionsExtensions
    {
        public static OptionsBuilder<TOptions> AddWithValidationForSection<TOptions, TValidator>(
            this IServiceCollection services,
            string configurationSection)
        where TOptions : class
        where TValidator : class, IValidator<TOptions>
        {
            // Add the validator
            services.AddScoped<IValidator<TOptions>, TValidator>();

            return services.AddOptions<TOptions>()
                .BindConfiguration(configurationSection)
                .ValidateFluentValidation()
                .ValidateOnStart();
        }

        public static OptionsBuilder<TOptions> AddWithValidation<TOptions, TValidator>(
            this IServiceCollection services)
        where TOptions : class
        where TValidator : class, IValidator<TOptions>
        {
            // Add the validator
            services.AddScoped<IValidator<TOptions>, TValidator>();

            return services.AddOptions<TOptions>()
                .ValidateFluentValidation()
                .ValidateOnStart();
        }
    }

    public static class OptionsBuilderFluentValidationExtensions
    {
        public static OptionsBuilder<TOptions> ValidateFluentValidation<TOptions>(
          this OptionsBuilder<TOptions> optionsBuilder) where TOptions : class
        {
            optionsBuilder.Services.AddSingleton<IValidateOptions<TOptions>>(
                provider => new FluentValidationOptions<TOptions>(
                  optionsBuilder.Name, provider));
            return optionsBuilder;
        }
    }

    public class FluentValidationOptions<TOptions>
    : IValidateOptions<TOptions> where TOptions : class
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _name;
        public FluentValidationOptions(string name, IServiceProvider serviceProvider)
        {
            // we need the service provider to create a scope later
            _serviceProvider = serviceProvider;
            _name = name; // Handle named options
        }

        public ValidateOptionsResult Validate(string name, TOptions options)
        {
            // Null name is used to configure all named options.
            if (_name != null && _name != name)
            {
                // Ignored if not validating this instance.
                return ValidateOptionsResult.Skip;
            }

            // Ensure options are provided to validate against
            ArgumentNullException.ThrowIfNull(options);

            // Validators are typically registered as scoped,
            // so we need to create a scope to be safe, as this
            // method is be called from the root scope
            using IServiceScope scope = _serviceProvider.CreateScope();

            // retrieve an instance of the validator
            var validator = scope.ServiceProvider.GetRequiredService<IValidator<TOptions>>();

            // Run the validation
            ValidationResult results = validator.Validate(options);
            if (results.IsValid)
            {
                // All good!
                return ValidateOptionsResult.Success;
            }

            // Validation failed, so build the error message
            string typeName = options.GetType().Name;
            var errors = new List<string>();
            foreach (var result in results.Errors)
            {
                errors.Add($"Fluent validation failed for '{typeName}.{result.PropertyName}' with the error: '{result.ErrorMessage}'.");
            }

            return ValidateOptionsResult.Fail(errors);
        }
    }
}
