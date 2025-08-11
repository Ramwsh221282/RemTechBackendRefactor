CREATE SCHEMA IF NOT EXISTS models_module;

CREATE TABLE IF NOT EXISTS models_module.models(
    id                   UUID PRIMARY KEY,
    name                 VARCHAR(50) UNIQUE,
    rating               BIGINT NOT NULL,
    embedding            vector(1024)
);

CREATE INDEX IF NOT EXISTS idx_vehicle_parsed_vehicles_hnsw
    ON models_module.models USING hnsw (embedding vector_cosine_ops);

CREATE INDEX IF NOT EXISTS idx_vehicle_models_text
    ON models_module.models(name);