FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /app

# копируем файлы для сборки парсера + nuget config
COPY ["../../parser_apps/RemTechAvitoVehiclesParser/RemTechAvitoVehiclesParser/",  "."]
COPY ["../../nuget.config", "."]

WORKDIR /app

RUN dotnet publish "./RemTechAvitoVehiclesParser.csproj" -c Release -r linux-x64 -o /app/publish

FROM mcr.microsoft.com/dotnet/sdk:10.0

RUN apt-get update && \
    apt-get install -y wget gnupg2 ca-certificates && \
    wget -q -O /tmp/google-chrome-key.pub https://dl.google.com/linux/linux_signing_key.pub && \
    gpg --dearmor -o /usr/share/keyrings/google-chrome-keyring.gpg /tmp/google-chrome-key.pub && \
    echo "deb [arch=amd64 signed-by=/usr/share/keyrings/google-chrome-keyring.gpg] http://dl.google.com/linux/chrome/deb/ stable main" > /etc/apt/sources.list.d/google-chrome.list && \
    apt-get update && \
    apt-get install -y google-chrome-stable \
        fonts-liberation \
        libxss1 \
        libappindicator3-1 \
        libatk-bridge2.0-0 \
        libgtk-3-0 \
        libgbm1 \
        libnss3 \
        libxshmfence1 \
        --no-install-recommends && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/* /tmp/* /var/tmp/*

ENV PUPPETEER_SKIP_CHROMIUM_DOWNLOAD=true
ENV PUPPETEER_SKIP_DOWNLOAD=true
ENV CHROME_BIN=/usr/bin/google-chrome

ENV RabbitMqRequestReplyResponseListeningQueueOptions__ResponseQueue=Avito.Техника.confirmation
ENV RabbitMqRequestReplyResponseListeningQueueOptions__ResponseExchange=Avito.Техника
ENV RabbitMqRequestReplyResponseListeningQueueOptions__ResponseRoutingKey=Avito.Техника.confirmation
ENV RabbitMqRequestReplyResponseListeningQueueOptions__SubscribtionParserDomain=Avito
ENV RabbitMqRequestReplyResponseListeningQueueOptions__SubscribtionParserType=Техника
ENV RabbitMqRequestReplyResponseListeningQueueOptions__RequestQueue=parsers.create
ENV RabbitMqRequestReplyResponseListeningQueueOptions__RequestExchange=parsers
ENV RabbitMqRequestReplyResponseListeningQueueOptions__RequestRoutingKey=parsers.create

ENV ParserStartOptions__Queue=Avito.Техника.start
ENV ParserStartOptions__Exchange=Avito.Техника
ENV ParserStartOptions__RoutingKey=Avito.Техника.start

WORKDIR /app/publish

COPY --from=build /app/publish .
COPY ../../.env .

ENTRYPOINT ["dotnet", "RemTechAvitoVehiclesParser.dll"]