CREATE SCHEMA IF NOT EXISTS identity_module;

CREATE TABLE IF NOT EXISTS identity_module.subjects
(
    id UUID PRIMARY KEY,
    email VARCHAR(256) NOT NULL,
    login VARCHAR(128) NOT NULL,
    password VARCHAR(512) NOT NULL,
    isActive BOOLEAN NOT NULL,
)
