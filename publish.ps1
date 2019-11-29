if ($env:NugetPushApiToken -ne $null)
{
	dotnet nuget push -k $Env:NugetPushApiToken artifacts\*.nupkg -s https://www.nuget.org/api/v2/package
	dotnet nuget push -k $Env:NugetPushApiToken artifacts\*.snupkg -s https://www.nuget.org/api/v2/package
}