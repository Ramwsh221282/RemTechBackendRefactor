CREATE SCHEMA IF NOT EXISTS mailers_module;

CREATE TABLE IF NOT EXISTS mailers_module.mailers
(
    id UUID PRIMARY KEY,
    email VARCHAR(256) NOT NULL,
    smtp_password VARCHAR(512) NOT NULL,
    send_limit INTEGER NOT NULL,
    send_at_this_moment INTEGER NOT NULL,
    UNIQUE(email)
);