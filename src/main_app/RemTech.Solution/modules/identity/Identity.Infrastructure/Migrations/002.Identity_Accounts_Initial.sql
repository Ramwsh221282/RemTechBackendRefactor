CREATE TABLE IF NOT EXISTS identity_module.accounts(
    id uuid primary key,
    name varchar(128) not null,
    email varchar(256) not null,
    password TEXT not null,
    activated boolean
);