CREATE SCHEMA IF NOT EXISTS locations_module;

CREATE TABLE IF NOT EXISTS locations_module.regions(
  id    UUID NOT NULL,
  name  VARCHAR(50) NOT NULL,
  kind  VARCHAR(20) NOT NULL,
  embedding vector(1024) NOT NULL,
  PRIMARY KEY(id),
  UNIQUE(name, kind)
);

CREATE INDEX IF NOT EXISTS idx_locations_module_regions_id ON locations_module.regions(id);

CREATE INDEX IF NOT EXISTS idx_locations_module_regions_name ON locations_module.regions(name);

CREATE INDEX IF NOT EXISTS idx_locations_module_regions_hnsw
    ON locations_module.regions USING hnsw (embedding vector_cosine_ops);

CREATE TABLE IF NOT EXISTS locations_module.cities(
    id  UUID NOT NULL,
    region_id UUID NOT NULL,
    name VARCHAR(50) NOT NULL,
    embedding vector(1024) NOT NULL,
    PRIMARY KEY(id),
    FOREIGN KEY(region_id) REFERENCES locations_module.regions(id) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS idx_locations_module_cities_id ON locations_module.cities(id);

CREATE INDEX IF NOT EXISTS idx_locations_module_cities_region_id ON locations_module.cities(region_id);

CREATE INDEX IF NOT EXISTS idx_locations_module_cities_name ON locations_module.cities(name);

CREATE INDEX IF NOT EXISTS idx_locations_module_cities_hnsw
    ON locations_module.cities USING hnsw (embedding vector_cosine_ops);