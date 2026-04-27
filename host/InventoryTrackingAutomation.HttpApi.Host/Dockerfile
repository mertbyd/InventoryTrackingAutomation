# syntax=docker/dockerfile:1.7

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG GITHUB_ACTOR
WORKDIR /src
COPY . .
WORKDIR /src/host/InventoryTrackingAutomation.HttpApi.Host
RUN --mount=type=secret,id=github_token \
    dotnet nuget remove source SystemStandardsLocal --configfile /src/NuGet.Config || true && \
    dotnet nuget remove source github --configfile /src/NuGet.Config || true && \
    dotnet nuget add source "https://nuget.pkg.github.com/mertbyd/index.json" \
      --name github \
      --username "${GITHUB_ACTOR}" \
      --password "$(cat /run/secrets/github_token)" \
      --store-password-in-clear-text \
      --configfile /src/NuGet.Config && \
    dotnet restore
RUN dotnet build --no-restore -c Release -o /app

FROM build AS publish
RUN dotnet publish --no-restore -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "InventoryTrackingAutomation.HttpApi.Host.dll"]
