FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine

WORKDIR /app

COPY publish/Rentals/ .

EXPOSE 8060
ENTRYPOINT ["dotnet", "Rentals.dll"]
