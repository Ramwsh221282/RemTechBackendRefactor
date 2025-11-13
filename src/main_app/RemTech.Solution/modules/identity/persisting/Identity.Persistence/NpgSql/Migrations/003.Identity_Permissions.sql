CREATE TABLE IF NOT EXISTS identity_module.permissions
(
    id uuid primary key,
    name varchar(128) unique
);