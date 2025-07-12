CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE IF NOT EXISTS parsed_advertisements_module.geos(
    id              UUID PRIMARY KEY,
    text            VARCHAR(80) UNIQUE NOT NULL,
    document_tsvector       TSVECTOR
);

DO $$
    BEGIN
    INSERT INTO
        parsed_advertisements_module.geos(id, text)
    VALUES
        (uuid_generate_v4(), 'Алтайский край'),
        (uuid_generate_v4(), 'Амурская область'),
        (uuid_generate_v4(), 'Архангельская область'),
        (uuid_generate_v4(), 'Астраханская область'),
        (uuid_generate_v4(), 'Белгородская область'),
        (uuid_generate_v4(), 'Брянская область'),
        (uuid_generate_v4(), 'Владимирская область'),
        (uuid_generate_v4(), 'Волгоградская область'),
        (uuid_generate_v4(), 'Вологодская область'),
        (uuid_generate_v4(), 'Воронежская область'),
        (uuid_generate_v4(), 'Еврейская автономная область'),
        (uuid_generate_v4(), 'Ивановская область'),
        (uuid_generate_v4(), 'Иркутская область'),
        (uuid_generate_v4(), 'Кабардино-Балкарская Республика'),
        (uuid_generate_v4(), 'Калининградская область'),
        (uuid_generate_v4(), 'Карачаево-Черкесская Республика'),
        (uuid_generate_v4(), 'Кировская область'),
        (uuid_generate_v4(), 'Липецкая область'),
        (uuid_generate_v4(), 'Московская область'),
        (uuid_generate_v4(), 'Мурманская область'),
        (uuid_generate_v4(), 'Ненецкий автономный округ'),
        (uuid_generate_v4(), 'Нижегородская область'),
        (uuid_generate_v4(), 'Новгородская область'),
        (uuid_generate_v4(), 'Новосибирская область'),
        (uuid_generate_v4(), 'Омская область'),
        (uuid_generate_v4(), 'Оренбургская область'),
        (uuid_generate_v4(), 'Пензенская область'),
        (uuid_generate_v4(), 'Пермский край'),
        (uuid_generate_v4(), 'Псковская область'),
        (uuid_generate_v4(), 'Республика Адыгея'),
        (uuid_generate_v4(), 'Республика Бурятия'),
        (uuid_generate_v4(), 'Республика Карелия'),
        (uuid_generate_v4(), 'Республика Мордовия'),
        (uuid_generate_v4(), 'Республика Саха (Якутия)'),
        (uuid_generate_v4(), 'Республика Северная Осетия'),
        (uuid_generate_v4(), 'Рязанская область'),
        (uuid_generate_v4(), 'Самарская область'),
        (uuid_generate_v4(), 'Санкт-Петербург'),
        (uuid_generate_v4(), 'Саратовская область'),
        (uuid_generate_v4(), 'Сахалинская область'),
        (uuid_generate_v4(), 'Свердловская область'),
        (uuid_generate_v4(), 'Смоленская область'),
        (uuid_generate_v4(), 'Ставропольский край'),
        (uuid_generate_v4(), 'Тамбовская область'),
        (uuid_generate_v4(), 'Тверская область'),
        (uuid_generate_v4(), 'Тюменская область'),
        (uuid_generate_v4(), 'Удмуртская Республика'),
        (uuid_generate_v4(), 'Ульяновская область'),
        (uuid_generate_v4(), 'Хабаровский край'),
        (uuid_generate_v4(), 'Красноярский край'),
        (uuid_generate_v4(), 'Краснодарский край'),
        (uuid_generate_v4(), 'Ханты-Мансийский автономный округ'),
        (uuid_generate_v4(), 'Челябинская область'),
        (uuid_generate_v4(), 'Чеченская Республика'),
        (uuid_generate_v4(), 'Чукотский автономный округ'),
        (uuid_generate_v4(), 'Ямало-Ненецкий автономный округ'),
        (uuid_generate_v4(), 'Ярославская область')
    ON CONFLICT(text) DO NOTHING;
END $$;
