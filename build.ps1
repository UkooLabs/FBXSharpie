$version = '1.0.0'

if ($env:APPVEYOR_BUILD_VERSION -ne $null)
{
	$version = $env:APPVEYOR_BUILD_VERSION
}

if ($env:SonarScannerToken -ne $null)
{
    dotnet tool install --global dotnet-sonarscanner
    dotnet sonarscanner begin /k:$env:SonarScannerToken /d:sonar.host.url="https://sonarcloud.io"
}

dotnet build -c Release /p:packageversion=$version
dotnet test
dotnet pack -c Release -o artifacts --no-build /p:packageversion=$version

if ($env:SonarScannerToken -ne $null)
{
    dotnet sonarscanner end 
}

