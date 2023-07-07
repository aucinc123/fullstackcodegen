FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 9999

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["DapperCommonScenarios.API/DapperCommonScenarios.API.csproj", "DapperCommonScenarios.API/"]
RUN dotnet restore "DapperCommonScenarios.API/DapperCommonScenarios.API.csproj"
COPY . .
WORKDIR "/src/DapperCommonScenarios.API"
RUN dotnet build "DapperCommonScenarios.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "DapperCommonScenarios.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DapperCommonScenarios.API.dll"]