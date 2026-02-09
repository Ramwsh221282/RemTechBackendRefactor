FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# делаем текущей папкой рутовую папку, куда будут копироваться все файлы для билда и ресурсы
WORKDIR /app

# копирование всех файлов для билда
COPY ["../main_app/RemTech.Solution/", "."]
COPY ["../nuget.config", "."]
RUN dotnet publish "./WebHostApplication/WebHostApplication.csproj" -c Release -r linux-x64 -o /app/publish

FROM mcr.microsoft.com/dotnet/sdk:10.0

RUN apt-get update && apt-get install -y \
    libgssapi-krb5-2 \
    libstdc++6 \
    libgcc1 \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish .
COPY ["../onnx/", "./onnx/"]
COPY ../.env .

EXPOSE 5238
EXPOSE 5185

ENTRYPOINT ["dotnet", "WebHostApplication.dll"]