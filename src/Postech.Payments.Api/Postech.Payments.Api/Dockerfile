FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Postech.Payments.Api/Postech.Payments.Api.csproj", "Postech.Payments.Api/"]
RUN dotnet restore "Postech.Payments.Api/Postech.Payments.Api.csproj"
COPY . .
WORKDIR "/src/Postech.Payments.Api"
RUN dotnet build "./Postech.Payments.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build --no-restore

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Postech.Payments.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish \
    /p:UseAppHost=false /p:PublishTrimmed=true /p:TrimMode=partial

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Postech.Payments.Api.dll"]
