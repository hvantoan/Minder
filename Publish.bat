set outputPath=api-publish

if exist %outputPath% (
	rmdir /S /Q %outputPath%
)

dotnet clean Minder.Api.sln
dotnet restore
dotnet publish Minder.Api\Minder.Api.csproj --output %outputPath% -c Release
