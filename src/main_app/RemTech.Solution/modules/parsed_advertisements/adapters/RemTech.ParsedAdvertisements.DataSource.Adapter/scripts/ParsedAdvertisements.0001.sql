CREATE EXTENSION IF NOT EXISTS pg_trgm;

CREATE TABLE IF NOT EXISTS vehicle_kinds (
    id              UUID PRIMARY KEY,
    text            VARCHAR(150) UNIQUE NOT NULL
);
CREATE INDEX IF NOT EXISTS idx_vehicle_kinds_text ON vehicle_kinds(text);

CREATE TABLE IF NOT EXISTS vehicle_brands (
    id              UUID PRIMARY KEY,
    text            VARCHAR(150) UNIQUE NOT NULL
);
CREATE INDEX IF NOT EXISTS idx_vehicle_brands_text ON vehicle_brands(text);

CREATE TABLE IF NOT EXISTS geos (
    id              UUID PRIMARY KEY,
    text            VARCHAR(150) UNIQUE NOT NULL
);
CREATE INDEX IF NOT EXISTS idx_vehicle_geos_text ON geos(text);

CREATE TABLE IF NOT EXISTS vehicle_characteristics (
    id              UUID PRIMARY KEY,
    text            VARCHAR(100) UNIQUE NOT NULL
);
CREATE INDEX IF NOT EXISTS idx_vehicle_characteristics_text ON vehicle_characteristics(text);

CREATE TABLE IF NOT EXISTS parsed_vehicles (
    id              VARCHAR(50) PRIMARY KEY,
    price           BIGINT NOT NULL,
    description     TEXT NOT NULL,
    title           VARCHAR(300) NOT NULL,
    photos          JSONB NOT NULL
);
CREATE INDEX IF NOT EXISTS idx_parsed_vehicles_price ON parsed_vehicles(price);
CREATE INDEX IF NOT EXISTS idx_parsed_vehicles_description ON parsed_vehicles(description);
CREATE INDEX IF NOT EXISTS idx_parsed_vehicles_title ON parsed_vehicles(title);
CREATE INDEX IF NOT EXISTS idx_parsed_vehicles_title_trgm ON parsed_vehicles USING gin(title gin_trgm_ops);
CREATE INDEX IF NOT EXISTS idx_parsed_vehicles_description_trgm ON parsed_vehicles USING gin(description gin_trgm_ops);

CREATE TABLE IF NOT EXISTS parsed_vehicle_kinds (
    vehicle_id      VARCHAR(50),
    kind_id         UUID,
    PRIMARY KEY (vehicle_id, kind_id),
    FOREIGN KEY (vehicle_id) REFERENCES parsed_vehicles(id) ON DELETE CASCADE,
    FOREIGN KEY (kind_id) REFERENCES vehicle_kinds(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS parsed_vehicle_geos (
    vehicle_id      VARCHAR(50),
    geo_id          UUID,
    PRIMARY KEY (vehicle_id, geo_id),
    FOREIGN KEY (vehicle_id) REFERENCES parsed_vehicles(id) ON DELETE CASCADE,
    FOREIGN KEY (geo_id) REFERENCES geos(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS parsed_vehicle_brands (
    vehicle_id      VARCHAR(50),
    brand_id        UUID,
    PRIMARY KEY (vehicle_id, brand_id),
    FOREIGN KEY (vehicle_id) REFERENCES parsed_vehicles(id) ON DELETE CASCADE,
    FOREIGN KEY (brand_id) REFERENCES vehicle_brands(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS parsed_vehicle_characteristics (
    vehicle_id      VARCHAR(50),
    ctx_id          UUID,
    ctx_name        VARCHAR(100) NOT NULL,
    ctx_value       VARCHAR(50) NOT NULL,
    PRIMARY KEY (vehicle_id, ctx_id),
    FOREIGN KEY (vehicle_id) REFERENCES parsed_vehicles(id) ON DELETE CASCADE,
    FOREIGN KEY (ctx_id) REFERENCES vehicle_characteristics(id) ON DELETE CASCADE,
    UNIQUE (vehicle_id, ctx_id, ctx_name, ctx_value)
);
CREATE INDEX IF NOT EXISTS idx_parsed_vehicle_characteristics_ctx_name ON parsed_vehicle_characteristics(ctx_name);
CREATE INDEX IF NOT EXISTS idx_parsed_vehicle_characteristics_ctx_value ON parsed_vehicle_characteristics(ctx_value);
