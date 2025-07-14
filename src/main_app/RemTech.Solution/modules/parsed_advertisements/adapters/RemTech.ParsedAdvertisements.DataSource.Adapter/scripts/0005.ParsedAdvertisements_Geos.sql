CREATE TABLE IF NOT EXISTS parsed_advertisements_module.geos(
    id                      UUID PRIMARY KEY,
    text                    VARCHAR(50) UNIQUE NOT NULL,
    kind                    VARCHAR(50) NOT NULL,
    document_tsvector       TSVECTOR
);

CREATE INDEX IF NOT EXISTS idx_vehicle_geos_tsvector
    ON parsed_advertisements_module.geos USING GIN(document_tsvector);

CREATE INDEX IF NOT EXISTS idx_vehicle_geos_text
    ON parsed_advertisements_module.geos(text);

CREATE INDEX IF NOT EXISTS idx_vehicle_geos_kind
    ON parsed_advertisements_module.geos(kind);

CREATE OR REPLACE FUNCTION parsed_advertisements_module.update_text_document_tsvector()
RETURNS trigger AS $$
BEGIN
  NEW.document_tsvector := to_tsvector('russian', COALESCE(NEW.text, ''));
RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_trigger
        WHERE tgname = 'trg_update_geos_document_tsvector'
        AND tgrelid = 'parsed_advertisements_module.geos'::regclass
    ) THEN
CREATE TRIGGER trg_update_geos_document_tsvector
    BEFORE INSERT OR UPDATE ON parsed_advertisements_module.geos
                         FOR EACH ROW EXECUTE FUNCTION parsed_advertisements_module.update_text_document_tsvector();
END IF;
END
$$;

DO $$
BEGIN
INSERT INTO
    parsed_advertisements_module.geos(id, text, kind)
VALUES
    (uuid_generate_v4(), 'Алтайский', 'край'),
    (uuid_generate_v4(), 'Амурская', 'область'),
    (uuid_generate_v4(), 'Архангельская', 'область'),
    (uuid_generate_v4(), 'Астраханская', 'область'),
    (uuid_generate_v4(), 'Белгородская', 'область'),
    (uuid_generate_v4(), 'Брянская', 'область'),
    (uuid_generate_v4(), 'Владимирская', 'область'),
    (uuid_generate_v4(), 'Волгоградская', 'область'),
    (uuid_generate_v4(), 'Вологодская', 'область'),
    (uuid_generate_v4(), 'Воронежская', 'область'),
    (uuid_generate_v4(), 'Еврейская', 'автономная область'),
    (uuid_generate_v4(), 'Ивановская', 'область'),
    (uuid_generate_v4(), 'Иркутская', 'область'),
    (uuid_generate_v4(), 'Кабардино-Балкарская', 'Республика'),
    (uuid_generate_v4(), 'Калининградская', 'Область'),
    (uuid_generate_v4(), 'Карачаево-Черкесская', 'Республика'),
    (uuid_generate_v4(), 'Кировская', 'область'),
    (uuid_generate_v4(), 'Липецкая', 'область'),
    (uuid_generate_v4(), 'Московская', 'область'),
    (uuid_generate_v4(), 'Мурманская', 'область'),
    (uuid_generate_v4(), 'Ненецкий автономный', 'округ'),
    (uuid_generate_v4(), 'Нижегородская', 'область'),
    (uuid_generate_v4(), 'Новгородская', 'область'),
    (uuid_generate_v4(), 'Новосибирская', 'область'),
    (uuid_generate_v4(), 'Омская', 'область'),
    (uuid_generate_v4(), 'Оренбургская', 'область'),
    (uuid_generate_v4(), 'Пензенская', 'область'),
    (uuid_generate_v4(), 'Пермский', 'край'),
    (uuid_generate_v4(), 'Псковская', 'область'),
    (uuid_generate_v4(), 'Адыгея', 'Республика '),
    (uuid_generate_v4(), 'Республика', 'Бурятия'),
    (uuid_generate_v4(), 'Республика', 'Карелия'),
    (uuid_generate_v4(), 'Республика', 'Мордовия'),
    (uuid_generate_v4(), 'Саха', 'Республика'),
    (uuid_generate_v4(), 'Северная Осетия', 'Республика'),
    (uuid_generate_v4(), 'Рязанская', 'область'),
    (uuid_generate_v4(), 'Самарская', 'область'),
    (uuid_generate_v4(), 'Санкт-Петербург', 'город'),
    (uuid_generate_v4(), 'Москва', 'город'),
    (uuid_generate_v4(), 'Саратовская', 'область'),
    (uuid_generate_v4(), 'Сахалинская', 'область'),
    (uuid_generate_v4(), 'Свердловская', 'область'),
    (uuid_generate_v4(), 'Смоленская', 'область'),
    (uuid_generate_v4(), 'Ставропольский', 'край'),
    (uuid_generate_v4(), 'Тамбовская', 'область'),
    (uuid_generate_v4(), 'Тверская', 'область'),
    (uuid_generate_v4(), 'Тюменская', 'область'),
    (uuid_generate_v4(), 'Удмуртская', 'Республика'),
    (uuid_generate_v4(), 'Ульяновская', 'область'),
    (uuid_generate_v4(), 'Хабаровский', 'край'),
    (uuid_generate_v4(), 'Красноярский', 'край'),
    (uuid_generate_v4(), 'Краснодарский', 'край'),
    (uuid_generate_v4(), 'Ханты-Мансийский', 'автономный округ'),
    (uuid_generate_v4(), 'Челябинская', 'область'),
    (uuid_generate_v4(), 'Чеченская', 'Республика'),
    (uuid_generate_v4(), 'Чукотский', 'автономный округ'),
    (uuid_generate_v4(), 'Ямало-Ненецкий', 'автономный округ'),
    (uuid_generate_v4(), 'Ярославская', 'область'),
    (uuid_generate_v4(), 'Калужская', 'область'),
    (uuid_generate_v4(), 'Костромская', 'область'),
    (uuid_generate_v4(), 'Курская', 'область'),
    (uuid_generate_v4(), 'Орловская', 'область'),
    (uuid_generate_v4(), 'Тульская', 'область'),
    (uuid_generate_v4(), 'Коми', 'Республика'),
    (uuid_generate_v4(), 'Ленинградская', 'область'),
    (uuid_generate_v4(), 'Дагестан', 'Республика'),
    (uuid_generate_v4(), 'Ингушетия', 'Республика'),
    (uuid_generate_v4(), 'Калмыкия', 'Республика'),
    (uuid_generate_v4(), 'Северная Осетия - Алания', 'Республика'),
    (uuid_generate_v4(), 'Ростовская', 'область'),
    (uuid_generate_v4(), 'Башкортостан', 'Республика'),
    (uuid_generate_v4(), 'Марий Эл', 'Республика'),
    (uuid_generate_v4(), 'Татарстан', 'Республика'),
    (uuid_generate_v4(), 'Чувашская', 'Республика'),
    (uuid_generate_v4(), 'Пермская', 'область'),
    (uuid_generate_v4(), 'Курганская', 'область'),
    (uuid_generate_v4(), 'Ханты-Мансийский', 'автономный округ'),
    (uuid_generate_v4(), 'Алтай', 'Республика'),
    (uuid_generate_v4(), 'Тыва', 'Республика'),
    (uuid_generate_v4(), 'Хакасия', 'Республика'),
    (uuid_generate_v4(), 'Эвенкийский', 'автономный округ'),
    (uuid_generate_v4(), 'Кемеровская', 'область'),
    (uuid_generate_v4(), 'Томская', 'область'),
    (uuid_generate_v4(), 'Читинская', 'область'),
    (uuid_generate_v4(), 'Приморский', 'край'),
    (uuid_generate_v4(), 'Камчатская', 'область'),
    (uuid_generate_v4(), 'Корякский', 'автономный округ'),
    (uuid_generate_v4(), 'Магаданская', 'область')
    ON CONFLICT(text) DO NOTHING;
END $$;
