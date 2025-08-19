CREATE TABLE IF NOT EXISTS scrapers_module.scraper_journals (
    id                  UUID NOT NULL,
    parser_name         VARCHAR(20) NOT NULL,
    parser_type         VARCHAR(20) NOT NULL,
    created_at          DATE NOT NULL,
    completed_at        DATE,
    PRIMARY KEY (id),
    FOREIGN KEY(parser_name, parser_type) REFERENCES scrapers_module.scrapers(name, type) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS scrapers_module.journal_records (
    id                  UUID NOT NULL,
    journal_id          UUID NOT NULL,
    action              VARCHAR(50) NOT NULL,
    text                TEXT NOT NULL,
    created_at          DATE NOT NULL,
    embedding           vector(1024) NOT NULL,    
    PRIMARY KEY (id),
    FOREIGN KEY(journal_id) REFERENCES scrapers_module.scraper_journals(id) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS idx_scraper_journals_records_hnsw
    ON scrapers_module.journal_records USING hnsw (embedding vector_cosine_ops);

CREATE INDEX IF NOT EXISTS idx_scraper_journals_action
    ON scrapers_module.journal_records(action);

CREATE INDEX IF NOT EXISTS idx_scraper_journal_created_at
    ON scrapers_module.scraper_journals(created_at);

CREATE INDEX IF NOT EXISTS idx_scraper_journal_record_created_at
    ON scrapers_module.journal_records(created_at);

CREATE INDEX IF NOT EXISTS idx_scraper_journals_scraper_name_type
    ON scrapers_module.scraper_journals(parser_name, parser_type);