$version = '1.0.0'

if ($env:APPVEYOR_BUILD_VERSION -ne $null)
{
	$version = $env:APPVEYOR_BUILD_VERSION
}

dotnet build -c Release /p:packageversion=$version
dotnet test
dotnet pack -c Release -o artifacts --no-build /p:packageversion=$version
