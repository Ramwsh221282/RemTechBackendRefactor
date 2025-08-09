CREATE TABLE IF NOT EXISTS parsed_advertisements_module.parsed_vehicles(
    id                      VARCHAR(100) PRIMARY KEY,
    kind_id                 UUID NOT NULL REFERENCES parsed_advertisements_module.vehicle_kinds(id) ON DELETE CASCADE,
    brand_id                UUID NOT NULL REFERENCES parsed_advertisements_module.vehicle_brands(id) ON DELETE CASCADE,
    geo_id                  UUID NOT NULL REFERENCES parsed_advertisements_module.geos(id) ON DELETE CASCADE,
    model_id                UUID NOT NULL REFERENCES parsed_advertisements_module.vehicle_models(id) ON DELETE CASCADE,
    price                   BIGINT NOT NULL,
    is_nds                  BOOLEAN NOT NULL,
    source_url              TEXT NOT NULL,
    source_domain           VARCHAR(50) NOT NULL,
    object                  JSONB NOT NULL,    
    description             TEXT NOT NULL,
    document_tsvector       TSVECTOR,
    embedding               vector(1024)
);

CREATE INDEX IF NOT EXISTS idx_vehicle_parsed_vehicles_hnsw
    ON parsed_advertisements_module.parsed_vehicles USING hnsw (embedding vector_cosine_ops);

CREATE INDEX IF NOT EXISTS idx_parsed_vehicles_document_tsvector
    ON parsed_advertisements_module.parsed_vehicles USING GIN(document_tsvector);

CREATE INDEX IF NOT EXISTS idx_parsed_vehicles_price
    ON parsed_advertisements_module.parsed_vehicles(price);