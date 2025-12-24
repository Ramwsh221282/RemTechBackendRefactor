CREATE SCHEMA IF NOT EXISTS parsers_control_module;

CREATE TABLE IF NOT EXISTS parsers_control_module.registered_parsers(
  id uuid primary key,
  type varchar(128) NOT NULL,
  domain varchar(128) NOT NULL,
  state varchar(128) NOT NULL,
  processed int not null,
  elapsed_seconds bigint not null,
  started_at timestamptz,
  finished_at timestamptz,
  next_run timestamptz,
  wait_days integer,
  UNIQUE (type, domain)
);

CREATE TABLE if not exists parsers_control_module.parser_links(
id uuid primary key,
parser_id uuid not null,
name varchar(255) not null,
url text not null,
CONSTRAINT fk_parser_id FOREIGN KEY(parser_id) REFERENCES parsers_control_module.registered_parsers(id),
UNIQUE (parser_id, name),
UNIQUE (parser_id, url)
);

CREATE INDEX IF NOT EXISTS idx_registered_parsers_type ON parsers_control_module.registered_parsers(type);
CREATE INDEX IF NOT EXISTS idx_registered_parsers_domain ON parsers_control_module.registered_parsers(domain);

CREATE TABLE IF NOT EXISTS parsers_control_module.links(
  id uuid primary key,
  name varchar(256) NOT NULL,
  url text NOT NULL,
  is_ignored boolean NOT NULL,
  parser_id uuid NOT NULL,
  UNIQUE (name, url, parser_id)
);