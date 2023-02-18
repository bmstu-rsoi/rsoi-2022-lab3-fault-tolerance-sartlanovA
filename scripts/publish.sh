dotnet restore CarRentalSystem/CarRentalSystem.sln

dotnet publish CarRentalSystem/APIGateway/APIGateway.csproj -c Release -o publish/APIGateway --no-restore
dotnet publish CarRentalSystem/Cars/Cars.csproj -c Release -o publish/Cars --no-restore
dotnet publish CarRentalSystem/Payments/Payments.csproj -c Release -o publish/Payments --no-restore
dotnet publish CarRentalSystem/Rentals/Rentals.csproj -c Release -o publish/Rentals --no-restore
