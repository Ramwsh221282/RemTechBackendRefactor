CREATE TABLE IF NOT EXISTS parsed_advertisements_module.vehicle_characteristics(
    id                  UUID PRIMARY KEY,
    text                VARCHAR(80) UNIQUE NOT NULL,
    measuring           VARCHAR(10) NOT NULL,    
    UNIQUE(text, measuring)
);

-- CREATE INDEX IF NOT EXISTS idx_vehicle_characteristics_tsvector
--     ON parsed_advertisements_module.vehicle_characteristics USING GIN(document_tsvector);

CREATE INDEX IF NOT EXISTS idx_vehicle_characteristics_text
    ON parsed_advertisements_module.vehicle_characteristics(text);
