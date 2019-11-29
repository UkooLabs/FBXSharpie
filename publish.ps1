if ($env:NugetPushApiToken -ne $null)
{
	dotnet nuget push -k $Env:NugetPushApiToken artifacts\*.nupkg -s https://www.nuget.org/api/v3/package
}