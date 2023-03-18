#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal-arm64v8 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal-arm64v8 AS build
WORKDIR /src
COPY ["Presentation/Amazon Price Tracker/Amazon Price Tracker.csproj", "Presentation/Amazon Price Tracker/"]
COPY ["core/AmazonPriceTrackerAPI.Application/AmazonPriceTrackerAPI.Application.csproj", "core/AmazonPriceTrackerAPI.Application/"]
COPY ["core/AmazonPriceTrackerAPI.Domain/AmazonPriceTrackerAPI.Domain.csproj", "core/AmazonPriceTrackerAPI.Domain/"]
COPY ["Infrastructure/AmazonPriceTrackerAPI.Persistence/AmazonPriceTrackerAPI.Persistence.csproj", "Infrastructure/AmazonPriceTrackerAPI.Persistence/"]
COPY ["Infrastructure/AmazonPriceTrackerAPI.Infrastructure/AmazonPriceTrackerAPI.Infrastructure.csproj", "Infrastructure/AmazonPriceTrackerAPI.Infrastructure/"]
RUN dotnet restore "Presentation/Amazon Price Tracker/Amazon Price Tracker.csproj"
COPY . .
WORKDIR "/src/Presentation/Amazon Price Tracker"
RUN dotnet build "Amazon Price Tracker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Amazon Price Tracker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Amazon Price Tracker.dll"]