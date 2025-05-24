FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/out ./
COPY entrypoint.sh ./
RUN chmod +x ./entrypoint.sh

EXPOSE 5000
ENTRYPOINT ["./entrypoint.sh"]
CMD ["dotnet", "PersonalPortfolioTracker.dll"]