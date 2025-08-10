CREATE SCHEMA IF NOT EXISTS spares_module;

CREATE TABLE IF NOT EXISTS spares_module.spares(
    id              VARCHAR(100) NOT NULL,
    region_id       UUID NOT NULL,
    city_id         UUID NOT NULL,
    price           BIGINT NOT NULL,
    is_nds          BOOLEAN NOT NULL,
    source_url      TEXT NOT NULL UNIQUE,
    source_domain   TEXT NOT NULL,
    object          JSONB NOT NULL,
    embedding       vector(1024) NOT NULL,
    PRIMARY KEY (id)    
);

CREATE INDEX IF NOT EXISTS idx_spares_module_id ON spares_module.spares(id);

CREATE INDEX IF NOT EXISTS idx_spares_module_region_id ON spares_module.spares(region_id);

CREATE INDEX IF NOT EXISTS idx_spares_module_city_id ON spares_module.spares(city_id);

CREATE INDEX IF NOT EXISTS idx_spares_module_source_url ON spares_module.spares(source_url);

CREATE INDEX IF NOT EXISTS idx_spares_module_source_hnsw
    ON spares_module.spares USING hnsw (embedding vector_cosine_ops);