$runtimes = "linux-x64", "win-x64", "osx-x64"

foreach ($runtime in $runtimes)
{
	dotnet publish $PSScriptRoot\..\src\Elzik.Mecon.Console\Elzik.Mecon.Console.csproj `
		-p:PublishSingleFile=true `
		-r $runtime `
		-c Release `
		--self-contained true `
		-p:PublishTrimmed=true `
		-o $PSScriptRoot\output\$runtime
}