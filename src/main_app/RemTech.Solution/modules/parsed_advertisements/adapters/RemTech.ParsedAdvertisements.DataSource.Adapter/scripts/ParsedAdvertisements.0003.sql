CREATE OR REPLACE FUNCTION parsed_advertisements_module.update_text_document_tsvector()
RETURNS trigger AS $$
BEGIN
  NEW.document_tsvector := to_tsvector('russian', COALESCE(NEW.text, ''));
RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION parsed_advertisements_module.update_document_tsvector()
RETURNS trigger AS $$
BEGIN
  NEW.document_tsvector := to_tsvector('russian', COALESCE(NEW.title, '') || ' ' || COALESCE(NEW.description, ''));
RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_update_document_tsvector
    BEFORE INSERT OR UPDATE ON parsed_advertisements_module.parsed_vehicles
                         FOR EACH ROW EXECUTE FUNCTION parsed_advertisements_module.update_document_tsvector();

CREATE TRIGGER trg_update_vehicle_kinds_document_tsvector
    BEFORE INSERT OR UPDATE ON parsed_advertisements_module.vehicle_kinds
                         FOR EACH ROW EXECUTE FUNCTION parsed_advertisements_module.update_text_document_tsvector();

CREATE TRIGGER trg_update_vehicle_brands_document_tsvector
    BEFORE INSERT OR UPDATE ON parsed_advertisements_module.vehicle_brands
                         FOR EACH ROW EXECUTE FUNCTION parsed_advertisements_module.update_text_document_tsvector();

CREATE TRIGGER trg_update_geos_document_tsvector
    BEFORE INSERT OR UPDATE ON parsed_advertisements_module.geos
                         FOR EACH ROW EXECUTE FUNCTION parsed_advertisements_module.update_text_document_tsvector();

CREATE TRIGGER trg_update_vehicle_characteristics_document_tsvector
    BEFORE INSERT OR UPDATE ON parsed_advertisements_module.vehicle_characteristics
                         FOR EACH ROW EXECUTE FUNCTION parsed_advertisements_module.update_text_document_tsvector();
