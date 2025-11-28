CREATE SCHEMA IF NOT EXISTS parsers_control_module;

CREATE TABLE IF NOT EXISTS parsers_control_module.registered_parsers(
  id uuid primary key,
  type varchar(128),
  domain varchar(128),
  UNIQUE (type, domain)
);

CREATE INDEX IF NOT EXISTS idx_registered_parsers_type ON parsers_control_module.registered_parsers(type);
CREATE INDEX IF NOT EXISTS idx_registered_parsers_domain ON parsers_control_module.registered_parsers(domain);