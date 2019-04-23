
#& coverlet ./Fare.IntegrationTests/bin/Debug/netcoreapp2.1/Fare.dll  --target "dotnet" --targetargs "test ./Fare.IntegrationTests/Fare.IntegrationTests.csproj --no-build"

& dotnet test /p:CollectCoverage=true /p:Include="[Fare*]*" /p:Exclude="[Fare.Integration*]*"  ./Fare.IntegrationTests