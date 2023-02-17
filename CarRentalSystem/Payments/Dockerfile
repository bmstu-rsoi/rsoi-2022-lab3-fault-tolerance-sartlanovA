FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine

WORKDIR /app

COPY publish/Payments/ .

EXPOSE 8050
ENTRYPOINT ["dotnet", "Payments.dll"]