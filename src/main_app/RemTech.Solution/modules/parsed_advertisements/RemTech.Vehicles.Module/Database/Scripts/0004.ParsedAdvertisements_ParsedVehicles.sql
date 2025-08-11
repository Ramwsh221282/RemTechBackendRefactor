CREATE TABLE IF NOT EXISTS parsed_advertisements_module.parsed_vehicles(
    id                      VARCHAR(100) PRIMARY KEY,
    kind_id                 UUID NOT NULL,
    brand_id                UUID NOT NULL,
    geo_id                  UUID NOT NULL,
    model_id                UUID NOT NULL,
    price                   BIGINT NOT NULL,
    is_nds                  BOOLEAN NOT NULL,
    source_url              TEXT NOT NULL,
    source_domain           VARCHAR(50) NOT NULL,
    object                  JSONB NOT NULL,    
    description             TEXT NOT NULL,
    embedding               vector(1024)
);

CREATE INDEX IF NOT EXISTS idx_vehicle_parsed_vehicles_hnsw
    ON parsed_advertisements_module.parsed_vehicles USING hnsw (embedding vector_cosine_ops);

CREATE INDEX IF NOT EXISTS idx_parsed_vehicles_price
    ON parsed_advertisements_module.parsed_vehicles(price);

CREATE INDEX IF NOT EXISTS idx_parsed_vehicles_kind_id
    ON parsed_advertisements_module.parsed_vehicles(kind_id);

CREATE INDEX IF NOT EXISTS idx_parsed_vehicles_brand_id
    ON parsed_advertisements_module.parsed_vehicles(brand_id);

CREATE INDEX IF NOT EXISTS idx_parsed_vehicles_model_id
    ON parsed_advertisements_module.parsed_vehicles(model_id);

CREATE INDEX IF NOT EXISTS idx_parsed_vehicles_geo_id
    ON parsed_advertisements_module.parsed_vehicles(geo_id);