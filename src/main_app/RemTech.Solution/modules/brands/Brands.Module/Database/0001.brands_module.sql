CREATE SCHEMA IF NOT EXISTS brands_module;

CREATE TABLE IF NOT EXISTS brands_module.brands(
    id                      UUID PRIMARY KEY,
    name                    VARCHAR(80) UNIQUE NOT NULL,
    rating                  BIGINT NOT NULL,
    embedding               vector(1024) NOT NULL
);

CREATE INDEX IF NOT EXISTS idx_vehicle_brands_hnsw
    ON brands_module.brands USING hnsw (embedding vector_cosine_ops);

CREATE INDEX IF NOT EXISTS idx_vehicle_brands_text
    ON brands_module.brands(name);