FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build


# делаем текущей папкой рутовую папку, куда будут копироваться все файлы для билда и ресурсы
WORKDIR /app

# копирование всех файлов для билда
COPY ["../main_app/RemTech.Solution/", "."]

# копирование пакетов из репозотрия для билда:
COPY ["../nuget.config", "."]

# делаем текущей папку папку с проектом, который будем билдить
WORKDIR /app

# восстанавливаем зависимости и билдим проект

RUN dotnet publish "./WebHostApplication/WebHostApplication.csproj" -c Release -r linux-x64 -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app/publish
EXPOSE 5238
EXPOSE 5185

COPY --from=build /app/publish .
COPY ["../onnx/", "./onnx/"]
COPY ../.env .

ENTRYPOINT ["dotnet", "WebHostApplication.dll"]
