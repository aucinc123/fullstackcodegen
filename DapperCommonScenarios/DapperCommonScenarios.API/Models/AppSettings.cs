using FluentValidation;

namespace DapperCommonScenarios.API.Models
{
    public class AppSettings
    {
        public string DBConnection { get; set; }

        public string CorsOrigins { get; set; }
        public string[] CorsOriginList
        {
            get
            {
                if (!string.IsNullOrEmpty(CorsOrigins))
                    return CorsOrigins.Split(",");

                return new string[0];
            }
        }
    }

    public class AppSettingsValidator : AbstractValidator<AppSettings>
    {
        public AppSettingsValidator()
        {
            RuleFor(x => x.DBConnection)
                .NotEmpty();
        }
    }
}
