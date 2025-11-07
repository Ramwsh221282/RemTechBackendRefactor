CREATE SCHEMA IF NOT EXISTS mailing_module;

CREATE TABLE IF NOT EXISTS mailing_module.senders(
    name    VARCHAR(20),
    email   VARCHAR(50),
    key     VARCHAR(20),
    PRIMARY KEY(name),
    UNIQUE(email)
);

CREATE INDEX IF NOT EXISTS idx_mailing_module_senders_name ON mailing_module.senders(name);

CREATE INDEX IF NOT EXISTS idx_mailing_module_senders_email ON mailing_module.senders(email);