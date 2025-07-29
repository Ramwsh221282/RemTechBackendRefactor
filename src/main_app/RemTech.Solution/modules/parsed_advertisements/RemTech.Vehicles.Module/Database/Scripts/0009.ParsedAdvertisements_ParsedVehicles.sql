CREATE TABLE IF NOT EXISTS parsed_advertisements_module.parsed_vehicles(
    id                      VARCHAR(100) PRIMARY KEY,
    kind_id                 UUID NOT NULL REFERENCES parsed_advertisements_module.vehicle_kinds(id) ON DELETE CASCADE,
    brand_id                UUID NOT NULL REFERENCES parsed_advertisements_module.vehicle_brands(id) ON DELETE CASCADE,
    geo_id                  UUID NOT NULL REFERENCES parsed_advertisements_module.geos(id) ON DELETE CASCADE,
    model_id                UUID NOT NULL REFERENCES parsed_advertisements_module.vehicle_models(id) ON DELETE CASCADE,
    price                   BIGINT NOT NULL,
    is_nds                  BOOLEAN NOT NULL,
    photos                  JSONB NOT NULL,
    document_tsvector       TSVECTOR    
);

CREATE INDEX IF NOT EXISTS idx_parsed_vehicles_document_tsvector
    ON parsed_advertisements_module.parsed_vehicles USING GIN(document_tsvector);

CREATE INDEX IF NOT EXISTS idx_parsed_vehicles_price
    ON parsed_advertisements_module.parsed_vehicles(price);

