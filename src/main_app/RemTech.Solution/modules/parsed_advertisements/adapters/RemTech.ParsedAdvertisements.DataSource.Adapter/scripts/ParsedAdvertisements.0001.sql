CREATE SCHEMA IF NOT EXISTS shared_advertisements_module;

CREATE TABLE IF NOT EXISTS shared_advertisements_module.contained_items (
    id              VARCHAR(50),
    source_id       UUID NOT NULL REFERENCES parsers_management_module.parser_links(id) ON DELETE SET NULL,
    date_created    DATE NOT NULL,
    is_new          BOOLEAN NOT NULL,
    PRIMARY KEY (id),
    UNIQUE  (source_id)
);

CREATE SCHEMA IF NOT EXISTS parsed_advertisements_module;

CREATE TABLE IF NOT EXISTS parsed_advertisements_module.vehicle_kinds(
    id              UUID PRIMARY KEY,
    text            VARCHAR(150) UNIQUE NOT NULL,
    document_tsvector       TSVECTOR
);

CREATE TABLE IF NOT EXISTS parsed_advertisements_module.vehicle_brands(
    id              UUID PRIMARY KEY,
    text            VARCHAR(150) UNIQUE NOT NULL,
    document_tsvector       TSVECTOR
);

CREATE TABLE IF NOT EXISTS parsed_advertisements_module.geos(
    id              UUID PRIMARY KEY,
    text            VARCHAR(150) UNIQUE NOT NULL,
    document_tsvector       TSVECTOR
);

CREATE TABLE IF NOT EXISTS parsed_advertisements_module.vehicle_characteristics(
    id              UUID PRIMARY KEY,
    text            VARCHAR(100) UNIQUE NOT NULL,
    document_tsvector       TSVECTOR
);

CREATE TABLE IF NOT EXISTS parsed_advertisements_module.parsed_vehicles(
    id                      VARCHAR(50) PRIMARY KEY,
    kind_id                 UUID NOT NULL REFERENCES parsed_advertisements_module.vehicle_kinds(id) ON DELETE CASCADE,
    brand_id                UUID NOT NULL REFERENCES parsed_advertisements_module.vehicle_brands(id) ON DELETE CASCADE,
    geo_id                  UUID NOT NULL REFERENCES parsed_advertisements_module.geos(id) ON DELETE CASCADE,
    price                   BIGINT NOT NULL,
    description             TEXT NOT NULL,
    title                   VARCHAR(300) NOT NULL,
    photos                  JSONB NOT NULL,
    document_tsvector       TSVECTOR,
    FOREIGN KEY (id) REFERENCES shared_advertisements_module.contained_items(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS parsed_advertisements_module.parsed_vehicle_characteristics(
    vehicle_id      VARCHAR(50),
    ctx_id          UUID,
    ctx_name        VARCHAR(100) NOT NULL,
    ctx_value       VARCHAR(50) NOT NULL,
    PRIMARY KEY (vehicle_id, ctx_id),
    FOREIGN KEY (vehicle_id) REFERENCES parsed_advertisements_module.parsed_vehicles(id) ON DELETE CASCADE,
    FOREIGN KEY (ctx_id) REFERENCES parsed_advertisements_module.vehicle_characteristics(id) ON DELETE CASCADE,
    UNIQUE (vehicle_id, ctx_id, ctx_name, ctx_value)
);
