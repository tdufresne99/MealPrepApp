FROM mcr.microsoft.com/dotnet/sdk:10.0.400 AS build
WORKDIR /source
COPY global.json MealPrepApp.slnx ./
COPY src/ src/
RUN dotnet publish src/MealPrep.Api/MealPrep.Api.csproj --configuration Release --output /app /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app ./
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "MealPrep.Api.dll"]
