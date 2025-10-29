namespace ParsedAdvertisements.Adapters.Storage;

public sealed class ParsedAdvertisementsCharacteristicsInitialSeedingCollection
{
    private static readonly ParsedAdvertisementCharacteristicSeedingDetail[] _details =
    [
        new(Guid.NewGuid(), "Тип кузова", null, new HashSet<string>(
        [
            "Самосвал",
            "Бортовой грузовик",
            "Тентованный грузовик",
            "Рефрижератор",
            "Изометрический фургон",
            "Промтоварный фургон",
            "Эвакуатор",
            "Зерновоз",
            "Шасси",
            "Лесовоз",
            "Автовоз",
            "Баллоновоз",
            "Бортовой грузовик",
            "Зерновоз",
            "Изотермический фургон",
            "Контейнеровоз",
            "Кунг",
            "Лесовоз",
            "Сортиментовоз",
            "Ломовоз",
            "Промтоварный фургон",
            "Рефрижератор",
            "Самосвал",
            "Скотовоз",
            "Тентованный грузовик",
            "Топливозаправщик",
            "Трубовоз",
            "Мусоровоз",
            "Бункеровоз",
            "Хлебовоз",
            "Щеповоз",
            "Коневоз",
            "Птицевоз",
            "Цистерна для перевозки пищевых жидкостей",
            "Цистерна для перевозки светлых нефтепродуктов",
            "Цистерна для перевозки тёмных нефтепродуктов",
            "Цистерна для перевозки хим. жидкостей",
            "Цистерна для перевозки сыпучих грузов",
            "Автогудронатор",
            "Эвакуатор",
            "Площадка без бортов",
            "Ломанная платформа",
            "Сдвижная платформа",
            "Шасси",
            "Автоцистерна",
        ])),
        new(Guid.NewGuid(), "Состояние", null, new HashSet<string>(
        [
            "Новое",
            "Б/у"
        ])),
        new(Guid.NewGuid(), "Пробег", "км", new HashSet<string>()),
        new(Guid.NewGuid(), "Год выпуска", "год", new HashSet<string>()),
        new(Guid.NewGuid(), "Тип двигателя", null, new HashSet<string>(
        [
            "Бензин",
            "Газ",
            "Газ/Бензин",
            "Газ/Дизель",
            "Гибрид",
            "Дизель",
            "Электро",
            "Этанол",
        ])),
        new(Guid.NewGuid(), "Мощность", "л.с.", new HashSet<string>()),
        new(Guid.NewGuid(), "Объём двигателя", "л.", new HashSet<string>()),
        new(Guid.NewGuid(), "Экологический класс", "л.", new HashSet<string>(
        [
            "Евро 0",
            "Евро 1",
            "Евро 2",
            "Евро 3",
            "Евро 4",
            "Евро 5",
            "Евро 6",
        ])),
        new(Guid.NewGuid(), "Объём топливного бака", "л.", new HashSet<string>()),
        new(Guid.NewGuid(), "Марка прицепа", null, new HashSet<string>()),
        new(Guid.NewGuid(), "Тип прицепа", null, new HashSet<string>()),
        new(Guid.NewGuid(), "Модель прицепа", null, new HashSet<string>()),
        new(Guid.NewGuid(), "Тип кабины", null, new HashSet<string>()),
        new(Guid.NewGuid(), "Задняя подвеска", null, new HashSet<string>(
        [
            "Рессорная",
            "Пневматическая",
            "Балансирная",
        ])),
        new(Guid.NewGuid(), "Коробка передач", "л.", new HashSet<string>(
        [
            "Автомат",
            "Механика",
            "Полуавтомат",
            "Робот",
        ])),
        new(Guid.NewGuid(), "Колесная формула", "л.", new HashSet<string>(
        [
            "10 на 10",
            "10 на 2",
            "10 на 4",
            "10 на 6",
            "10 на 8",
            "10 на 8.1",
            "12 на 12",
            "12 на 12.1",
            "12 на 4",
            "12 на 8",
            "3 на 2",
            "4 на 2",
            "4 на 4",
            "6 на 2",
            "6 на 2/4",
            "6 на 4",
            "6 на 6",
            "6 на 6.1",
            "8 на 2",
            "8 на 2/6",
            "8 на 4/4",
            "8 на 8.1",
            "10 x 10",
            "10 x 2",
            "10 x 4",
            "10 x 6",
            "10 x 8",
            "10 x 8.1",
            "12 x 12",
            "12 x 12.1",
            "12 x 4",
            "12 x 8",
            "3 x 2",
            "4 x 2",
            "4 x 4",
            "6 x 2",
            "6 x 2/4",
            "6 x 4",
            "6 x 6",
            "6 x 6.1",
            "8 x 2",
            "8 x 2/6",
            "8 x 4/4",
            "8 x 8.1",
        ])),
        new(Guid.NewGuid(), "Подвеска кабины", null, new HashSet<string>(
        [
            "Гидроподъем",
            "Механическая",
            "Пневматическая",
            "Пружинная",
            "Рессорная",
        ])),
        new(Guid.NewGuid(), "Тип кабины", null, new HashSet<string>(
        [
            "2-х местная без спального",
            "2-х местная с 1 спальным",
            "2-х местная с 2 спальными",
            "3-х местная без спального",
            "3-х местная с 1 спальным",
        ])),
        new(Guid.NewGuid(), "Тип тормозов", null, new HashSet<string>(
        [
            "Барабанные",
            "Дисковые",
            "Отсутствуют",
        ])),
        new(Guid.NewGuid(), "Ход (трактора), ходовая часть", null, new HashSet<string>(
        [
            "Колесный",
            "Гусеничный",
            "Колесно-гусеничный",
        ])),
        new(Guid.NewGuid(), "Привод", null, new HashSet<string>(
        [
            "Полный",
            "Передний",
            "Задний",
        ])),
        new(Guid.NewGuid(), "Привод", null, new HashSet<string>(
        [
            "Полный",
            "Передний",
            "Задний",
        ])),
        new(Guid.NewGuid(), "Тип пресс-подборщика", null, new HashSet<string>(
        [
            "Рулонный",
            "Тюковый",
        ])),
        new(Guid.NewGuid(), "Тип пресс-подборщика", null, new HashSet<string>(
        [
            "Рулонный",
            "Тюковый",
        ])),
        new(Guid.NewGuid(), "Эксплуатационная масса", "кг.", new HashSet<string>()),
        new(Guid.NewGuid(), "Грузоподъемность", "т.", new HashSet<string>()),
        new(Guid.NewGuid(), "Моточасть", "ч.", new HashSet<string>()),
        new(Guid.NewGuid(), "Длина стрелы", null, new HashSet<string>()),
        new(Guid.NewGuid(), "Тип двигателя", null, new HashSet<string>(
        [
            "Бензин",
            "Дизель",
            "Электро",
        ])),
        new(Guid.NewGuid(), "Тип грабель", null, new HashSet<string>(
        [
            "Колесно-пальцевые",
            "Ленточный валкователь",
            "Поперечные",
            "Роторные",
            "Снегопах",
            "Цепные",
        ])),
        new(Guid.NewGuid(), "Ширина захвата", "м.", new HashSet<string>()),
    ];
}

public sealed class ParsedAdvertisementCharacteristicSeedingDetail(
    Guid Id,
    string Name,
    string? Measurement,
    object extras);
// INSERT INTO parsed_advertisements_module.characteristics(id, name, measurement, extras, embedding, extras_embedding);