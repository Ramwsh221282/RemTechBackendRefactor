CREATE TABLE IF NOT EXISTS parsed_advertisements_module.vehicle_characteristics(
    id                  UUID PRIMARY KEY,
    text                VARCHAR(80) UNIQUE NOT NULL,
    measuring           VARCHAR(10),
    document_tsvector   TSVECTOR,
    UNIQUE(text, measuring)
);

CREATE INDEX IF NOT EXISTS idx_vehicle_characteristics_tsvector
    ON parsed_advertisements_module.vehicle_characteristics USING GIN(document_tsvector);

CREATE INDEX IF NOT EXISTS idx_vehicle_characteristics_text
    ON parsed_advertisements_module.vehicle_characteristics(text);

CREATE
OR REPLACE FUNCTION parsed_advertisements_module.update_text_document_tsvector()
RETURNS trigger AS $$
BEGIN
  NEW.document_tsvector
:= to_tsvector('russian', COALESCE(NEW.text, ''));
RETURN NEW;
END;
$$
LANGUAGE plpgsql;

DO
$$
BEGIN
    IF
NOT EXISTS (
        SELECT 1 FROM pg_trigger
        WHERE tgname = 'trg_update_vehicle_characteristics_document_tsvector'
        AND tgrelid = 'parsed_advertisements_module.vehicle_characteristics'::regclass
    ) THEN
CREATE TRIGGER trg_update_vehicle_characteristics_document_tsvector
    BEFORE INSERT OR
UPDATE ON parsed_advertisements_module.vehicle_characteristics
    FOR EACH ROW EXECUTE FUNCTION parsed_advertisements_module.update_text_document_tsvector();
END IF;
END
$$;

DO
$$
BEGIN
INSERT INTO parsed_advertisements_module.vehicle_characteristics(id, text)
VALUES (uuid_generate_v4(), 'Марка'),
       (uuid_generate_v4(), 'Модель'),
       (uuid_generate_v4(), 'Тип техники'),
       (uuid_generate_v4(), 'Год выпуска'),
       (uuid_generate_v4(), 'Мощность'),
       (uuid_generate_v4(), 'Объём двигателя'),
       (uuid_generate_v4(), 'Состояние'),
       (uuid_generate_v4(), 'Крутящий момент'),
       (uuid_generate_v4(), 'Емкость топливного бака'),
       (uuid_generate_v4(), 'Грузоподъемность'),
       (uuid_generate_v4(), 'Масса'),
       (uuid_generate_v4(), 'Длина рамы'),
       (uuid_generate_v4(), 'Колесная база'),
       (uuid_generate_v4(), 'Двигaтeль'),
       (uuid_generate_v4(), 'Манипулятoр'),
       (uuid_generate_v4(), 'Xapвестернвый aгрегат'),
       (uuid_generate_v4(), 'Koличествo колес'),
       (uuid_generate_v4(), 'Paзмeр пеpeдниx кoлec'),
       (uuid_generate_v4(), 'Размeр зaдниx колec'),
       (uuid_generate_v4(), 'Ocтаток рeзины'),
       (uuid_generate_v4(), 'Наработка'),
       (uuid_generate_v4(), 'Трансмиссия'),
       (uuid_generate_v4(), 'Рабочий объем'),
       (uuid_generate_v4(), 'Эксплуатационная масса'),
       (uuid_generate_v4(), 'Мощность двигателя'),
       (uuid_generate_v4(), 'Макс. крутящий момент'),
       (uuid_generate_v4(), 'Рабочий объем'),
       (uuid_generate_v4(), 'Скорость передвижения'),
       (uuid_generate_v4(), 'Hарaбoткa'),
       (uuid_generate_v4(), 'Высота выгрузки'),
       (uuid_generate_v4(), 'Емкость топливного бака'),
       (uuid_generate_v4(), 'Макс. высота подъёма'),
       (uuid_generate_v4(), 'Центр тяжести'),
       (uuid_generate_v4(), 'Ходовая часть'),
       (uuid_generate_v4(), 'Моточасы'),
       (uuid_generate_v4(), 'Двигатель') 
        ON CONFLICT(text) DO NOTHING;
END $$;
