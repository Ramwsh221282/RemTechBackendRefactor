CREATE SCHEMA IF NOT EXISTS cleaners_module;

CREATE TABLE IF NOT EXISTS cleaners_module.cleaners(
id              UUID NOT NULL,
cleaned_amount  BIGINT NOT NULL,
last_run        DATE NOT NULL,
next_run        DATE NOT NULL,
wait_days       INTEGER NOT NULL,
state           VARCHAR(50) NOT NULL,
hours           INTEGER NOT NULL,
minutes         INTEGER NOT NULL,
seconds         INTEGER NOT NULL,
items_date_day_threshold INTEGER NOT NULL,
PRIMARY KEY(id)
);

CREATE INDEX IF NOT EXISTS idx_contained_cleaners_next_run
    ON cleaners_module.cleaners(next_run);