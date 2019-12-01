if ($env:NugetPushApiToken -ne $null)
{
	.\nuget push artifacts\*.nupkg -ApiKey $env:NugetPushApiToken -source https://api.nuget.org/v3/index.json
}