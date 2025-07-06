CREATE TABLE IF NOT EXISTS parsers (
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

CREATE TABLE IF NOT EXISTS parser_links (
    id              UUID PRIMARY KEY,
    parser_id       UUID NOT NULL REFERENCES parsers(id) ON DELETE CASCADE,
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

CREATE INDEX idx_parsers_name ON parsers(name);

CREATE INDEX idx_parser_links_name ON parser_links(name);
