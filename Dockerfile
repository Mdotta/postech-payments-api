
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS base
WORKDIR /repo

FROM base AS build
COPY src ./src
COPY Postech.Shared.dll ./Postech.Shared.dll
WORKDIR /repo/src/Postech.Payments.Api
RUN dotnet restore Postech.Payments.Api.csproj
RUN dotnet build Postech.Payments.Api.csproj -c Release -o /app/build

FROM build AS test
WORKDIR /repo/src/Postech.Payments.Api.Tests
RUN dotnet test Postech.Payments.Api.Tests.csproj -c Release --verbosity normal

FROM build AS publish
WORKDIR /repo/src/Postech.Payments.Api
RUN dotnet publish Postech.Payments.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "Postech.Payments.Api.dll"]
