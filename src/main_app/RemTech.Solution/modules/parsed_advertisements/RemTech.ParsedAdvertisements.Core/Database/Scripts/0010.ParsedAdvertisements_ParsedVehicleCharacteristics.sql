CREATE TABLE IF NOT EXISTS parsed_advertisements_module.parsed_vehicle_characteristics(
    vehicle_id      VARCHAR(100),
    ctx_id          UUID,
    ctx_name        VARCHAR(80) NOT NULL,
    ctx_value       VARCHAR(30) NOT NULL,
    ctx_measure     VARCHAR(30) NOT NULL,
    PRIMARY KEY (vehicle_id, ctx_id),
    FOREIGN KEY (vehicle_id) REFERENCES parsed_advertisements_module.parsed_vehicles(id) ON DELETE CASCADE,
    FOREIGN KEY (ctx_id) REFERENCES parsed_advertisements_module.vehicle_characteristics(id) ON DELETE CASCADE    
);

CREATE INDEX IF NOT EXISTS idx_parsed_vehicle_characteristics_ctx_name
    ON parsed_advertisements_module.parsed_vehicle_characteristics(ctx_name);

CREATE INDEX IF NOT EXISTS idx_parsed_vehicle_characteristics_ctx_value
    ON parsed_advertisements_module.parsed_vehicle_characteristics(ctx_value);


