# Гайд по конфигам для разворачиванию на сервере/локальной разработки:

---

### 1. Убеждаемся, что работаем в папке, в которой выполняется разворачивание:

`/src`

**Убеждаемся, что структура папки имеет следующие файлы, необходимые для билда:**

- `/src`
  - `/src/deploy-compose.yml`
  - `/src/nginx.conf`
  - `/src/nuget.config`
  - `/src/.env`
  - `/src/onnx`
    - `/src/onnx/model.onnx`
    - `/src/onnx/tokenizer.onnx`
    - `/src/onnx/model.onnx_data`

Так же в этой папке должны быть проекты backend и frontend (`parser apps`, `main_app`, `frontend_app`)

---

### 2. Пример конфига .env:

```.env
RabbitMqOptions__Hostname=<хост rabbit mq>
RabbitMqOptions__Password=<пароль rabbit mq>
RabbitMqOptions__Username=<пользователь rabbit mq>
RabbitMqOptions__Port=<порт rabbit mq>

NpgSqlOptions__Host=<хост (ссылка) npgSQL>
NpgSqlOptions__Port=<внешний порт npgSQL>
NpgSqlOptions__Database=<база данных npgSQL>
NpgSqlOptions__Username=<пользователь npgSQL>
NpgSqlOptions__Password=<пароль npgSQL>

EmbeddingsProviderOptions__TokenizerPath=<путь к tokenizer .onnx>
EmbeddingsProviderOptions__ModelPath=<путь к модели .onnx>

AesEncryptionOptions__PlainKey=<хеш строка для AeS шифрования>

SuperUserCredentialsOptions__Login=<логин первого пользователя>
SuperUserCredentialsOptions__Email=<почта первого пользователя>
SuperUserCredentialsOptions__Password=<пароль первого пользователя>

BcryptWorkFactorOptions__WorkFactor=<сложность хеширования. Больше значение - сложнее брутфорсить>

JwtOptions__SecretKey=<Рандомная строка>
JwtOptions__Issuer=<Ссылка на бекенд>
JwtOptions__Audience=<Ссылка на фронтенд>

CachingOptions__RedisConnectionString=<строка подключения к Redis>
CachingOptions__LocalCacheExpirationMinutes=<время истечения данных в локальном кеше>
CachingOptions__CacheExpirationMinutes=<время истечения данных в распределенном кеше>

FrontendOptions__Url=<ссылка на фронтенд>
```

---

### Пример конфига nuget.config:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <packageSources>
        <clear />
        <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
        <add key="gitlab" value="https://gitlab.com/api/v4/projects/77341701/packages/nuget/index.json" />
    </packageSources>
    <packageSourceCredentials>
        <gitlab>
            <add key="Username" value="пользователь gitlab у которого есть доступ к репозиторию и пакетам" />
            <add key="ClearTextPassword" value="PAT токен" />
        </gitlab>
    </packageSourceCredentials>
</configuration>
```

---

### 3. Запуск:

Используем docker compose для запуска сервисов:

`docker compose -p <название проекта в docker compose> -f <путь к docker compose файлу> up -d --build`

## Для локальной разработки используется.

**Для локальной разработки используется appsettings.json и docker-compose, который лежит по пути:**

`~/src/main_app/RemTech_Solution/docker-compose.yml`

---

### 1. Настройки appsettings для основного backend приложения:

```json
    "RabbitMqOptions": {
        "Hostname": "localhost",
        "Password": "пароль rabbit mq",
        "Username": "пользователь rabbit mq",
        "Port": "5672"
    },
    "NpgSqlOptions": {
        "Host": "localhost",
        "Port": "5535",
        "Database": "название БД",
        "Username": "пользователь npgSQL",
        "Password": "пароль npgSQL"
    },
    "EmbeddingsProviderOptions": {
        "TokenizerPath": "путь к tokenizer.onnx",
        "ModelPath": "путь к model.onnx"
    },
    "AesEncryptionOptions": {
        "PlainKey": "хеш ключ для AeS"
    },
    "SuperUserCredentialsOptions": {
        "Login": "логин первого пользователя",
        "Email": "почта первого пользователя",
        "Password": "пароль первого пользователя"
    },
    "BcryptWorkFactorOptions": {
        "WorkFactor": сложность хеширования
    },
    "JwtOptions": {
        "SecretKey": "рандомная строка для ключа jwt",
        "Issuer": "ссылка на бекенд",
        "Audience": "ссылка на фронтенд (или кто будет использовать токены бекенда)"
    },
    "CachingOptions": {
        "RedisConnectionString": "localhost:6379",
        "LocalCacheExpirationMinutes": время локального кеша,
        "CacheExpirationMinutes": время распределенного кеша
    },
    "FrontendOptions": {
        "Url": "ссылка на фронтенд"
    }
```

---

### 2. Настройки appsettings для сервисов-парсеров:

```json
    "RabbitMqRequestReplyResponseListeningQueueOptions": {
        "ResponseQueue": "Очередь, на которую будет отправлено сообщение о подтверждении подписки с бекенда", // например Avito.Запчасти.confirmation
        "ResponseExchange": "Обменник на который будет отправлено соощение с бекенда о подтверждении подписки, например Avito.Запчасти", //  Avito.Запчасти
        "ResponseRoutingKey": "Ключ маршрутизации на который будет отправлено сообщение с бекенда о подтверждении подписки", // например Avito.Запчасти.confirmation
        "SubscribtionParserDomain": "домен сервиса парсера - по сути название сервиса парсера", // например Avito
        "SubscribtionParserType": "тип объявлений который парсит парсер", // например Запчасти
        "RequestQueue": "create.parsers", // дефолт, если не меняли в основном приложении. Очередь куда будет слаться запрос на подписку
        "RequestExchange": "parsers", // дефолт, если не меняли в основном приложении. Обменник куда будет слаться запрос на подписку
        "RequestRoutingKey": "parsers.creation" // дефолт, если не меняли в основном приложении. Ключ маршрутизации куда будет слаться запрос на подписку
  },
    "ParserStartOptions": {
        "Queue": "очередь в которую основной бекенд будет слать запрос на запуск парсера Avito.Запчасти.start", // например Avito.Запчасти.start. СТРОИТСЯ КАК <Название домена> + <Тип объявлений который парсит> + <start> в основном бекенде.
        "Exchange": "обменник в которую основной бекенд будет слать запрос на запуск парсера", // СТРОИТСЯ КАК <Название домена> + <Тип объявлений который парсит> в основном бекенде.
        "RoutingKey": "очередь в которую основной бекенд будет слать запрос на запуск парсера" // например Avito.Запчасти.start. СТРОИТСЯ КАК <Название домена> + <Тип объявлений который парсит> + <start> в основном бекенде.
    },
    // настройки инфраструктуры следующей должны быть такие же как и у основного бекенда
    "RabbitMqOptions": {
        "Hostname": "localhost",
        "Password": "password",
        "Username": "admin",
        "Port": "5672"
    },
    "NpgSqlOptions": {
        "Host": "localhost",
        "Port": "5535",
        "Database": "",
        "Username": "",
        "Password": ""
    },
    "ScrapingBrowserOptions": {
        "BrowserPath": "", // путь к браузеру. Если путь - пустая строка, то сервис сам скачает браузер и будет использовать путь скачанного браузера. Используется chromium.
        "Headless": false // включить GUI интерфейс браузера или нет
    }
```
