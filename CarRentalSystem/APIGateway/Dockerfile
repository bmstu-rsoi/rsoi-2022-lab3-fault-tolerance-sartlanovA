FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine

WORKDIR /app

COPY publish/APIGateway/ .

EXPOSE 8080
ENTRYPOINT ["dotnet", "APIGateway.dll"]