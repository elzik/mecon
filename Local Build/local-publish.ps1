dotnet publish ..\src\Elzik.Mecon.Console\Elzik.Mecon.Console.csproj `
	-p:PublishSingleFile=true `
	-r linux-x64 `
	-c Release `
	--self-contained true `
	-p:PublishTrimmed=true `
	-o .\output\linux-x64

dotnet publish ..\src\Elzik.Mecon.Console\Elzik.Mecon.Console.csproj `
	-p:PublishSingleFile=true `
	-r win-x64 `
	-c Release `
	--self-contained true `
	-p:PublishTrimmed=true `
	-o .\output\win-x64

	dotnet publish ..\src\Elzik.Mecon.Console\Elzik.Mecon.Console.csproj `
	-p:PublishSingleFile=true `
	-r osx-x64 `
	-c Release `
	--self-contained true `
	-p:PublishTrimmed=true `
	-o .\output\osx-x64