using System.Data;
using System.Globalization;
using System.Reflection;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Pgvector;
using Shared.Infrastructure.Module.Postgres;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace ParsedAdvertisements.Tests;

public static class IDoerFactory
{
    public static TechParametersClassificationTests.IDoer Loggable(this TechParametersClassificationTests.IDoer doer)
    {
        return TechParametersClassificationTests.LoggingProxy<TechParametersClassificationTests.IDoer>.Create(doer);
    }
}

[AttributeUsage(AttributeTargets.Method)]
public class LoggableAttribute : Attribute
{
}

public sealed class ValidatableAttribute<T> : Attribute
{
    private readonly Func<T, bool> _validation;

    public ValidatableAttribute(Func<T, bool> validation)
    {
        _validation = validation ?? throw new ArgumentNullException(nameof(validation));
    }

    public bool IsValid(T value)
    {
        return _validation(value);
    }
}

public sealed class TechParametersClassificationTests : IClassFixture<ParsedAdvertisementsServices>
{
    private readonly ParsedAdvertisementsServices _services;

    public TechParametersClassificationTests(ParsedAdvertisementsServices services)
    {
        _services = services;
    }

    public class LoggingProxy<T> : DispatchProxy where T : class
    {
        private T _target = null!;

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (targetMethod == null)
                throw new ApplicationException("Can't execute. Target method is null.");

            if (targetMethod.GetCustomAttribute<LoggableAttribute>() != null)
            {
                Console.WriteLine($"Invoking method: {targetMethod.Name}");
                var result = targetMethod.Invoke(_target, args);
                return result;
            }

            return null;
        }

        public static T Create(T decorated)
        {
            object? proxy = Create<T, LoggingProxy<T>>();
            ((LoggingProxy<T>)proxy!)?.SetParameters(decorated);
            return (T)proxy!;
        }

        private void SetParameters(T decorated)
        {
            _target = decorated ?? throw new ArgumentNullException(nameof(decorated));
        }
    }

    public class ValidatingProxy<T, U> : DispatchProxy where T : class
    {
        private T _target = null!;

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (targetMethod == null)
                throw new ApplicationException("Can't execute. Target method is null.");

            var attribute = targetMethod.GetCustomAttribute<ValidatableAttribute<U>>();
            if (attribute != null)
            {
                foreach (var @object in args)
                {
                    if (@object is not null)
                    {
                        if (@object is U u)
                            attribute.IsValid(u);
                    }
                }
            }

            return targetMethod.Invoke(_target, args);
        }

        public static T Create(T decorated)
        {
            object? proxy = Create<T, ValidatingProxy<T, U>>();
            ((ValidatingProxy<T, U>)proxy!)?.SetParameters(decorated);
            return (T)proxy!;
        }

        private void SetParameters(T decorated)
        {
            _target = decorated ?? throw new ArgumentNullException(nameof(decorated));
        }
    }

    public interface IDoer
    {
        void Do();
        void NotDo();
        void Validatble(string value);
    }

    public sealed class Doer : IDoer
    {
        [Loggable]
        public void Do()
        {
        }

        public void NotDo()
        {
        }

        public void Validatble([Validatable<string>(x => !string.IsNullOrEmpty(x) && x.Length > 3)] string value)
        {
            Console.WriteLine("Valid");
        }
    }

    [Fact]
    private void Proxy_Test()
    {
        IDoer doer = new Doer().Loggable();
        doer.Do();
        doer.NotDo();
    }

    [Fact]
    public async Task Test()
    {
        await using var scope = _services.CreateScope();
        var scopedProvider = scope.ServiceProvider;
        var database = scopedProvider.GetRequiredService<PostgresDatabase>();
        var generator = scopedProvider.GetRequiredService<IEmbeddingGenerator>();
        using var connection = await database.ProvideConnection();

        await CreateEngineTypesTable(connection);
        await CreateEngineTopicsTable(connection);

        string[] categories =
        [
            "электро",
            "гибридный",
            "газ",
            "бензин",
            "этанол",
            "дизель"
        ];

        foreach (var category in categories)
        {
            await AddEngineType(category, generator, connection);
        }

        string topic = "Какой тип двигателя: дизельный, бензиновый, газовый, электрический или гибридный?";
        await AddEngineTopic(topic, generator, connection);

        string text1 = "Экскаватор после пожара ?";
        string text2 = "Дизельный тип двигателя ?";
        string text3 = "дизельный, жидкостного охлаждения, шестицилиндровый, с турбонаддувом ?";
        string text4 = "Экскаватор после пожара. На запчасти или восстановление...";
        string text5 =
            "Джoйстик c чeтыpьмя переключaтeлями\nCиcтeма oтопления-кондициoнировaния с aвтoматичecким контролeм темпepaтуpы\nУнивеpcaльный ключ\nPадио с CD&МР3 рlаyеr & USВ ХС40802\nСолнцезащитные шторки в кабине (крыша, переднее и заднее стекло)\nРемень безопасности водителя, 75 мм\nОсновное рабочее освещение галогеновое\nДополнительные рабочие фары, 4 шт. (установлены на кабине и противовесе)\nСветодиодный проблесковый маячок\nПереключатель режимов работы с положением «Р»\nСистема доступа к компьютерам машины, GSМ/GРS\nЭлектрический подогреватель двигателя 240 В\nСтрела повышенной прочности длиной 6.2 м\nРукоять повышенной прочности длиной 3.05 м\nВкладыши рабочего оборудования в стандартном исполнении\nРычажный механизм ковша в стандартном исполнении\nРукоять с приваренными защитными пластинами с внутренней стороны\nТраки гусениц с тремя грунтозацепами, ширина 600 мм\nЗащита гусеницы стандартная\nХодовая тележка со складными ступенями для доступа в кабину\nУсиленная защита днища ходовой тележки, НD\nДвигатель Тiеr 2\nЗаправочный насос производительностью 35 л/мин\nПредочиститель \"циклон\"\nТопливный фильтр с водоотделителем и подогревом\nПрямой привод вентилятора\nКабина с неоткрывающимся люком\nКабина с антивандальной подготовкой";
        string text6 =
            "Характеристики\nМарка: ГАЗ\nМодель: Соболь 2752\nТип техники: Микроавтобус\nГод выпуска: 2023\nКоличество дверей: 5\nПоколение: II (2010–2025)\nТип двигателя: Дизель\nПривод: Задний\nКоробка передач: Механика\nМодификация: 2.8 D MT (120 л.с.)\nДлина шасси: Стандарт (4810 мм)\nВысота кабины: Стандарт (2300 мм)\nТип кабины: Стандарт\nКомплектация: Базовая\nЦвет: Белый\nРуль: Левый\nКоличество мест: 7\nСостояние: Б/у\nСостояние б/у: Не битый\nПробег: 37 738 км\nВладельцев по ПТС: 1\nVIN, номер кузова или SN: X962*************\nДоступность: В наличии";

        var result1 = await DetectEngineTypeInText(text1, generator, connection);
        var result2 = await DetectEngineTypeInText(text2, generator, connection);
        var result3 = await DetectEngineTypeInText(text3, generator, connection);
        var result4 = await DetectEngineTypeInText(text4, generator, connection);
        var result5 = await DetectEngineTypeInText(text5, generator, connection);
        var result6 = await DetectEngineTypeInText(text6, generator, connection);
        int a = 0;
    }

    public async Task<(string, double, double)> DetectEngineTypeInText(
        string inputText,
        IEmbeddingGenerator generator,
        IDbConnection connection)
    {
        // Этап 1: Проверяем, есть ли вообще упоминание темы
        double relevancy = await GetRelevancyToEngineTopic(inputText, generator, connection);

        if (relevancy > 0.75)
            return ("не относится к типу двигателя", relevancy, 0.0);

        // Этап 2: Классифицируем по категориям
        var classification = await ClassifyEngineType(inputText, generator, connection);
        return (classification.Item1, relevancy, classification.Item2);
    }

    private async Task<double> GetRelevancyToEngineTopic(string inputText, IEmbeddingGenerator generator,
        IDbConnection connection)
    {
        // Получаем вектор входного текста
        var inputVector = generator.GenerateVector(inputText);

        // Ищем расстояние до темы "Тип двигателя: ..."
        const string sql = "SELECT embedding <=> @input FROM engine_topics LIMIT 1";

        double distance = await connection.ExecuteScalarAsync<double>(
            sql, new { input = inputVector });

        return distance;
    }

    private async Task<(string, double)> ClassifyEngineType(string inputText, IEmbeddingGenerator generator,
        IDbConnection connection)
    {
        var inputVector = generator.GenerateVector(inputText);

        const string sql = """
                           SELECT name, (embedding <=> @input) AS distance
                           FROM engine_types
                           ORDER BY distance
                           LIMIT 1;
                           """;

        var result = await connection.QuerySingleOrDefaultAsync<(string, double)>(
            sql, new { input = inputVector });

        const double CATEGORY_THRESHOLD = 0.65;
        return result.Item2 < CATEGORY_THRESHOLD
            ? result
            : ("тип двигателя в тексте не указан", result.Item2);
    }

    private async Task CreateEngineTypesTable(IDbConnection connection)
    {
        string sql = """
                     CREATE TABLE engine_types (
                         id UUID PRIMARY KEY,
                         name TEXT not null,
                         embedding vector(1024)
                     )
                     """;
        await connection.ExecuteAsync(sql);
    }

    private async Task AddEngineType(string type, IEmbeddingGenerator generator, IDbConnection connection)
    {
        var vector = generator.GenerateVector(type);
        var parameters = new DynamicParameters();
        parameters.Add("id", Guid.NewGuid(), DbType.Guid);
        parameters.Add("name", type, DbType.String);
        parameters.Add("embedding", vector);
        string sql = """
                     INSERT INTO engine_types (id, name, embedding)
                     VALUES (@id, @name, @embedding)
                     """;
        var command = new CommandDefinition(sql, parameters);
        await connection.ExecuteAsync(command);
    }

    private async Task CreateEngineTopicsTable(IDbConnection connection)
    {
        string sql = """
                     CREATE TABLE engine_topics (
                         id UUID PRIMARY KEY,
                         description TEXT not null,
                         embedding vector(1024)
                     )
                     """;
        await connection.ExecuteAsync(sql);
    }

    private async Task AddEngineTopic(string text, IEmbeddingGenerator generator, IDbConnection connection)
    {
        var vector = generator.GenerateVector(text);
        var parameters = new DynamicParameters();
        parameters.Add("id", Guid.NewGuid(), DbType.Guid);
        parameters.Add("description", text);
        parameters.Add("embedding", vector);
        string sql = """
                     INSERT INTO engine_topics (id, description, embedding)
                     VALUES (@id, @description, @embedding)
                     """;
        var command = new CommandDefinition(sql, parameters);
        await connection.ExecuteAsync(command);
    }

    private double GetVectorNormalization(string text, IEmbeddingGenerator generator)
    {
        var vectors = generator.Generate(text);

        double sum = 0.0;
        for (int i = 0; i < vectors.Span.Length; i++)
        {
            double vectorValue = vectors.Span[i];
            double calculation = vectorValue * vectorValue;
            sum += calculation;
        }

        return Math.Sqrt(sum);
    }

    private async Task<ReadOnlyMemory<float>> GetNormalizedVector(string text, IEmbeddingGenerator generator)
    {
        var vectors = generator.GenerateVector(text);
        double norm = 0;

        for (int i = 0; i < vectors.Memory.Span.Length; i++)
        {
            norm += vectors.Memory.Span[i] * vectors.Memory.Span[i];
        }

        norm = Math.Sqrt(norm);
        var result = new float[vectors.Memory.Span.Length];
        for (int i = 0; i < vectors.Memory.Span.Length; i++)
        {
            result[i] = (float)(vectors.Memory.Span[i] / norm);
        }

        return result;
    }

    private async Task<double> GetSimilarityFor(string text, IEmbeddingGenerator generator, IDbConnection connection)
    {
        var inputVector = generator.GenerateVector(text);

        string sql = """
                     SELECT  embedding <-> @inputVector
                     FROM tech_parameters
                     ORDER BY embedding <-> @inputVector
                     LIMIT 1
                     """;

        var parameters = new DynamicParameters();
        parameters.Add("inputVector", inputVector);

        var command = new CommandDefinition(sql, parameters);
        return await connection.ExecuteScalarAsync<double>(command);
    }

    private async Task<(string, double)> GetQueryResult(string text, IEmbeddingGenerator generator,
        IDbConnection connection)
    {
        var inputVector = generator.GenerateVector(text);

        string sql = """
                     SELECT 
                         parameter_measurement, 
                         embedding <-> @inputVector
                     FROM tech_parameters
                     ORDER BY embedding <-> @inputVector
                     LIMIT 1
                     """;
        var parameters = new DynamicParameters();
        parameters.Add("inputVector", inputVector);

        var command = new CommandDefinition(sql, parameters);
        return await connection.QueryFirstAsync<(string, double)>(command);
    }

    [Fact]
    public async Task Test2()
    {
        await using var scope = _services.CreateScope();
        var scopedProvider = scope.ServiceProvider;
        var database = scopedProvider.GetRequiredService<PostgresDatabase>();
        var generator = scopedProvider.GetRequiredService<IEmbeddingGenerator>();
        using var connection = await database.ProvideConnection();

        string[] techParameterCategories =
        [
            "Бензин",
            "Газ",
            "Газ/Бензин",
            "Газ/Дизель",
            "Гибридный",
            "Электро",
            "Этанол"
        ];

        ReadOnlyMemory<float>[] techParameterVectors =
            techParameterCategories.Select(cat => generator.Generate(cat)).ToArray();

        int dimension = 1024;
        var averageVector = await CalculateAverageVector(techParameterVectors, dimension, connection);
        double result1 = await Calculate("Экскаватор после пожара.", averageVector, generator, connection);
        double result2 = await Calculate("Дизельный тип двигателя", averageVector, generator, connection);
        int a = 0;
    }

    private async Task<double> Calculate(
        string inputText,
        Vector average,
        IEmbeddingGenerator generator,
        IDbConnection connection)
    {
        string sql = "SELECT 1 - (@average <-> @input)";
        var inputVector = generator.GenerateVector(inputText);

        var parameters = new DynamicParameters();
        parameters.Add("average", average);
        parameters.Add("input", inputVector);

        var command = new CommandDefinition(sql, parameters);
        return await connection.ExecuteScalarAsync<dynamic>(command);
    }

    private async Task<Vector> CalculateAverageVector(
        ReadOnlyMemory<float>[] vectorsArray,
        int dimension,
        IDbConnection connection)
    {
        List<string> vectorParameters = [];
        foreach (var vector in vectorsArray)
        {
            vectorParameters.Add($"({CreateRawVectorParameterString(vector, dimension)})");
        }

        string vectorClause = string.Join(',', vectorParameters);
        string sql = $"""
                      WITH vectors AS (
                      SELECT column1 as emb FROM (VALUES
                         {vectorClause}  
                         )
                      )
                      SELECT AVG(emb) FROM vectors;
                      """;

        var average = await connection.ExecuteScalarAsync<Vector>(sql);
        return average!;
    }

    private string CreateRawVectorParameterString(ReadOnlyMemory<float> vector, int dimension)
    {
        var span = vector.Span;
        var parts = new string[vector.Length];
        for (int i = 0; i < vector.Length; i++)
        {
            parts[i] = span[i].ToString("R", CultureInfo.InvariantCulture);
        }

        return $"'[{string.Join(",", parts)}]'::vector({dimension})";
    }

    private async Task CreateTechParametersTable(IDbConnection connection)
    {
        string sql = """
                     CREATE TABLE tech_parameters(
                         id UUID PRIMARY KEY,
                         parameter_topic VARCHAR(255) NOT NULL,
                         parameter_measurement VARCHAR(255),
                         embedding vector(1024)
                     );
                     """;
        await connection.ExecuteAsync(sql);
    }

    private async Task CreateTechParametersCategoriesTable(IDbConnection connection)
    {
        var sql = """
                  CREATE TABLE tech_parameters_categories(
                      id UUID PRIMARY KEY,
                      topic_id UUID REFERENCES tech_parameters(id),
                      value VARCHAR(255) NOT NULL,
                      embedding vector(1024)
                  );
                  """;
        await connection.ExecuteAsync(sql);
    }

    private async Task<Guid> AddTechParameterCategory(
        Guid topicId,
        string value,
        Vector vector,
        IDbConnection connection)
    {
        var sql = """
                  INSERT INTO tech_parameters_categories
                      (id, topic_id, value, embedding)
                  VALUES
                      (@id, @topic_id, @value, @embedding)
                  """;
        var id = Guid.NewGuid();

        var parameters = new DynamicParameters();
        parameters.Add("id", id, DbType.Guid);
        parameters.Add("topic_id", topicId, DbType.Guid);
        parameters.Add("value", value, DbType.String);
        parameters.Add("embedding", vector);
        var command = new CommandDefinition(sql, parameters);
        await connection.ExecuteAsync(command);
        return topicId;
    }

    private async Task<Guid> AddTechParameter(
        string topicName,
        string? topicMeasurement,
        Vector vector,
        IDbConnection connection)
    {
        var sql = """
                  INSERT INTO tech_parameters
                      (id, parameter_topic, parameter_measurement, embedding)
                  VALUES
                      (@id, @topic, @measurement, @embedding)
                  """;

        Guid id = Guid.NewGuid();
        var parameters = new DynamicParameters();
        parameters.Add("id", id, DbType.Guid);
        parameters.Add("topic", topicName, DbType.String);
        parameters.Add("measurement", topicMeasurement, DbType.String);
        parameters.Add("embedding", vector);
        var command = new CommandDefinition(sql, parameters);
        await connection.ExecuteAsync(command);
        return id;
    }
}