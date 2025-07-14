CREATE TABLE IF NOT EXISTS parsed_advertisements_module.parsed_vehicles(
    id                      VARCHAR(50) PRIMARY KEY,
    kind_id                 UUID NOT NULL REFERENCES parsed_advertisements_module.vehicle_kinds(id) ON DELETE CASCADE,
    brand_id                UUID NOT NULL REFERENCES parsed_advertisements_module.vehicle_brands(id) ON DELETE CASCADE,
    geo_id                  UUID NOT NULL REFERENCES parsed_advertisements_module.geos(id) ON DELETE CASCADE,
    price                   BIGINT NOT NULL,
    description             TEXT NOT NULL,
    title                   VARCHAR(300) NOT NULL,
    photos                  JSONB NOT NULL,
    document_tsvector       TSVECTOR,
    FOREIGN KEY (id) REFERENCES shared_advertisements_module.contained_items(id) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS idx_parsed_vehicles_document_tsvector
    ON parsed_advertisements_module.parsed_vehicles USING GIN(document_tsvector);

CREATE INDEX IF NOT EXISTS idx_parsed_vehicles_price
    ON parsed_advertisements_module.parsed_vehicles(price);

CREATE INDEX IF NOT EXISTS idx_parsed_vehicles_description
    ON parsed_advertisements_module.parsed_vehicles(description);

CREATE INDEX IF NOT EXISTS idx_parsed_vehicles_title
    ON parsed_advertisements_module.parsed_vehicles(title);

CREATE OR REPLACE FUNCTION parsed_advertisements_module.update_document_tsvector()
RETURNS trigger AS $$
BEGIN
  NEW.document_tsvector := to_tsvector('russian', COALESCE(NEW.title, '') || ' ' || COALESCE(NEW.description, ''));
RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_trigger
        WHERE tgname = 'trg_update_document_tsvector'
        AND tgrelid = 'parsed_advertisements_module.parsed_vehicles'::regclass
    ) THEN
CREATE TRIGGER trg_update_document_tsvector
    BEFORE INSERT OR UPDATE ON parsed_advertisements_module.parsed_vehicles
                         FOR EACH ROW EXECUTE FUNCTION parsed_advertisements_module.update_document_tsvector();
END IF;
END
$$;
