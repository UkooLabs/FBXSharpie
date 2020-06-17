if ($env:NugetPushApiToken -ne $null)
{
	$key = $env:NugetPushApiToken
	.\nuget push artifacts\*.nupkg -ApiKey $key -source https://api.nuget.org/v3/index.json
}