CREATE SCHEMA IF NOT EXISTS categories_module; 

CREATE TABLE IF NOT EXISTS categories_module.categories(
    id                   UUID PRIMARY KEY,
    name                 VARCHAR(80) UNIQUE NOT NULL,
    rating               BIGINT NOT NULL,
    embedding            vector(1024) NOT NULL
);

CREATE INDEX IF NOT EXISTS idx_vehicle_kinds_hnsw
    ON categories_module.categories USING hnsw (embedding vector_cosine_ops);

CREATE INDEX IF NOT EXISTS idx_vehicle_kinds_text
    ON categories_module.categories(name);