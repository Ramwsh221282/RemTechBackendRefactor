CREATE SCHEMA IF NOT EXISTS contained_items;

CREATE TABLE IF NOT EXISTS contained_items.cleaners(
    id      UUID NOT NULL,
    cleaned_amount BIGINT NOT NULL,
    last_run    DATE NOT NULL,
    next_run    DATE NOT NULL,
    wait_days INTEGER NOT NULL,
    state VARCHAR(50) NOT NULL,
    PRIMARY KEY(id)
);

CREATE INDEX IF NOT EXISTS idx_contained_cleaners_next_run
    ON contained_items.cleaners(next_run);

CREATE TABLE IF NOT EXISTS contained_items.items(
    id          VARCHAR(100) NOT NULL,
    type        VARCHAR(20) NOT NULL,
    domain      VARCHAR(20) NOT NULL,
    created_at  DATE NOT NULL,
    is_deleted  BOOLEAN NOT NULL,
    source_url  TEXT NOT NULL UNIQUE,
    PRIMARY KEY(id)
);

CREATE INDEX IF NOT EXISTS idx_contained_items_created_at 
    ON contained_items.items(created_at);

CREATE INDEX IF NOT EXISTS idx_contained_items_id
    ON contained_items.items(id);