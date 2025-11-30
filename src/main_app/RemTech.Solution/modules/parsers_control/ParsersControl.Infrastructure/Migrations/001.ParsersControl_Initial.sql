CREATE SCHEMA IF NOT EXISTS parsers_control_module;

CREATE TABLE IF NOT EXISTS parsers_control_module.registered_parsers(
  id uuid primary key,
  type varchar(128),
  domain varchar(128),
  UNIQUE (type, domain)
);

CREATE INDEX IF NOT EXISTS idx_registered_parsers_type ON parsers_control_module.registered_parsers(type);
CREATE INDEX IF NOT EXISTS idx_registered_parsers_domain ON parsers_control_module.registered_parsers(domain);

CREATE TABLE IF NOT EXISTS parsers_control_module.work_states(
  id uuid primary key,
  state varchar(128) NOT NULL
);

CREATE TABLE IF NOT EXISTS parsers_control_module.links(
  id uuid primary key,
  name varchar(256) NOT NULL,
  url text NOT NULL,
  is_ignored boolean NOT NULL,
  parser_id uuid NOT NULL,
  UNIQUE (name, url, parser_id)
);

CREATE TABLE IF NOT EXISTS parsers_control_module.statistics(
  id uuid primary key,
  processed int not null,
  elapsed_seconds bigint not null
);

CREATE TABLE IF NOT EXISTS parsers_control_module.schedules
(
    id uuid primary key,
    finished_at timestamptz,
    wait_days integer
);