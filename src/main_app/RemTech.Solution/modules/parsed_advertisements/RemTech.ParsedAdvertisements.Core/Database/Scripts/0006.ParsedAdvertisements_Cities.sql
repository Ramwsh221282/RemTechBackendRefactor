CREATE TABLE IF NOT EXISTS parsed_advertisements_module.cities(
    id                      UUID PRIMARY KEY,
    region_id               UUID NOT NULL REFERENCES parsed_advertisements_module.geos(id) ON DELETE CASCADE,
    text                    VARCHAR(50),
    document_tsvector       TSVECTOR,
    UNIQUE (region_id, text)
);

CREATE INDEX IF NOT EXISTS idx_cities_tsvector
    ON parsed_advertisements_module.cities USING GIN(document_tsvector);

CREATE INDEX IF NOT EXISTS idx_cities_text
    ON parsed_advertisements_module.cities(text);

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
        WHERE tgname = 'trg_update_cities_document_tsvector'
        AND tgrelid = 'parsed_advertisements_module.cities'::regclass
    ) THEN
CREATE TRIGGER trg_update_cities_document_tsvector
    BEFORE INSERT OR UPDATE ON parsed_advertisements_module.cities
                         FOR EACH ROW EXECUTE FUNCTION parsed_advertisements_module.update_text_document_tsvector();
END IF;
END
$$;


DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Алтайский') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Барнаул'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Новоалтайск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Заринск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Алейск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Бийск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Рубцовск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Змеиногорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Камень-на-Оби'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Горняк'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Славгород'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Яровое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Белокуриха'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'ЗАТО Сибирский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Алейский район'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Калманка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Баево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Волчиха'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Чарышское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Кулунда'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Алтайское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Новогорьевское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Мамонтово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Куяган'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Павловск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Тальменка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Курья'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Бурла'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Красногорское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Романово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Кытманово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Крутиха'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Зональное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Михайловское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Южный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Поспелиха'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Солтон'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Ая'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Акутиха'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Александровский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Куяча'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Амурский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Пещерка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Стан-Бехтемир'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Яново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Старобелокуриха'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Гальбштадт'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Асямовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Ананьевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Староалейское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Афонино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтайский'), 'Целинный')
        ON CONFLICT (region_id, text) DO NOTHING;
ELSE
        RAISE NOTICE 'Регион "Алтайский" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Амурская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Благовещенск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Сковородино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Райчихинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Зея'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Шимановск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Белогорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Свободный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Тында'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Цилковский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Серышево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Тамбовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Ромны'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Новокиевский Увал'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Ивановка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Поярково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Константиновка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Екатеринославка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Возжаевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Чигири'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Пригородное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Белоусовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Ближний Сахалин'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Нижние Бузули'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Базисное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Безымянное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Белоцерковка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Богородское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Богословка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Белый Яр'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Бысса'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Белоногово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Братолюбовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Безозерное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Болдыревка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Бибиково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Большеозерка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Борисоглебка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Беляковка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Большая Сазанка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Белогорка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Бирма'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Бочкаревка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Борисполь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Бардагон'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Буссе'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Белоярово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Бичура'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Берея'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Введеновка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Аркадьевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Амурская'), 'Долдыкан')
        ON CONFLICT (region_id, text) DO NOTHING;
ELSE
        RAISE NOTICE 'Регион "Амурская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Архангельская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Архангельск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Мирный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Мезень'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Онега'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Вельск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Новодвинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Котлас'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Северодвинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Шенкурск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Няндома'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Коряжма'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Каргополь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Сольвычегодск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Октябрьский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Коноша'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Карпогоры'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Талажский Авиагородок'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Ильинско-Подомское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Лешуконское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Красноборск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Холмогоры'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Яренск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Катунино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Уемский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Рикасиха'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Сопка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Мелехово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Кулига'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Прилук'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Першинская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Борисовская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Гавриловская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Родионовская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Овсянниковская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Прилуки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Федотовская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Яковлевская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Шиловская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Климовская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Неклюдовская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Покровская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Двинской'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Кононовская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Макаровская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Заозерье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Заборье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Шангалы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Патракеевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Подволочье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Пурнема'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Архангельская'), 'Ломоносово')
        ON CONFLICT (region_id, text) DO NOTHING;
ELSE
        RAISE NOTICE 'Регион "Архангельская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Астраханская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Астрахань'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Нариманов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Ахтубинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Харабали'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Камызяк'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Знаменск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Лиман'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Красные Баррикады'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Ильинка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Верхний Баскунчак'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Нижний Баскунчак'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Волго-Каспийский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Володарский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Харабалинский район'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Икряное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Черный Яр'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Енотаевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Началовский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Сасыколи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Ахтубинский район'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Красный Яр'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Кировский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Никольское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Воленский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Тамбовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Оля'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Карагали'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Зеленга'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Болхуны'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Ушаковка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Новониколаевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Копановка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Трудфронт'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Растопуловка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Буруны'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Новые Булгары'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Соленое Займище'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Береговой'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Джурак'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Бахаревский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Чилимный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Озерное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Гусиное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Образцово-Травино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Селитренное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Хмелевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Яксатово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Старокучергановка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Мумра'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Маково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Астраханская'), 'Гандурино')
        ON CONFLICT (region_id, text) DO NOTHING;
ELSE
        RAISE NOTICE 'Регион "Астраханская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Белгородская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Белгород'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Алексеевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Шебекино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Старый Оскол'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Бирюч'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Грайворон'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Новый Оскол'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Короча'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Валуйки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Губкин'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Строитель'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Прохоровка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Волоконовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Майский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Разумное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Уразово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Пятницкое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Пролетарский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Ближняя Игуменка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Таврово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Архангельское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Маслова Пристань'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Скородное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Роговатое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Засосна'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Бехтеевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Беловское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Ливенка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Бессоновка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Великомихайловка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Головчино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Пушкарное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Троицкий'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Стрелецкое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Дубовое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Яблоново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Щетиновка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Репное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Новосадовый'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Северный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Незнамово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Новостроевка-первая'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Насоново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Октябрьский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Ястребово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Суворово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Солоти'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Солохи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Свистовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Сеймица'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Белгородская'), 'Сагайдачное')
        ON CONFLICT (region_id, text) DO NOTHING;
ELSE
        RAISE NOTICE 'Регион "Белгородская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Брянская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Брянск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Сельцо'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Стародуб'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Карачев'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Клинцы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Унеча'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Дятьково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Мглин'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Фокино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Злынка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Новозыбков'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Жуковка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Сураж'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Почеп'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Севск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Трубчевск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Дубровка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Гордеевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Белые Берега'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Ивот'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Большое Полпино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Радица-Крыловка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Алтухово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Супонево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Морачово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Лесозавод'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Овстуг'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Великая Топаль'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Савлуково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Веребск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Перелазы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Малфа'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Дивовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Лопушь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Мазнева'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Малое Полпино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Коржовка-Голубовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Норино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Отрадное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Добрик'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Немерь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Алешинка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Городец'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Беловодка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Петрова Буда'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Рябчи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Герасимовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Леденево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Немеричи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Брянская'), 'Кабиличи')
       ON CONFLICT (region_id, text) DO NOTHING;
ELSE
        RAISE NOTICE 'Регион "Брянская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Владимирская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Владимир'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Александров'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Киржач'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Камешково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Юрьев-Польский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Вязники'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Муром'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Меленки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Кольчугино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Гусь-Хрустальный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Карабаново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Петушки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Струнино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Собинка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Ковров'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Гороховец'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Курлово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Радужный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Судогда'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Лакинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Суздаль'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Покров'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Костерово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Красная Горбатка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Балакирево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Никологоры'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Мелехово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Ставрово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Вольгинский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Муромский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Малые Удолы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Большевысоково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Оборино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Рахманов Перевоз'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Кусуново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Заклязьменский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Галкино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Годуново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Фоминки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Григорьево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Андреевское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Сарыево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Сергиевы-Горки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Чеково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Шустово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Крутово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Кудрино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Колпь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Демидово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Мокрое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Владимирская'), 'Барское Татарово')
       ON CONFLICT (region_id, text) DO NOTHING;
ELSE
        RAISE NOTICE 'Регион "Владимирская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Волгоград'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Николаевск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Котельниково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Суровикино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Петров Вал'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Жирновск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Ленинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Дубовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Серафимович'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Калач-на-Дону'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Палласовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Котово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Михайловка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Волжский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Фролово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Новоаннинский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Камышин'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Краснослободск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Урюпинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Быково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Елань'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Кумылженская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Нехаевская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Алексеевская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Старая Полтавка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Линево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Медведцкий'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Подчинное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Ольховка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Горный Балыклей'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Давыдовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Луговая Пролейка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Клетская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Преображенская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Светлый Яр'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Приморск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Новый Рогачик'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Лог'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Эльтон'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Нижний Чир'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Филоновская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Букановская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Суводская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Голубинская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Верхний Балыклей'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Прямая Балка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Горная Пролейка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Пичуга'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Оленье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Горноводяное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Волгоградская'), 'Лозное')
       ON CONFLICT (region_id, text) DO NOTHING;
ELSE
        RAISE NOTICE 'Регион "Волгоградская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Вологодская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Вологда'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Бабаево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Череповец'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Грязовец'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Белозерск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Харовск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Сокол'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Красавино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Кадников'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Никольск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Тотьма'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Кириллов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Устюжна'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Великий Устюг'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Шексна'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Устье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Верховажье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Сямжа'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Волкова'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Георгиевское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Городищево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Артюшино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Антушево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Безгачиха'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Суздалиха'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Душнево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Льнозавод'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Плесо'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Кожухово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Климшин Бор'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Косиково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Андреевское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Жилкино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Сидорово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Дийково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Костеньково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Бучиха'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Малое Борисово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Овсянниково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Андреевское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Жилкино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Сидорово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Дийково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Костеньково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Бучиха'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Малое Борисово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Овсянниково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Заборье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Тереховая'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Волково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Крюково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Миньково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Ляменьга'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Козлец'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Подберезка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Новосерково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Харчевня'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Вологодская'), 'Карасово')
       ON CONFLICT (region_id, text) DO NOTHING;
ELSE
        RAISE NOTICE 'Регион "Вологодская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Воронежская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Воронеж'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Бобров'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Калач'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Лиски'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Россошь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Острогожск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Нововоронеж'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Новохоперск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Павловск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Поворино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Эртиль'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Борисоглебск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Богучар'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Бутурлиновка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Семилуки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Воробьевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Рельевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Верхний Мамон'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Верхняя Хава'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Новохоперский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Перелешинский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Нижний Кисляй'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Таловая'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Гремячье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Новая Усмань'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Коротояк'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Сторожевое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Белогорье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Щучье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Каширское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Колодезный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Темирязево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Болдыревка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Краснолипье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Богана'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Макашевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Нижнее Турово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Пчелиновка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Коденцово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Гороховка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Крыловка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Парусное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Алферовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Макарье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Аношкино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Подосиновка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Нащекино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Колодежное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Караяшник'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Рождественская Хава'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Воронежская'), 'Клеповка')
       ON CONFLICT (region_id, text) DO NOTHING;
ELSE
        RAISE NOTICE 'Регион "Воронежская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Еврейская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Биробиджан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Облучье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Николаевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Приамурский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Теплоозерское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Известковый'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Бираканское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Лондоко'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Хинганск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Ленинское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Птичник'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Биджан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Надеждинское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Валдгеймское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Помпеевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Сутара'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Унгун'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Озерное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Дежневка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Бирофельд'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Партизанское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Село Камышовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Пашково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Целинное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Опытное Поле'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Казанка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Будукан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Двуречье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Белгородское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Нагибово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Заречное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Башурово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Ручейки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Снарский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Рудное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Ленинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Абрамовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Полевое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Кимкан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Союзное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Доброе'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Ключевое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Аур'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Лазарево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Кирга'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Ударный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Соловьевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Красный Восток'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Красивое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Дубовое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Еврейская'), 'Пронькино')
       ON CONFLICT (region_id, text) DO NOTHING;
ELSE
        RAISE NOTICE 'Регион "Еврейская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Ивановская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Иваново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Шуя'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Вичуга'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Кинешма'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Кохма'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Тейково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Родники'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Гаврилов Посад'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Приволжск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Фурманов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Пучеж'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Южа'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Заволжск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Юрьевец'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Наволоки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Комсомольск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Плёс'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Лежнево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Палех'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Ильинское-Хованское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Лух'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Старая Вичуга'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Пестяки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Колобово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Новописцово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Каменка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Ново-Талицы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Петровский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Писцово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Коптево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Васильевское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Михалево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Богданиха'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Талицы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Ингарь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Панфилово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Богородское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Хромцово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Новое Горяново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Дуляпино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Каминский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Майдаково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Новое Левушино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Чернореченское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Китово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Новые Горки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Аньково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Подвязновский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Шилыково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Липовая Роща'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ивановская'), 'Мыт')
       ON CONFLICT (region_id, text) DO NOTHING;
ELSE
        RAISE NOTICE 'Регион "Ивановская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Иркутская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Иркутск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Братск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Нижнеудинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Усолье-Сибирское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Усть-Кут'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Усть-Илимск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Ангарск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Тулун'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Саянск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Свирск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Железногорск-Илимский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Шелехов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Тайшет'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Киренск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Байкальск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Слюдянка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Черемхово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Алзамай'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Вихоревка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Бирюсинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Бирюсинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Бодайбо'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Зима'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Залари'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Белореченский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Усть-Ордынский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Качуг'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Верхоленск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Тайтурка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Квиток'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Артемовский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Видим'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Юрты'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Янталь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Листвянка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Лесогорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Мишелёвка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Култук'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Мамакан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Луговский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Кропоткин'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Грановщина'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Баклаши'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Еланцы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Тыреть'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Хомутово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Большой Луг'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Онот'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Бутырки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Урик'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Тайтура'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Иркутская'), 'Максимовщина')
       ON CONFLICT (region_id, text) DO NOTHING;
ELSE
        RAISE NOTICE 'Регион "Иркутская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Нальчик'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Тырныауз'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Прохладный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Майский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Нартакала'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Баксан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Терек'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Чегем'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Шалушка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Нартан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Исламей'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Заюково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Чегем'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Залукокоаже'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Анзорей'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Хасанья'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Белая Речка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Жанхотеко'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Бабугент'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Малакановское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Озрек'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Каменномостское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Сармаково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Лашкута'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Инаркой'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Хабаз'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Шитхала'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Псыгансу'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Новая Балкария'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Малка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Кенделен'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Баксаненок'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Урвань'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Этоко'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Дальнее'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Ново-Полтавское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Ташлы-Тала'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Учебное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Советское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Ново-Ивановское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Октябрьское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Псыншоко'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Терекское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Белокаменское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Верхний Лескен'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Новое Химидие'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Ерокко'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Зольское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Верхний Куркужин'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кабардино-Балкарская'), 'Благовещенка')
       ON CONFLICT (region_id, text) DO NOTHING;
ELSE
        RAISE NOTICE 'Регион "Кабардино-Балкарская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Калининградская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Калининград'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Балтийск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Зеленоградск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Светлогорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Гвардейск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Черняховск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Неман'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Гусев'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Правдинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Светлый'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Ладушкин'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Пионерский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Гурьевск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Мамоново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Нестеров'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Полесск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Багратионовск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Краснознаменск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Приморск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Славск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Озерск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Советск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Янтарный городской округ'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Советск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Кёнигсберг'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Знаменск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Добровольск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Гвардейское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Коврово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Большаково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Волочаевское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Нивенское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Большое Исаково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Колосевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Исаково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Большая Поляна'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Славское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Люблино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Цветное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Каширское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Приморье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Ладыгино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Тургенево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Васильково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Черемухино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Парусное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Рожково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Августовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Лунино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Речное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калининградская'), 'Ильино')
       ON CONFLICT (region_id, text) DO NOTHING;
ELSE
        RAISE NOTICE 'Регион "Калининградская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Черкесск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Карачаевск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Усть-Джегута'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Теберда'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Зеленчукская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Преградная'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Новый Карачай'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Правокубанский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Терезе'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Чапаевское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Эркен-Шахар'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Али-Бердуковский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Кардоникская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Кумыш'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Сторожевая'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Псыж'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Первомайское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Ударный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Архыз'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Водораздельный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Малокурганный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Красногорская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Уруп'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Нижний Архыз'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Нижняя Теберда'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Койдан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Холоднородниковское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Маруха'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Дружба'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Нижняя Ермоловка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Псемен'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Подскальное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Абазакт'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Пристань'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Светлое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Таллык'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Кызыл-Покун'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Хасаут-Греческое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Садовое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Знаменка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Джага'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Родниковский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Село имени Коста Хетагурова'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Хасаут'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Ильичевское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Элькуш'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Спарта'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Красный Восток'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Кубан-халк'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Лесо-Кяфарь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карачаево-Черкесская'), 'Рим-Горский')
       ON CONFLICT (region_id, text) DO NOTHING;
ELSE
        RAISE NOTICE 'Регион "Карачаево-Черкесская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Кировская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Киров'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Кирово-Чепецк'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Слободской'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Вятские Поляны'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Мураши'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Зуевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Яранск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Кирс'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Белая Холуница'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Луза'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Малмыж'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Омутнинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Нолинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Котельнич'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Сосновское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Советск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Уржум'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Орлов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Оричи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Санчурск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Фаленки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Стрижи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Осокино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Нижнеивкино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Малая Гора'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Шкляевская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Бахта'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Мухино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Цепели'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Богородская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Большая Гора'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Истобенск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Лойно'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Югрино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Колобовщина'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Гнусино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Сергеево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Ломовская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Малая Субботиха'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Захарищевы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Садаковский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Большая Субботиха'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Балезинщина'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Иунинцы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Дороничи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Ганино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Порошино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Сидоровка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Сосновый'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Катковы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кировская'), 'Костино')
       ON CONFLICT (region_id, text) DO NOTHING;
ELSE
        RAISE NOTICE 'Регион "Кировская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Липецкая') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Липецк'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Елец'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Грязи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Лебедянь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Данков'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Усмань'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Чаплыгин'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Задонск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Доброе'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Волово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Боринское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Хлевное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Стебаево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Добринка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Становое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Крутые Хутора'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Тележенка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Введенка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Санцово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Сырское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Частая Дубрава'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Грязное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Бруслановка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Новодмитриевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Ивово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Большая Кузьминка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Сухая Лубна'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Пружинки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Вербилово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Лев Толстой'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Измалково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Кузьмино-Отвержский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Новодеревенский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Кашары'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Отскочное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Кореневщино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Яблоново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Чернава'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Трубетчино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Марьино-Николаевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Большая Поляна'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Рогожино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Шовское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Верхнедрезгалово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Домачи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Голиково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Калабино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Знаменское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Талица'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Гудаловка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Липецкая'), 'Каликино')
       ON CONFLICT (region_id, text) DO NOTHING;
ELSE
        RAISE NOTICE 'Регион "Липецкая" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Московская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Королёв'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Королев'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Химки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Балашиха'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Мытищи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Подольск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Люберцы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Клин'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Сергиев Посад'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Истра'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Павловский Посад'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Коломна'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Одинцово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Лыткарино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Воскресенск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Краснознаменск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Зарайск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Красногорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Дзержинский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Серпухов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Шатура'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Звенигород'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Электросталь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Домодедово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Ногинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Жуковский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Егорьевск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Чехов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Волоколамск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Верея'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Дубна'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Лобня'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Раменское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Видное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Пушкино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Фрязино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Озёры'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Ступино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Щёлково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Дмитров'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Лосино-Петровский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Красноармейск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Апрелевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Орехово-Зуево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Москва'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Луховицы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Долгопрудный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Реутов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Белоозёрский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Руза'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Бронницы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Московская'), 'Котельники')
       ON CONFLICT (region_id, text) DO NOTHING;
ELSE
        RAISE NOTICE 'Регион "Московская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Мурманская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Мурманск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Алатиты'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Оленегорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Кандалакша'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Североморск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Кировск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Островной'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Полярные Зори'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Гаджиево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Мончегорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Кола'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Заполярный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Полярный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Ковдор'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Заозерск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Снежногорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Никель'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Ловозеро'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Кильдинстрой'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Междуречье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Алакуртти'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Печенга'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Высокий'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Видяево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Африканда'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Териберка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Лапландия'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Магнетиты'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Раякоски'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Пялица'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Кашкаранцы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Приречный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Луостари'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Борисоглебский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Лиинахамари'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Ёна'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Пулозеро'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Пинозеро'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Минькино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Лумбовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Ретинское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Ягельный Бор'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Кайралы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Ковда'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Оленица'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Чапома'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Краснощелье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Федосеевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Тетрино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мурманская'), 'Куолоярви')
       ON CONFLICT (region_id, text) DO NOTHING;
ELSE
        RAISE NOTICE 'Регион "Мурманская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Нарьян-Мар'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Оксино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Тельвиска'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Шойна'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Андег'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Бугрино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Усть-Кара'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Коткино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Нельмин-нос'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Хорей-Вер'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Волонга'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Вижас'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Хонгурей'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Макарово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Щелино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Снопа'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Кия'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Харьягинский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Куя'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Каратайка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Пылемец'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Волоковая'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Лабожское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Варнек'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Верхняя Пеша'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Белушье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Тошвиска'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Каменка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Устье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Красное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Чёрная'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Зеленый Яр'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Тельвисочный сельсовет'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Пустозёрск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Носовая'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Хоседа-Хард'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Малоземельский сельсовет'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ненецкий'), 'Великая')
       ON CONFLICT (region_id, text) DO NOTHING;
ELSE
        RAISE NOTICE 'Регион "Ненецкий" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Нижний Новгород'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Павлово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Городец'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Дзержинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Арзамас'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Семенов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Бор'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Кстово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Балахна'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Чкаловск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Шахунья'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Перевоз'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Выкса'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Сергач'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Сергач'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Лукоянов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Лысково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Урень'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Богородск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Навашино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Первомайск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Княгинино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Заволжье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Володарск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Саров'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Горбатов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Кулебаки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Ветлуга'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Ворсма'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Ардатов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Шатки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Ковернино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Сосновское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Пильна'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Досчатое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Вахтан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Шиморское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Желнино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Сокольское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Красные Баки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Фролищи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Ильиногорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Большое Козино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Малое Козино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Починки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Большое Болдино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Сеченово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Ближнеконстантиново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Сатис'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Волосово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Новинки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Нижегородская'), 'Усленье')
       ON CONFLICT (region_id, text) DO NOTHING;
ELSE
        RAISE NOTICE 'Регион "Нижегородская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Новгородская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Великий Новгород'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Боровичи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Сольцы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Пестово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Окуловка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Старая Русса'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Малая Вишера'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Чудово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Холм'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Валдай'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Демянск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Крестцы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Хвойная'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Волот'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Батецкое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Поддорье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Мошенское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Неболчское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Угловка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Кулотино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Лычково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Сопины'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Городцы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Яжелбицы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Будрино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Большие Ясковицы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Березник'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Коегоща'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Старое Рахино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Теребони'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Березицы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Тесово-Нетыльский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Грузино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Кулаково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Нароново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Жарки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Завал'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Мясной Бор'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Приволье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Новоселицы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Гряды'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Люболяды'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Подберезье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Ручьи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Городня'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Сутоки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Чавницы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Глебово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Коростынь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Русское Пестово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новгородская'), 'Лужки')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Новгородская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Новосибирск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Каргат'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Чулым'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Обь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Болотное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Барабинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Барабинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Татарск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Карасук'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Черепаново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Купино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Тогучин'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Куйбышев'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Искитим'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Бердск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Кольцово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Маслянино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Коченево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Мошкова'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Краснозерское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Линево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Краснообск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Сузун'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Колывань'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Дорогино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Довольное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Здвинский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Венгерово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Северное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Баган'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Станционно-Ояшинский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Усть-Тарка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Кудряшовский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Сибирский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Кыштовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Убинское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Бергуль'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Красный Восток'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Барабанский район'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Плотниково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Болотнинский район'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Барлакский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Боровое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Барабо-Юдино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Гербаево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Рогалево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Искитимский район'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Верх-Тула'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Каинская Заимка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Новоспасск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Филошенка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Новосибирская'), 'Чупино')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Новосибирская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Омская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Омск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Калачинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Тара'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Искилькуль'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Тюкалинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Называевск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Черлак'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Таврическое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Нововаршавка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Саргатское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Полтавка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Шербакуль'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Муромцево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Павлоградка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Большеречье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Оконешниково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Прииртышье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Одесское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Куломзино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Чулино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Кошкарево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Становка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Щегловка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Иванов Мыс'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Эбаргуль'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Евгащино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Нижняя Омка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Колосовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Руслановка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Сереброполье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Тамбовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Аксеново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Красноярское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Марьяновка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Усть-Ишим'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Лаврино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Кормиловка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Бергамак'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Ишмегал'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Знаменское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Большой Атмас'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Золотая Нива'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Красовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Атрачи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Аёв'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Бузан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Рязаны'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Бобринка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Большая Бича'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Низовое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Омская'), 'Карбыза')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Омская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Оренбург'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Орск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Медногорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Гай'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Абдулино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Ясный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Новотроицк'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Сорочинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Бугуруслан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Кувандык'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Бузулук'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Соль-Илецк'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Асекеево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Новосергиевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Саракташ'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Илек'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Новоорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Каргала'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Краснохолм'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Нижнесакмарский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Переволоцкий'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Самородово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Матвеевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Беляевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Курманаевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Грачевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Татищево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Плешаново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Курманаевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Грачевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Татищево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Плешаново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Сладков'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Светлый'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Шарлык'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Нежинка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Аксаково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Первомайский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Комаровский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Баймурат'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Теплое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Лесопитомник'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Хабарное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Среднеильясово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Зарево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Гостеприимный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Новопотоцк'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Старобогдановка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Мазуровка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Черноречье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Пригорное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Затонное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Булатовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Белогорский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Оренбургская'), 'Свердловский')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Оренбургская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Пензенская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Пенза'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Каменка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Городище'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Кузнецк'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Заречный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Белинский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Никольск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Сердобск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Нижний Ломов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Сурск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Спасск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Шемышейка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Грабово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Средняя Елюзань'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Лунино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Богданово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Исса'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Засечное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Тамала'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Поселки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Кондоль'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Лопатино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Малая Сердоба'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Русский Камешкир'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Александрово-ростовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Сюзюм'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Агринка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Масловка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Иваново-Наумовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Сюверня'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Мача'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Куликовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Мосолово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Большая корнеевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Обвал'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Кулясова'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Бугры'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Ермоловка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Долгоруково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Аничкино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Колбинка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Рельёвка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Усть-каремша'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Наскафтым'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Кикино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Головинщина'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Татарская канадей'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Вышелей'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Засурское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пензенская'), 'Китунькино')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Пензенская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Пермский') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Пермь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Гремячинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Соликамск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Березники'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Оса'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Лысьва'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Очер'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Губаха'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Чернушка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Краснокамск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Кунгур'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Добрянка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Нытва'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Усолье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Красновишерск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Чайковский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Казел'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Горнозаводск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Верещагино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Кудымкар'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Чердынь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Чермоз'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Оханск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Сёла'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Углеуральский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Скальный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Лямино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Калино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Теплая гора'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Сараны'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Промысла'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Кусье-Александровский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Пашия'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Старый Бисер'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Звездный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Нововильвенский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Парма'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Медведкинское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Сергино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Малая Соснова'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Голубята'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Липово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Альняш'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Дуброво'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Мысы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Челва'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Казаево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Мульково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермский'), 'Барда')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Пермский" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Псковская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Псков'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Великие Луки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Пустошка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Невель'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Опочка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Себеж'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Дно'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Новосокольники'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Остров'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Новоржев'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Пыталово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Гдов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Печоры'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Порхов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Усвяты'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Палкино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Кунья'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Долосцы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Хотицы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Щукино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Лавры'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Гультяи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Зубово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Верхний Мост'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Попадинка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Забелье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Крупп'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Новый Изборск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Славковичи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Рябово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Богданово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Назимово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Голубово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Подосьё'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Ершово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Карамышево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Логозовичи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Теребище'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Чудинково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Андрюшино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Нижние Галковичи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Конёчек'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Вёшки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Тавлово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Дубоновичи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Псковская'), 'Зайцево')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Псковская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Адыгея') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Адыгейск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Майкол'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Гиагинская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Тлюстенхабль'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Красногвардейское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Тульский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Тахтамукай'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Понежукай'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Кошехабль'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Хакуринохабль'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Краснооктябрьский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Хатукайское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Подгорный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Гавердовский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Гатлукай'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Курджипская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Старобежгокай'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Безводная'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Безводная'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Родниковый'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Подгорный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Натырбово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Даховская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Адамий'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Псекупс'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Шундук'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Днепровский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Новосевастопольское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Сергиевское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Георгиевское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Задунаевский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Политотдел'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Махошеполяна'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Фарсовский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Вольное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Образцовое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Чумаков'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Бжедугхабль'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Севастопольская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Игнатьевский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Нешукай'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Начерезий'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Черемушкин'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Саратовский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Новобжегокай'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Пустоселов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Адыгея'), 'Вочепший')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Адыгея" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Бурятия') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Гусиноозерск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Улан-Удэ'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Бабушкин'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Кяхта'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Северобайкальск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Закаменск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Баргузин'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Хоринск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Петропавловка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Илька'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Турунтаево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Кабанск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Наушки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Курумкан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Нижний Саянтуй'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Сужа'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Новоильинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Новая Брянь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Ильинка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Хойтобэе'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Гусиное озеро'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Саган-Нур'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Выдрино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Гремячинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Максимиха'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Горячинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Тунка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Романовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Макаринино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Харацай'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Улентуй'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Холтосон'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Ара-киреть'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Утата'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Узкий Луг'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Ташир'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Елань'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Вознесеновка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Санномыск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Торы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Каленово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Чикой'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Ганзурино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Подлопатки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Загустай'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Бурятия'), 'Дырестуй')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Бурятия" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Карелия') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Петрозаводск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Кемь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Суоярви'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Сортавала'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Олонец'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Беломорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Медвежьегорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Пудож'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Питкяранта'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Кондопога'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Лахденпохья'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Костомукша'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Сегежа'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Лоухи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Калевала'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Пяозерский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Великая Губа'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Ладва'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Марциальные Воды'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Полга'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Кестеньга'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Шуньга'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Шокша'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Машинсельга'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Ангенлахта'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Вешкелица'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Типиницы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Хаутаваара'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Сямозеро'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Ажепнаволок'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Евгора'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Ведлозеро'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Линдозеро'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Рыбрека'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Мегрега'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Рубчойла'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Каскесручей'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Юшкозеро'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Шуезеро'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Корза'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Кинерма'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Березовая Гора'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Шуерецкое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Канзанаволок'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Данилово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Авнепорог'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Толвуйское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Импилахтинское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Карелия'), 'Батова')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Карелия" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Мордовия') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Саранск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Темников'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Рузаевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Ковылкино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Ардатов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Краснослободск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Инсар'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Тургенево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Умет'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Комсомольский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Луховка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Ялга'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Явас'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Потьма'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Колопино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Лямбирь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Ромоданово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Ельники'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Старое Шайгово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Ельники'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Теньгушево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Мазилуг'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Большие Березники'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Подлесная Тавла'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Смольный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Вадовские Селищи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Дегилёвка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Баево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Каласево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Жаренки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Кельвядни'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Чукалы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Урусово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Кученяево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Пиксяси'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Дмитриев Усад'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Сабаево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Русские Дубровки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Капасово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Казенный Майдан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Вольно-Никольское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Большие Манадыши'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Киржеманы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Адашево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Старый Верхиссы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Парадеево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Сиалеевская Пятина'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Новлей'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Нижняя Вязера'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Верхняя Лухма'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Мордовия'), 'Тарханово')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Мордовия" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Саха') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Якутск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Алдан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Среднеколымск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Ленск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Вилюйск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Покровск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Мирный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Нерюнгри'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Олекминск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Верхоянск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Нюрба'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Удачный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Томмот'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Сангар'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Депутатский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Сыскылах'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Ытык-Кюель'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Сунтар'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Тикси'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Батагай'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Мохсоголлох'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Белая Гора'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Черский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Усть-Куйга'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Чурапча'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Намцы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Оймякон'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Жиганск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Серебряный Бор'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Беркакит'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Нижнеянск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Алмазный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Эсэ-Хайя'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Ленинский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Кутана'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Угоян'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Верхний Куранах'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Кильдямцы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Хатассы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Старая Табага'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Табага'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Амга'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Ыллымах'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Куберганя'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Сыаганнах'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Сутуруоха'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Акана'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Айхал'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Юрюнг-кюель'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Юрюнг-хая'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саха'), 'Ючюгей')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Саха" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Владикавказ'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Дигора'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Алагир'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Ардон'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Моздок'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Беслан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Ногир'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Заводской'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Чермен'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Гизель'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Михайловское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Майский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Сунжа'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Кизляр'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Даргавс'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Эльхотово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Камбилеевское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Луковская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Павлодопьская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Змейская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Махческ'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Майрамадаг'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Моздокский район'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Чми'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Лескен'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Хазнидон'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Дзуарикау'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Бекан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Нар'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Ир'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Верхняя Саниба'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Какадур'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Ламардон'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Фазикау'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Нижняя Кани'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Кобан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Кора-Урсдон'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Алханчурт'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Джимара'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Саниба'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Тменикау'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Нижняя Саниба'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Тарское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Куртат'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Комгарон'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Донгарон'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Хидикус'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Кармадон'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Верхний Бирагзанг'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Дзивгис'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Брут')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Северная Осетия" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Рязанская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Рязань'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Касимов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Кораблино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Ряжск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Михайлов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Рыбное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Сасово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Сколин'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Новомиучирнск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Спасск-Рязанский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Шацк'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Спас-Клепики'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Милославское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Сараи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Тума'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Шилово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Пителино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Путянино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Захарово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Горняк'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Старожилово Ухолово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Гусь-Железный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Павелец'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Побединка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Поплевинский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Сынтул'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Поляны'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Чучково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Заборье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Дубровичи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Казарь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Заокское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Дядьково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Приозерный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Льгово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Шумашь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Коростово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Подвязье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Букрино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Гавердово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Екимовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Вышетравино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Пальное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Алеканово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Борисково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Вышгород'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Агро-Пустынь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Болошнево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Рязанская'), 'Тюшево')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Рязанская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Самарская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Рязань'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Тольятти'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Жигулёвск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Отрадный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Кинель'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Нефтегорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Нефтегорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Октябрьск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Новокуйбышевск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Сызрань'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Чапаевск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Похвистнево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Смышляевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Новосемейкино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Винновка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Осинки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Стройкерамика'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Суходол'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Усть-Кинельский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Междуреченск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Нижнее Санчелеево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Старая Бинарадка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Рождествено'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Волжский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Лопатино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Жигули'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Сенькино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Сырейка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Чубовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Подвалье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Узюково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Бейдеряково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Ташла'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Кармалы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Зеленовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Мордово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Брусяны'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Новая Бинарадка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Валы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Спиридоновка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Ветлянка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Новое Еремкино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Березовый Солонец'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Хрящевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Бузаевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Сосновый Солонец'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Жемковка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Русская Борковка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Тимофеевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Аскулы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Печерские выселки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Самарская'), 'Надеждино')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Самарская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Саратовская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Саратов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Энгельс'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Калининск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Новоузенск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Вольск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Ершов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Красноармейск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Петровск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Балаково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Хвалынск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Маркс'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Балашов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Аркадак'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Пугачев'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Аткарск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Красный Кут'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Ртищево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Степное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Ровное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Каменский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Соколовый'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Советское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Мокроус'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Светлый'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Александров-Гай'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Пинеровка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Свенной'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Михайловский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Богородское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Ивантеевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Багаевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Питерка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Сторожевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Сбродовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Сельхозтехника'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Бутурлинка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Усть-Золиха'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Безымянное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Большая Журавка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Талалихино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Бобровка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Баклуши'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Кряжим'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Барановка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Большой Кушум'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Большая Тавложка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Беленка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Белая Гора'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Саратовская'), 'Тепловка')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Саратовская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Южно-Сахалинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Долинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Оха'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Холмск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Макаров'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Томари'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Анива'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Поронайск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Невельск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Корсаков'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Александровск-Сахалинский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Углегорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Северо-Курильск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Тымовское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Курильск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Чехов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Горнозаводск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Шахтерск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Красногорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Лесогорское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Стародубское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Рыбновск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Таранай'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Москальво'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Бошняково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Леонидово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Яблочное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Бамбучки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Горячие Ключи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Пензенское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Чалаево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Дальнее'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Сокол'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Троицкое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Рощино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Победино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Охотское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Березняки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Старорусское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Синегорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Шебунино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Соловьевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Озерское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Муравьево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Ныш'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Сахалинская'), 'Арсентьевка')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Сахалинская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Свердловская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Екатеренбург'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Первоуральск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Каменск-Уральский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Карпинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Верхняя Пышма'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Заречный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Ревда'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Нижний Тагил'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Серов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Березовский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Кушва'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Краснотурьинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Полевской'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Асбест'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Ирбит'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Алапаевск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Красноуфимск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Качканар'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Реж'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Нижняя Салда'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Новоуральск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Дегтярск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Среднеуральск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Североуральск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Тавда'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Кировград'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Лесной'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Арамиль'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Красноуральск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Нижняя Тура'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Ивдель'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Верхний Тагил'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Сухой Лог'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Волчанск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Богданович'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Сысерть'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Верхняя Тура'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Артемовский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Туринск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Невьянск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Верхняя Салда'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Новая Ляля'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Камышлов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Нижние Серги'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Михайловск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Верхотурье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Талица'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Уральский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Байкалово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Свободный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Свердловская'), 'Староуткинск')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Свердловская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Смоленская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Смоленск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Ельня'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Рудня'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Починок'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Сычёвка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Демидов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Духовщина'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Сафоново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Гагарин'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Дорогобуж'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Велиж'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Вязьма'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Десногорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Ярцево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Рославль'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Монастырщина'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Богородицкое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Село'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Новомихайловское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Карповка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Андрейково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Новое Село'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Егоровка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Заборье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Жеруны'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Бизюково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Любавичи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Исаково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Ефремово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Лобково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Рябцево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Максимово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Сергеевское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Заболонье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Надва'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Белый Холм'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Ольша'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Тетери'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Родькова'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Клушино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Вяземский район'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Гусино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Третьяково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Титовщина'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Мерлино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Добромино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Тюшино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Новомихайловское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Смоленская'), 'Малеево')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Смоленская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Ставрополь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Кисловодск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Пятигорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Изобильный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Невинномысск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Минеральные Воды'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Благородный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Георгиевск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Светлоград'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Михайловск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Новоалександровск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Ессентуки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Ипатово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Железноводск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Лермонтов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Зеленокумск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Буденновск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Нефтекумск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Новопавловск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Кочубеевское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Александровское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Краснокумское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Красногвардейское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Левокумское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Бешпагир'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Красный Ключ'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Рыздвяный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Сольнечнодольск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Иноземцево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Новоселицкое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Арзгир'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Грачевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Степное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Ладовская Балка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Суворовская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Эдиссия'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Молочный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Шпаковский район'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Заря'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Красный Пахарь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Терновский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Благодатное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Константиновка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Турксад'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Северное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Красный Кундул'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Нижний Барханчак'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Горнозаводское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Марьины Колодцы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Греческое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ставропольский'), 'Пшеничный')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Ставропольский" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Тамбов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Кирсанов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Моршанск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Мичуринск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Котовск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Рассказово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Жердевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Уварово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Сосновский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Инжавино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Мучкапский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Первомайский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Токаревка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Мордово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Ржакса'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Новая Ляда'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Новопокровка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Гавриловка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Петровское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Сатинка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Бокино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Староюрьево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Волчки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Митрополье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Пичаево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Заворонежское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Трегуляй'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Борисовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Жидиловка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Саюкино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Заводской'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Красный Куст'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Татарщино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Сатино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Мельгуны'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Кершинские Борки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Красивое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Старое Хмелевое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Крюковка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Остролучье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Хотобец-Васильевское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Новоархангельское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Котовское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Новоспасское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Изосимовский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Турмасово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тамбовская'), 'Новосеславино')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Тамбовская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Тверская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Тверь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Бежецк'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Вышний Волочёк'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Андреаполь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Ржев'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Зубцов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Осташков'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Кувшиново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Белый'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Торопец'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Весьегонск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Западная Двина'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Красный Холм'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Лихославль'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Кимры'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Кашин'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Старица'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Удомля'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Конаково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Нелидово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Калязин'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Рамешки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Озёрный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Спирово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Молоково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Кесова Гора'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Максатиха'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Юрьевское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Лесное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Васильево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Фомино-Городище'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Жданово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Медведево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Солнечный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Борисково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Лядины'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Торжок'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Коптево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Павлушкино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Кошелево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Луковниково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Денежное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Васильевское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Коровкино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Власьево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Муравьево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Митино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Коньково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Осипово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тверская'), 'Загорье')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Тверская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Тюменская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Тюмень'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Тобольск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Ишим'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Ялуторовск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Заводоуковск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Омутинское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Ярково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Викулово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Исетское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Нижняя Тавда'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Юргинское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Большое Сорокино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Сладково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Казанское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Уват'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Боровский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Упорово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Каскара'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Ембаево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Вагай'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Богандинский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Винзили'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Верхние Аремзяны'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Абалак'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Водолазово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Кротово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Заводопетровское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Малая Зоркальцева'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Карасуль'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Усть-Ламенка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Слободчики'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Вознесенка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Созоново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Новоатьялово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Раздолье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Горнослинкина'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Ситниково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Балаганы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Ашлык'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Ушарова'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Кулаковское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Савина'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Смоленка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Конево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Афонькино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Новопокровка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Суерка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Аксурка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Одина'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Скородум'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тюменская'), 'Шевырино')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Тюменская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Ижевск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Сарапул'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Глазов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Воткинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Камбарка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Можга'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Красногорское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Сюмси'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Балезино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Алнаши'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Дебесы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Киясово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Шаркан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Хохряки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Июльское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Зура'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Пугачево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Селты'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Италмас'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Ягул'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Пихтовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Якшур-Бодья'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Лудорвай'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Ромашкино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Яжбахтино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Хорохоры'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Кокман'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Чуялуд'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Большой Унтем'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Малый Лудошур'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Кабалуд'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Замостные Какси'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Нижние Адам-Учи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Гуляево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Рябово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Кетул'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Каменное Заделье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Лудзя-Норья'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Мишкино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Верхняя Игра'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Дроздовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Нижнее Асаново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Норья'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Каркалай'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Вятское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Кулига'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Крымская Слудка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Балдейка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Удмуртская'), 'Верхний Бемыж')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Удмуртская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Ульяновск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Новоульяновск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Барыш'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Димитровград'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Сенгилей'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Инза'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Вешкайма'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Чердаклы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Базарный Сызган'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Старая Кулатка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Карсун'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Жадовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Сурское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Тереньга'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Радищево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Старая Майна'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Мулловка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Цемзавод'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Языково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Измайлово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Игнатовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Чуфарово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Новая Майна'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Большое Нагаткино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Новая Малыкла'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Белый Ключ'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Кувшиновка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Арское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Луговое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Отрада'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Поливно'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Плодовый'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Большие Ключищи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Пригородный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Шиловка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Кузоватово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Новый Дол'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Крутояр'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Мордово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Шлемасс'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Ясашное Помряскино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Адоевщина'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Кирзять'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Черненово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Красная Поляна'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Тепловка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Чирково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Ерыклинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Старый Белый Яр'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ульяновская'), 'Авдотьино')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Ульяновская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Хабаровск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Амурск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Николаевск-на-Амуре'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Бикин'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Вяземский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Комсомольск-на-Амуре'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Советская Гавань'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Охотск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Переяславка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Ванино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Солнечный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Согдинское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Князе-Волоконское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Богородское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Троицкое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Бичевая'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Мухен'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Эльбан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Обор'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Корфовский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Лососина'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Высокогорный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Многовершинный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Посёлок Суклай'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Селехино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Мирное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Таёжное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Невельское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Красносельское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Краснознаменка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Кедрово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Наумовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Уктур'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Ровное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Хурба'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Тырма'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Ильинка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Иня'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Иннокентьевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Ракитное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Иннокентьевский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Известковый'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Монгохто'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Березовый'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Горин'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хабаровский'), 'Омми')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Хабаровский" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Красноярский') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Красноярск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Зеленогорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Железногорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Сосновоборск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Бородино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Шарыпово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Назарово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Боготоп'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Лесосибирск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Дивногорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Норильск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Енисейск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Минусинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Ачинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Заозерный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Канск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Кодинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Игарка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Ужур'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Иланский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Дудинка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Уяр'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Артемовск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Подгорный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Горячегорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Овсянка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Пировское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Тюхтет'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Ораки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Косые Ложки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Додоново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Гляден'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Двинка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Шагирислам'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Рубино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Новоалтатка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Белогорка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Безручейка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Шушь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Ажинское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Пузаново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Хохловка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Косонголь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Сартачуль'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Базыр'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Чистый Ручей'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Инголь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Романовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Юферовское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Усть-Чульск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Красноярский'), 'Черкасск')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Красноярский" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Краснодар'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Новороссийск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Крымск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Абинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Лабинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Кореновск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Темрюк'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Апшеронск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Тимашевск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Усть-Лабинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Белореченск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Ейск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Тихорецк'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Новокубанск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Курганинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Анапа'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Армавир'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Сочи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Гулькевичи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Кропоткин'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Геленджик'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Хадыженск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Горячий Ключ'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Мостовской'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Гирей'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Ленинградская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Тбилисская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Джубгское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Новопокровская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Ковалевское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Красносельское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Братковское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Бейсугское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Мысхако'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Небуг'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Кабардинка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Краснодарский'), 'Криница')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Краснодарский" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Ханты-Мансийск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Урай'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Югорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Нягань'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Советский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Лангепас'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Когалым'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Мегион'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Белоярский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Сургут'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Радужный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Нижнеавартовск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Лянтор'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Пыть-Ях'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Покачи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Нефтеюганск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Берёзовский район'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Березово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Куминский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Луговой'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Таежный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Игрим'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Кондинское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Покур'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Большой Камень'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Алтай'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Батово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Ямки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Полноват'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Большой Ларьяк'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Карым'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Ломбовож'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Зенково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Охтеурье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Долгое Плесо'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Реполово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Ларьяк'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Тугияны'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Малоюганский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Елизарово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Шаим'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Большой Атлым'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Корлики'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Шеркалы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Болчары'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Нялина'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Малый Атлым'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Верхне-Мысовая'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Саранпауль')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Ханты-Мансийский" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Челябинская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Челябинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Аша'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Куса'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Коркино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Копейск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Каштым'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Миасс'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Сатка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Озёрск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Нязепетровск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Верхнеуральск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Картапы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Карталы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Златоуст'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Троицк'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Верхний Уфалей'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Карабаш'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Еманжелинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Бакал'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Усть-Катав'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Сим'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Пласт'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Снежинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Магнитогорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Катав-Ивановск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Катав-Ивановск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Чебаркуль'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Касли'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Южноуральск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Трехгорный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Юрюзань'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Миньярское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Локомотивный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Увельский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Фершампенуаз'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Миасское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Уйское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Еманжелинка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Межозерный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Кропачево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Кизильское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Метлино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Кочкарь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Верхняя Санарка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Новогородный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Красный Партизан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Канашево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Арасланово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Первомайский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Казанцево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Полетаево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Челябинская'), 'Коелга')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Челябинская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Чеченская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Грозный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Шали'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Урус-Мартан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Ачхой-Мартан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Аргун'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Гудермес'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Курчалой'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Наурская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Серноводск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Ойсхара'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Шелковская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Алхан-Кала'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Мескер-Юрт'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Знаменское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Шатой'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Новые-Атаги'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Шарой'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Гехи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Майртуп'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Автуры'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Цоцин-Юрт'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Бачи-Юрт'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Макажой'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Гойты'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Старые Атаги'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Герменчук'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Аллерой'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Надтеречное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Борзой'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Ассиновская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Ведено'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Саясан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Шалажи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Червленная'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Старощедринская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Чири-Юрт'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Старогладовская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Толстой-Юрт'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Химой'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Шаами-Юрт'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Старогладовская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Толстой-Юрт'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Хиимой'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Шаами-Юрт'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Дарго'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Нагорное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Побединское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Подгорное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Пионерское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Беной-Ведено'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Правобережное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Бердакел'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Виноградное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чеченская'), 'Левобережное')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Чеченская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Чукотский') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чукотский'), 'Анадырь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чукотский'), 'Певек'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чукотский'), 'Билибино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чукотский'), 'Эгвекинот'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чукотский'), 'Беринговский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чукотский'), 'Мыс Шмидта'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чукотский'), 'Уэлен'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чукотский'), 'Амгуэма'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чукотский'), 'Марково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чукотский'), 'Уэлькаль'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чукотский'), 'Рыткучи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чукотский'), 'Ламутское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чукотский'), 'Снежное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чукотский'), 'Хатырка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чукотский'), 'Усть-Белая'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чукотский'), 'Энмелен'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чукотский'), 'Новое Чаплино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чукотский'), 'Нунлигран'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чукотский'), 'Янракыннот'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чукотский'), 'Сиреники'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чукотский'), 'Конергино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чукотский'), 'Нутэпэльмен'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чукотский'), 'Ванкарем'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чукотский'), 'Тавайваам'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чукотский'), 'Берёзово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чукотский'), 'Тамватней'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чукотский'), 'Наукан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чукотский'), 'Звёздный')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Чукотский" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Салехард'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Муравленко'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Тарко-Сале'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Новый Уренгой'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Лабытнанги'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Надым'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Ноябрьск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Губкинский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Мужи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Ханымей'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Ныда'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Панаевск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Халясавэй'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Ратта'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Антипаюта'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Сеяха'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Правохеттинский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Приозерный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Ягельный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Лонгъюган'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Пуровск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Белоярск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Толька'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Зеленый Яр'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Усть-Войкар'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Овгорт'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Харсаим'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Горнокнязевск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Товопогол'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Тадебя-Яха'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Юрибей'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Хошгорт'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Вылпосл'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Шурышкары'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Харампур'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Находка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Кутопьюганское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Пельвож'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Каменка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Красное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Чёрная'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ямало-Ненецкий'), 'Старый Надым')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Ямало-Ненецкий" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Ярославская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Ярославль'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Любим'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Углич'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Данилов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Рыбинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Пошехонье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Тутаев'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Гаврилов-Ям'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Мышкин'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Петровское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Ростов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Пречистое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Лесная Поляна'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Некрасовское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Щедрино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Семибратово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Константиновский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Поречье-Рыбное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Ишня'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Лютово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Щипцово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Хабарово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Бойтово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Новый Некоуз'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Бовыкино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Большое Село'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Красный Ткачи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Хозницы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Кузьминское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Андреевское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Ермаково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Сельцо'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Пестрецово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Климатино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Михайловское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Курба'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Васильевское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Глебовское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Никульское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Потопчино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Наумовское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Палутино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Трубенинское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Колосово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ярославская'), 'Каюрово')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Ярославская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Калужская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Калуга'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Балабаново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Людиново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Киров'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Обнинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Жуков'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Жиздра'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Мещовск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Кондрово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Козельск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Малоярославец'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Мосальск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Таруса'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Боровск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Юхнов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Медынь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Ермолино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Сухиничи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Кременки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Сосенский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Спас-Деменск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Рождественно'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Барятино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Воротынск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Пятовский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Перемышль'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Ульяново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Милеево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Гремячево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Стайки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Губино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Городец'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Полошково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Лазинки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Мокрое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Ферзиково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Плюсково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Подборки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Городня'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Шепелево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Маклаки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Радищево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Песочня'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Кольцово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Канищево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Кириеевское Первое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Павлиново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Дугна'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Детчино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калужская'), 'Прокшино')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Калужская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Костромская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Кострома'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Буй'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Шарья'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Галич'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Нерехта'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Нея'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Мантурово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Макарьев'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Волгореченск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Чухлома'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Солигалич'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Кологрив'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Сусанино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Судиславль'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Поназырево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Парфеньево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Ветлужский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Пыщуг'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Боговарово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Чистые Боры'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Абабково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Серково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Шувалово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Черемховица'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Чернопенье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Сущево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Сулятино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Малое Андрейково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Завражье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Пасынково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Караваево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Прибрежный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Княжево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Головино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Антипина'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Жарки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Панфилово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Бедрино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Антипино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Жарки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Панфилово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Бедрино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Кожухово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Сафоново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Прудовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Хмелевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Михайловское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Яковлево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Яснево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Фоминское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Медвежье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Костино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Костромская'), 'Ожегино')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Костромская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Курская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Курск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Железногорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Щигры'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Курчатов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Льгов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Фатеж'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Рыльск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Дмитриев-Льговский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Обоянь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Суджа'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Коренево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Глушково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Горшечное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Солнцево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Поныри'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Троицкое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Пристень'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Новокасторное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Иванино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Олымский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Медвенка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Кшенский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Прямицыно'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Касторное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Теткино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Магнитный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Черемисиново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Жигаево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Калиновка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Большое Солдатское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Отрешково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Крупец'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Уланок'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Карыж'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Сейм'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Внезапное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Букреевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Линец'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Малые Крюки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Верхнее Жданово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Игино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Верхний Любаж'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Овсянниково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Новоивановка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курская'), 'Гнездилово')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Курская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Орловская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Орёл'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Ливны'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Мценск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Дмитровск-Орловский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Малоархангельск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Новосиль'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Кромы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Верховье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Покровское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Нарышкино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Хомутово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Долгое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Шаблыкино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Колпны'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Хотынец'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Корсаково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Знаменское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Тросна'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Жилина'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Крутое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Березовец'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Косарево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Егорьевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Голянка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Мелынь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Черногрязка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Парамоново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Плоское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Морозовский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Пашково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Гранкино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Лопашино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Каменево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Большие Озерки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Стрелецкий'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Дмитровский район'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Красное Знамя'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Протасово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Ржавец'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Разбегаевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Спасское-Лутовиново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Борилово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Мужиково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Хотимль-Кузменково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Дежкино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Дросково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Большие Пруды'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Голунь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Борисоглебское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Орловская'), 'Воротынцево')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Орловская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Тульская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Тула'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Донской'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Ефремов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Узловая'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Новомосковск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Венев'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Богородицк'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Плавск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Алексин'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Щекино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Ясногорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Болохово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Кимовск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Липки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Чекалин'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Суворов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Белев'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Киреевск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Советск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Новогуровский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Славный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Теплое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Архангельское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Чернь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Волово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Куркино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Ленинский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Алешня'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Зайцево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Хрущево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Дубна'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Плеханово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Барсуки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Ломовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Спасское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Урусово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Воскресенское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Островки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Пашково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Тайдаково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Шатск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Большая Еловая'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Варфоломеево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Иевлево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Бахметьево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Николо-Гастунь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Дедилово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Кузовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Верхоупье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Бунырево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тульская'), 'Дмитриевское')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Тульская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Коми') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Сыктывкар'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Воркута'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Инта'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Ухта'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Печора'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Вуктыл'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Усинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Сосногорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Микунь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Емва'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Объячево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Визинга'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Заполярный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Корткерос'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Усогорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Междуреченск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Боровой'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Парма'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Седкыркещ'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Верхняя Максимовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Верхняя Инта'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Мульда'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Межег'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Важгорт'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Черёмуховка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Хабариха'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Коровий Ручей'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Большая Пысса'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Трусово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Окунев Нос'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Пыёлдино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Глотово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Спаспоруб'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Среднее Бугаево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Нерица'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Палауз'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Гагшор'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Усть-Уса'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Летка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Кемъяр'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Усть-Цильма'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Замежная'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Новый Бор'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Мадмас'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Куниб'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Красный Яг'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Юсьтыдор'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Кажым'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Ираёль'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Коми'), 'Серёгово')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Коми" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Кировск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Гатчина'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Шлиссельбург'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Сосновый Бор'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Выборг'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Отрадное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Подпорожье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Никольское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Волхов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Кингисепп'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Каменногорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Приозерск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Луга'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Высоцк'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Новая Ладога'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Всеволожск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Сясьстрой'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Людейное Поле'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Бокситогорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Ивангород'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Кириши'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Волосово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Мурино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Коммунар'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Светогорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Сертолово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Тихвин'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Тосно'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Сланцы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Приморск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Кудрово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Кузьмоловский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Мга'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Тельмана'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Новоселье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Большая Ижора'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Лесогорский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Старая Ладога'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Красный Бор'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Приладожский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Дубровка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Толмачёво'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Фёдоровское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Сосново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ленинградская'), 'Форносово')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Ленинградская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Дагестан') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Махачкала'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Дербент'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Кизляр'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Кизилюрт'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Буйнакск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Дагестанские Огни'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Южно-Сухоумск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Избербаш'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Каспийск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Хасавюрт'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Гуниб'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Тарки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Курах'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Гергебиль'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Коркмаскала'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Хив'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Рутул'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Касумкент'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Цуриб'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Маджалис'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Уркарах'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Кумух'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Новолакское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Вачи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Хунзах'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Тлярата'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Новокаякент'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Нижнее Казанище'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Ленинаул'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Новый Хушет'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Губден'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Султан-Янги-Юрт'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Каякент'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Ленинкент'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Аксай'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Шамхал-Термен'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Цунта'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Сулак'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Кяхулай'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Тюбе'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Мекеги'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Унцукуль'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Комсомольский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Куллар'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Шангода'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Тагиркент-Казмаляр'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Яраг-Казмаляр'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Митаги-Казмаляр'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Кафыр-Кумух'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Гоцатль Большой'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Дагестан'), 'Нюгди')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Дагестан" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Магас'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Назрань'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Малгобек'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Карабулак'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Сунжа'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Кантышево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Плиево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Верхние Ачалуки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Нижние Ачалуки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Алхаст'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Троицкая'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Барсуки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Кязи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Долаково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Яндыри'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Средние Ачалуки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Гази-Юрт'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Инарки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Таргим'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Сагопши'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Ольгети'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Даттых'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Зязиков-Юрт'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Ляжги'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Аки-Юрт'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Вежарий'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Южное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Чемульга'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Армхи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Гули'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Гайрбек-Юрт'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Галашки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Али-Юрт'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Пседах'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Алкун'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Новый Редант'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Койрах'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Аршты'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Берд-Юрт'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Хяни'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Вознесенская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Духургишт'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Эгикал'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Фуртог'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Лейми'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Мужичи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Пялинг'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Хамхи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Салги'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Цори'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ингушетия'), 'Тярш')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Ингушетия" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Элиста'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Городовиковск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Лагань'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Цаган Аман'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Троицкое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Садовое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Малые Дербеты'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Приютное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Ики-Бурул'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Кетченеры'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Большой Царын'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Комсомольский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Аршан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Ут-Сала'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Утта'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Хонч-Нур'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Найнтахн'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Ики-Чонос'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Эрмели'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Виноградное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Розенталь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Ахнуд'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Бембешево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Чапаевское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Дружное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Пушкинское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Заагин Сала'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Харба'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Шовгр Толга'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Красинское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Сладкое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Теегин Герл'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Шин Тег'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Эркетен'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Манджикины'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Босхачи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Вторые Ульдючины'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Максимовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Березовское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Буранное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Салын'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Годжур'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Доценг'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Теегин Нур'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Северное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Обильное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Соленое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Ульяновское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Калмыкия'), 'Новая Жизнь')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Калмыкия" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Алания') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Владикавказ'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Ардон'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Дигора'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Алагир'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Моздок'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Беслан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Заводской'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Ногир'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Майский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Гизель'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Михайловское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Чермен'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Сунжа'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Камбилеевское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Кизляр'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Моздокский район'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Эльхотово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Луковская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Павлодольская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Змейская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Нар'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Даргавс'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Куртат'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Верхняя Саниба'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Ир'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Саниба'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Джимара'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Какадур'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Ламардон'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Фазикау'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Нижняя Кани'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Тменикау'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Нижняя Саниба'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Тарское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Кобан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Комгарон'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Донгарон'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Махческ'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Кармадон'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Чми'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Лескен'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Хазнидон'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Майрамадаг'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Алханчурт'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Дзуарикау'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Камунта'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Нижняя Унал'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Горный Карца'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Кора-Урсдон'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алания'), 'Хидикус')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Алания" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Алания') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Владикавказ'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Ардон'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Дигора'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Алагир'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Моздок'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Беслан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Заводской'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Ногир'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Майский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Гизель'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Михайловское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Чермен'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Сунжа'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Камбилеевское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Кизляр'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Моздокский район'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Эльхотово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Луковская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Павлодольская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Змейская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Нар'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Даргавс'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Куртат'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Верхняя Саниба'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Ир'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Саниба'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Джимара'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Какадур'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Ламардон'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Фазикау'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Нижняя Кани'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Тменикау'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Нижняя Саниба'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Тарское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Кобан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Комгарон'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Донгарон'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Махческ'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Кармадон'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Чми'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Лескен'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Хазнидон'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Майрамадаг'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Алханчурт'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Дзуарикау'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Камунта'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Нижняя Унал'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Горный Карца'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Кора-Урсдон'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Северная Осетия'), 'Хидикус')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Северная Осетия" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Ростовская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Ростов-на-Дону'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Волгодонск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Цимлянск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Константиновск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Сальск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Зверево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Шахты'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Таганрог'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Новочеркасск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Батайск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Гуково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Азов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Семикаракорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Морозовск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Пролетарск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Белая Калитва'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Зерноград'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Каменск-Шахтинский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Новошахтинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Миллерево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Красный Сулин'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Аксай'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Донецк'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Морозовский Район'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Глубокий'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Орловский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Песчанокопское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Багаевская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Тацинская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Тарасовский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Дубовское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Зимовники'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Углеродовский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Покровское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Заветное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Большая Мартыновка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Чертково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Вешенская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Милютинская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Кашары'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Матвеев Курган'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Родионово-Несветайская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Казанская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Романовская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Боковская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Зерноградский район'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Миллеровский район'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Новая Надежда'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Самарское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Камышный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ростовская'), 'Тимизяревский')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Ростовская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Уфа'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Ишимбай'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Туймазы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Давлеканово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Нефтекамск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Октябрьский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Агидель'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Межгорье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Мелеуз'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Стерлитамак'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Бирск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Благовещенск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Баймак'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Салават'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Янаул'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Белебей'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Кумертау'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Дюртили'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Белорецк'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Учалы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Сибай'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Бакалы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Аскино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Раевский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Чишмы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Красноусольский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Иглино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Мясегутово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Башкортостан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Толбазы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Чекмагуш'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Языково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Новобелокатай'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Старобалтачево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Мраково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Буздяк'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Исянгулово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Шаран'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Кандры'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Николо-Березовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Малояз'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Караидель'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Ермолаево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Акъяр'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Нагаево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Уральский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Акбердино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Кага'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Старые Казанчи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Шакарла'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Башкортостан'), 'Минзитарово')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Башкортостан" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Йошкар-Ола'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Звенигово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Козьмодемьянск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Волжск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Новый Торъял'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Куженер'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Юрино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Помары'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Суслонгер'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Краснооктябрьский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Шелангер'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Знаменский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Семеновка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Руэм'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Кокшайск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Русский Кукмор'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Куяр'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Чодраял'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Казанское сельское поселение'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Табашино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Березники'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Дубники'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Ежово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Азаново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Мари-Билямор'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Шора'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Цибикнур'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Косолапово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Троицкий Посад'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Кулаково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Шиньша'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Зашижемье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Пектубаевское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Виловатово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Нурма'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Марисола'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Новые Параты'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Эмеково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Русские Шои'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Петъял'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Еласы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Шойбулак'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Кукнур'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Игнатьево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Микряково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Савино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Емангаши'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Сарамбаево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Юлъялы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Малые Янгурцы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Марий Эл'), 'Коротни')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Марий Эл" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Татарстан') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Казань'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Тетюши'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Арск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Нижнекамск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Елабуга'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Буинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Альметьевск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Азнакаево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Чистополь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Менделеевск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Бугульма'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Бавлы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Заинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Агрыз'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Набережные Челны'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Лениногорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Мамадыш'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Зеленодольск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Кукмор'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Нурлат'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Мензелинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Болгар'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Лаишево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Уруссу'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Васильево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Камские Поляны'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Высокая Гора'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Богатые Сабы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Пестрецы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Верхний Услон'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Базарные Матаки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Актаныш'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Осиново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Куйбышевский Затон'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Нижние Вязовые'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Тенишево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Большие Кайбицы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Большая Атня'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Тюлячи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Старое Дрожжаное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Новошешминск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Муслюмово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Свияжск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Билярск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Тимяшевское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Борискино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Большой Сардек'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Байлянгар'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Большие Ключи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Татарстан'), 'Большие Берези')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Татарстан" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Чувашская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Чебоксары'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Канаш'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Мариинский Посад'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Козловка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Шумерля'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Цивильск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Алатырь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Ядрин'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Новочебоксарск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Порецкое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Красноармейское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Янтиково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Новые Лапсары'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Батырево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Вурнары'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Красные Четаи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Моргауши'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Комсомольское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Кугеси'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Шыгырдан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Яльчики'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Шемурша'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Хирлеппоси'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Ахманей'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Шоршелы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Кольцовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Тойси'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Иваньково-Ленино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Камайкасы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Семёновское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Сугуты'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Завражное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Чуманкасы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Акулево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Карцев Починок'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Янгличи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Старые Щелканы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Именево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Вознесенское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Тогаево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Ердово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Сормвары'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Кудеиха'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Норваш-Шигали'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Ахматово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Березовый Майдан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Чувашская'), 'Алманчиково')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Чувашская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Пермская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Пермь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Гремячинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Березники'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Оса'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Очер'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Лысьва'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Чернушка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Нытва'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Кизел'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Соликамск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Красновишерск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Добрянка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Чайковский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Губаха'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Краснокамск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Горнозаводск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Верещагино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Усолье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Чердынь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Кунгур'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Кудымкар'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Чермоз'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Оханск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Сёла'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Углеуральский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Лямино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Скальный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Калино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Теплая гора'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Сараны'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Промысла'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Парма'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Старый Бисер'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Кусье-Александровский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Пашия'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Сергино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Малая Соснова'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Голубята'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Липово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Альняш'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Дуброво'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Мысы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Звездный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Челва'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Мульково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Кын'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Барда'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Пермская'), 'Большая Соснова')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Пермская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Курганская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Курган'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Далматово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Катайск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Куртамыш'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Шумиха'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Петухово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Шадринск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Макушино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Юргамыш'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Мишкино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Целинное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Половинное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Глядянское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Шатрово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Частоозерье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Ушаковское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Никитинское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Чернавское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Верхняя Теча'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Сафакулево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Кетово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Красный Октябрь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Мартыновка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Шутихинское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Шутино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Верхнесуерское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Петропавловское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Улугушское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Верхнеключевское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Введенское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Смирново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Житниковское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Чемякина'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Усть-Уйское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Пуктыш'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Мальцево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Раскатиха'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Боровичи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Медвежье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Окуневское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Добровольское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Вознесенское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Заложное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Просвет'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Ягодное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Строево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Смолина'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Крутихинское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Малодубровное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Дулино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Курганская'), 'Фроловка')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Курганская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Ханты-Мансийск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Урай'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Нягань'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Югорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Сургут'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Мегион'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Лангепас'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Нижневартовск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Радужный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Советский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Белоярский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Нефтеюганск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Когалым'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Пыть-Ях'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Покачи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Лянтор'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Березово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Долгое Плесо'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Батово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Цингалы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Согом'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Горноправдинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Селиярово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Куминский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Таежный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Игрим'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Кондинское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Зенково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Нялина'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Реполово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Елизарово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Кедровый'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Лугофилинская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Шапша'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Кышик'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Кирпичный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Луговской'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Белгорье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Красноленинский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Сибирский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Чембакчина'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Берёзовский район'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Луговой'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Аган'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Большой Камень'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Шаим'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Ямки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Ханты-Мансийский'), 'Малый Атлым')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Ханты-Мансийский" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Алтай') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Горно-Алтайск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Онгурдайское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Усть-Кан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Турочак'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Кызыл-Озёк'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Черга'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Сёйка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Катанда'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Уймень'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Мариинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Каспинское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Актел'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Верх-Апшуяхта'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Беш-озек'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Красносельск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Мыюта'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Бешпельтир'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Уожан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Ускуч'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Каракол'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Узнезя'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Ортолык'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Кулада'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Урлу-Аспак'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Каракокша'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Усть-Сема'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Верх-Уймон'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Замульта'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Барангол'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Ябоганское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Анос'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Усть-Мутинское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Ело'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Инегень'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Иодро'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Кара Кобы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Новый Бельтир'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Акбом'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Чуйозы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Малый Яломан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Малая Иня'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Аркыт'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Боочи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Большой Яломан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Курота'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Нижняя Талда'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Сухой Карасук'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Огневка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Курдюм'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Яконур'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Алтай'), 'Курунда')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Алтай" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Тыва') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Кызыл'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Шагонар'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Туран'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Чадан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Ак-Довурак'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Каа-Хем'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Бай-Хем'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Бий-Хем'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Сырыг-Сеп'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Эрзин'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Балгазын'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Сукпак'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Хову-Аксы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Суг-Аксы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Мугур-Аксы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Чаа-Холь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Кызыл-Мажалык'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Сыстыг-Хем'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Барлык'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Севи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Ий'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Хайыракан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Уюк'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Алдан-Маадыр'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Усть-Элегест'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Аржаан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Ак-тал'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Адыр-Кежиг'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Ак-даш'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Аксы-барлык'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Авыйган'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Ак-дуруг'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Ак-Эрик'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Бажын-Алаак'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Арыг-Узю'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'О-шынаа'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Кызыл-Даг'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Ийме'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Шуй'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Кундустуг'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Кызыл-Хая'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Шеми'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Булун-бажы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Салдам'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Чыргакы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Усть-хадын'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Шеклээр'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Чыраа-Бажы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Хондергей'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Элдиг-хем'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Баян-тала'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Тыва'), 'Хорум-даг')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Тыва" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Хакасия') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Абакан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Абаза'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Сорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Саяногорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Черногорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Усть-Абакан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Черемушки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Шира'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Таштып'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Туим'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Вершина Теи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Белый Яр'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Бискамжа'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Пригорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Калинино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Подсинее'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Бельтирский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Изыхские Копи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Июс'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Пуланколь'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Сонское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Арбатский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Малые Арбаты'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Кайбалы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Райковский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Усть-сос'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Балыкса'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Ефремкино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Усть-Бюр'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Куйбышево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Неожиданный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Летник'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Полиндейка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Очуры'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Усть-чуль'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Целинное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Краснополье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Топанов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Перевозное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Кожухово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Сарагаш'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Усть-ерба'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Большая Сея'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Вершино-Биджа'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Чебаки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Харачул'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Красный Камень'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Чертыковская'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Хакасия'), 'Карагай')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Хакасия" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Эвенкийский') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Эвенкийский'), 'Тура'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Эвенкийский'), 'Учами'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Эвенкийский'), 'Бурный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Эвенкийский'), 'Чемдальск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Эвенкийский'), 'Юкта'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Эвенкийский'), 'Чиринда'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Эвенкийский'), 'Муторай'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Эвенкийский'), 'Тутончаны'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Эвенкийский'), 'Суломай'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Эвенкийский'), 'Суринда'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Эвенкийский'), 'Ошарово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Эвенкийский'), 'Кузьмовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Эвенкийский'), 'Усть-Камо'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Эвенкийский'), 'Ногинск')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Эвенкийский" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Кемерово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Новокузнецк'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Ленинск-Кузнецкий'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Топки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Березовский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Междуреченск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Мариинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Полысаево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Гурьевск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Белово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Прокопьевск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Калтан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Осинники'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Анжеро-Судженск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Киселевск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Мыски'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Юрга'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Салаир'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Таштагол'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Краснобродский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Тайга'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Промышленная'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Яя'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Яшкино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Вишнёвка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Новый Городок'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Таштагольский район'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Зеленогорский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Итатский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Мариинский район'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Белогорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Юргинский район'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Гурьевский район'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Воскресенка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Суровка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Верх-Великосельское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Валерьяновка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Амзас'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Вотиновка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Верхотомское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Ваганово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Васьково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Варюхино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Восходящий'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Нижняя Суета'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Иверка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Дегтяревка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Верхний Калтан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Котовский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Новороссийка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Кемеровская'), 'Комсомольск')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Кемеровская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Томская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Томск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Северск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Колпашево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Кедровый'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Асино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Стрежевой'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Первомайское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Александровское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Тимирязевское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Белоярское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Кисловка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Зырянское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Конинино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Половинка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Бакчар'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Дзержинское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Молчаново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Мельниково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Тогур'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Парабель'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Лоскутово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Зональная станция'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Эушта'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Синий Утёс'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Басандайка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Моряковский Затон'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Халдеево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Батурино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Рыбалово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Калтай'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Кафтанчиково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Лучаново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Подломск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Итатка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Межениновка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Тахтамышево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Сухоречье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Семилужки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Новорождественское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Ново-Архангельское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Вершинино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Курлек'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Наумовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Корнилово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Зоркальцево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Петухово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Турунтаево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Карбышево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Верхнее Сеченево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Спасо-Яйское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Томская'), 'Перовка')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Кемеровская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Читинская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Чита'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Шилка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Балей'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Сретенск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Домна'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Кокуй'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Дровяная'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Улеты'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Хвойный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Черново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Ягодный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Ясная'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Маккавеево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Верх-Чита'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Смоленка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Орловский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Оленгуй'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Мухор-Кондуй'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Бургень'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Сохондо'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Преображенка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Новотроицк'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Береговой'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Елизаветино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Сивяково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Домно-ключи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Старая Кука'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Беклемишево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Яблоново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Иргень'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Сыпчегур'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Верх. Нарым'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Шишкино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Подволок'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Кука'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Ручейки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Карповка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Лесной Городок'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Иван-Озеро'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Ильинка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Нижняя Шахтама'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Бырка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Тунгокочен'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Батакан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Бура'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Абагайтуй'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Мангут'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Бурулятуй'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Казаковский Промысел'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Читинская'), 'Новоцурухайтуй')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Читинская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Приморский') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Владивосток'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Артем'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Уссурийск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Находка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Дальнереченск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Лесозаводск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Фокино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Большой Камень'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Партизанск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Арсеньев'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Дальнегорск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Спасск-Дальний'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Лазо'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Черниговка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Пластун'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Чугуевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Хрустальный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Зарубино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Смоляниново'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Трудовое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Шмаковка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Владимиро-Александровское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Яковлевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Покровка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Чкаловское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Гродеково'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Камень-Рыболов'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Раздольное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Береговое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Саратовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Полевое'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Виноградовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Гражданка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Минеральное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Устиновка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Авдеевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Сокольчи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Черноручье'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Борисовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Новоникольск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Филаретовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Суражевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Ясное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Голубовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Хмыловка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Серафимовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Новостройка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Нагорное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Верхний Перевал'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Приморский'), 'Губерово')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Приморский" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Камчатская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Петропавловск-Камчатский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Елизово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Вилючинск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Усть-Камчатск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Палана'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Эссо'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Соболево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Ключи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Тиличики'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Каменское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Коряки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Оссора'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Вулканный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Козыревское сельское поселение'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Термальный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Раздольный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Озерновский'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Нагорный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Атласовское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Таежный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Березняки'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Ганалы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Апача'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Усть-Хайрюзово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Седанка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Средние Пахачи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Манилы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Шаромы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Лазо'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Пущино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Карымай'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Кавалерское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Оклан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Алука'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Пиначево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Хаилино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Село Вывенка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Ивашка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Слаутное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Малка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Майское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Кеткино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Крутобереговый'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Шумный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Паужетка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Никольское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Кострома'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Николаевка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Камчатская'), 'Октябрьский')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Камчатская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Корякский') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Корякский'), 'Палана'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Корякский'), 'Каменское'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Корякский'), 'Тиличики'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Корякский'), 'Воямполка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Корякский'), 'Седанка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Корякский'), 'Манилы'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Корякский'), 'Кострома'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Корякский'), 'Оклан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Корякский'), 'Средние Пахачи'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Корякский'), 'Апука'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Корякский'), 'Парень'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Корякский'), 'Хаилино'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Корякский'), 'Усть-Хайрюзово'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Корякский'), 'Село Вывенка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Корякский'), 'Ивашка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Корякский'), 'Слаутное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Корякский'), 'Корф'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Корякский'), 'Оссора'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Корякский'), 'Воямполка')
       ON CONFLICT (region_id, text) DO NOTHING;

ELSE
        RAISE NOTICE 'Регион "Корякский" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM parsed_advertisements_module.geos WHERE text = 'Магаданская') THEN
        INSERT INTO parsed_advertisements_module.cities (id, region_id, text)
    VALUES
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Магадан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Сусуман'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Ягодное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Сокол'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Холодный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Галимый'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Бурхала'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Дукат'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Уптар'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Карамкен'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Беличан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Армань'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Талая'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Мальдяк'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Талон'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Тауйск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Усть-Хакчан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Верхний Сеймчан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Балаганное'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Гадля'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Тахтоямск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Гарманда'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Верхний Парень'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Клёпка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Транспортный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Меренга'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Ямск'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Тополовка'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Кедровый'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Буркандья'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Мой-уруста'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Радужный'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Усть-Среднекан'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Кулу'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Большевик'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Оротук'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Ларюковая'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Широкий'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Ударник'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Яна'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Нагаево'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Обо'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Яна'),
        (uuid_generate_v4(), (SELECT id FROM parsed_advertisements_module.geos WHERE text = 'Магаданская'), 'Мякит')
       ON CONFLICT (region_id, text) DO NOTHING;
ELSE
        RAISE NOTICE 'Регион "Магаданская" не найден в таблице geos. Вставка пропущена.';
END IF;
END $$;
