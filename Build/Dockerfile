FROM mcr.microsoft.com/dotnet/runtime-deps:6.0 as base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS publish
WORKDIR /src
COPY ["src/Elzik.Mecon.Console/Elzik.Mecon.Console.csproj", "src/Elzik.Mecon.Console/"]
RUN dotnet restore "src/Elzik.Mecon.Console/Elzik.Mecon.Console.csproj" -v n
COPY . .
RUN dotnet publish src/Elzik.Mecon.Console/Elzik.Mecon.Console.csproj -r linux-x64 -c Release --self-contained true -p:PublishTrimmed=true -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["/app/mecon"]