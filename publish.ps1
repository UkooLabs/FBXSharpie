if ($env:NugetPushApiToken -ne $null)
{
	echo $env:NugetPushApiToken
	.\nuget push artifacts\*.nupkg -ApiKey $env:NugetPushApiToken -source https://api.nuget.org/v3/index.json
}