if ($env:NugetPushApiToken -ne $null)
{
	dotnet nuget push -k $env:NugetPushApiToken artifacts\*.nupkg -s https://api.nuget.org/v3/index.json
}