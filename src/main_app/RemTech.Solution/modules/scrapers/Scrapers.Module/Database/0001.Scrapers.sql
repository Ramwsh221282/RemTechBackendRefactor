CREATE SCHEMA IF NOT EXISTS scrapers_module;

CREATE TABLE IF NOT EXISTS scrapers_module.scrapers (    
    name            varchar(20) NOT NULL,
    type            varchar(20) NOT NULL,
    state           varchar(20) NOT NULL,
    domain          varchar(20) NOT NULL,
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

CREATE TABLE IF NOT EXISTS scrapers_module.scraper_links (
    name            VARCHAR(75) NOT NULL,
    parser_name     VARCHAR(20) NOT NULL,        
    url             text NOT NULL,
    activity        boolean NOT NULL,
    processed       integer NOT NULL,
    total_seconds   bigint NOT NULL,
    hours           integer NOT NULL,
    minutes         integer NOT NULL,
    seconds         integer NOT NULL,
    PRIMARY KEY(name, parser_name),
    FOREIGN KEY(parser_name) REFERENCES scrapers_module.scrapers(name) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS scrapers_module.scraper_links (
    id              VARCHAR(100),
    source_url      TEXT,
    source_domain   VARCHAR(20),    
    PRIMARY KEY (id)
);

CREATE INDEX IF NOT EXISTS idx_parsers_name
    ON scrapers_module.scrapers(name);

CREATE INDEX IF NOT EXISTS idx_parser_links_name
    ON scrapers_module.scraper_links(name);

CREATE INDEX IF NOT EXISTS idx_parser_links_parser_name
    ON scrapers_module.scraper_links(parser_name);
