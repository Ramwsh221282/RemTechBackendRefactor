FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /app

# копируем файлы для сборки парсера + nuget config
COPY ["../../parser_apps/RemtechDromVehiclesParser/DromVehiclesParser/",  "."]
COPY ["../../nuget.config", "."]

WORKDIR /app

RUN dotnet publish "./DromVehiclesParser.csproj" -c Release -r linux-x64 -o /app/publish

FROM mcr.microsoft.com/dotnet/sdk:10.0

# chrome браузер из старой версии парсера
# RUN apt-get update && \
#     apt-get install -y wget gnupg2 && \
#     wget -q -O - https://dl.google.com/linux/linux_signing_key.pub | apt-key add - && \
#     echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google-chrome.list && \
#     apt-get update && \
#     apt-get install -y google-chrome-stable && \
#     apt-get clean && \
#     rm -rf /var/lib/apt/lists/* /tmp/* /var/tmp/*

RUN apt-get update && \
    apt-get install -y chromium-browser && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/* /tmp/* /var/tmp/*

ENV PUPPETEER_SKIP_CHROMIUM_DOWNLOAD=true
ENV PUPPETEER_SKIP_DOWNLOAD=true
ENV CHROME_BIN=/usr/bin/chromium-browser

WORKDIR /app/publish

COPY --from=build /app/publish .
COPY ../../.env .

ENTRYPOINT ["dotnet", "DromVehiclesParser.dll"]