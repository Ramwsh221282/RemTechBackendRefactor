CREATE SCHEMA IF NOT EXISTS parsers_management_module;

CREATE TABLE IF NOT EXISTS parsers_management_module.parsers (
    id              UUID PRIMARY KEY,
    name            varchar(150) NOT NULL,
    type            varchar(100) NOT NULL,
    state           varchar(100) NOT NULL,
    domain          varchar(100) NOT NULL,
    processed       integer NOT NULL,
    total_seconds   bigint NOT NULL,
    hours           integer NOT NULL,
    minutes         integer NOT NULL,
    seconds         integer NOT NULL,
    wait_days       integer NOT NULL,
    next_run        DATE NOT NULL,
    last_run        DATE NOT NULL,
    UNIQUE (name, type),
    UNIQUE (domain, type)
);

CREATE TABLE IF NOT EXISTS parsers_management_module.parser_links (
    id              UUID PRIMARY KEY,
    parser_id       UUID NOT NULL REFERENCES parsers_management_module.parsers(id) ON DELETE CASCADE,
    name            varchar(150) NOT NULL,
    url             text NOT NULL,
    activity        boolean NOT NULL,
    processed       integer NOT NULL,
    total_seconds   bigint NOT NULL,
    hours           integer NOT NULL,
    minutes         integer NOT NULL,
    seconds         integer NOT NULL,
    UNIQUE (parser_id, name)
);

CREATE INDEX idx_parsers_name
    ON parsers_management_module.parsers(name);

CREATE INDEX idx_parser_links_name
    ON parsers_management_module.parser_links(name);

CREATE SCHEMA IF NOT EXISTS shared_advertisements_module;

CREATE TABLE IF NOT EXISTS shared_advertisements_module.contained_items (
    id              VARCHAR(50),
    source_id       UUID NOT NULL REFERENCES parsers_management_module.parser_links(id) ON DELETE SET NULL,
    date_created    DATE NOT NULL,
    is_new          BOOLEAN NOT NULL,
    PRIMARY KEY (id),
    UNIQUE  (source_id)
);
