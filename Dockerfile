# Use the official .NET 9.0 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Use the SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["LandValueTaxCalculator/LandValueTaxCalculator.csproj", "LandValueTaxCalculator/"]
RUN dotnet restore "LandValueTaxCalculator/LandValueTaxCalculator.csproj"
COPY . .
WORKDIR "/src/LandValueTaxCalculator"
RUN dotnet build "LandValueTaxCalculator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LandValueTaxCalculator.csproj" -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LandValueTaxCalculator.dll"]