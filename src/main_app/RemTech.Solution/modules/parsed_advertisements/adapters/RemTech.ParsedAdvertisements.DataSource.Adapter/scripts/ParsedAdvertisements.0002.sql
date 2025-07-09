CREATE INDEX IF NOT EXISTS idx_vehicle_kinds_document_tsvector
    ON parsed_advertisements_module.vehicle_kinds USING GIN(document_tsvector);

CREATE INDEX IF NOT EXISTS idx_vehicle_kinds_text
    ON parsed_advertisements_module.vehicle_kinds(text);

CREATE INDEX IF NOT EXISTS idx_vehicle_brands_document_tsvector
    ON parsed_advertisements_module.vehicle_brands USING GIN(document_tsvector);

CREATE INDEX IF NOT EXISTS idx_vehicle_brands_text
    ON parsed_advertisements_module.vehicle_brands(text);

CREATE INDEX IF NOT EXISTS idx_vehicle_geos_tsvector
    ON parsed_advertisements_module.geos USING GIN(document_tsvector);

CREATE INDEX IF NOT EXISTS idx_vehicle_geos_text
    ON parsed_advertisements_module.geos(text);

CREATE INDEX IF NOT EXISTS idx_vehicle_characteristics_tsvector
    ON parsed_advertisements_module.vehicle_characteristics USING GIN(document_tsvector);

CREATE INDEX IF NOT EXISTS idx_vehicle_characteristics_text
    ON parsed_advertisements_module.vehicle_characteristics(text);

CREATE INDEX IF NOT EXISTS idx_parsed_vehicles_document_tsvector
    ON parsed_advertisements_module.parsed_vehicles USING GIN(document_tsvector);

CREATE INDEX IF NOT EXISTS idx_parsed_vehicles_price
    ON parsed_advertisements_module.parsed_vehicles(price);

CREATE INDEX IF NOT EXISTS idx_parsed_vehicles_description
    ON parsed_advertisements_module.parsed_vehicles(description);

CREATE INDEX IF NOT EXISTS idx_parsed_vehicles_title
    ON parsed_advertisements_module.parsed_vehicles(title);

CREATE INDEX IF NOT EXISTS idx_parsed_vehicles_title_trgm
    ON parsed_advertisements_module.parsed_vehicles USING gin(title gin_trgm_ops);

CREATE INDEX IF NOT EXISTS idx_parsed_vehicles_description_trgm
    ON parsed_advertisements_module.parsed_vehicles USING gin(description gin_trgm_ops);

CREATE INDEX IF NOT EXISTS idx_parsed_vehicle_characteristics_ctx_name
    ON parsed_advertisements_module.parsed_vehicle_characteristics(ctx_name);

CREATE INDEX IF NOT EXISTS idx_parsed_vehicle_characteristics_ctx_value
    ON parsed_advertisements_module.parsed_vehicle_characteristics(ctx_value);
