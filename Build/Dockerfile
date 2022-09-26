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
COPY Build/keep-alive.sh .
LABEL org.opencontainers.image.source=https://github.com/elzik/mecon
LABEL org.opencontainers.image.description="Find media that exists in Plex but needn't and media that you expect to be present but isn't"
LABEL org.opencontainers.image.licenses=GPL-3.0
ENV PATH /app:$PATH
ENTRYPOINT ["/bin/bash", "keep-alive.sh"]