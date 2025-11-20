CREATE SCHEMA IF NOT EXISTS tickets_module;

CREATE TABLE IF NOT EXISTS tickets_module.tickets
(
    id UUID primary key,
    creator_id UUID NOT NULL,
    type varchar(128) NOT NULL,
    created timestamptz NOT NULL,
    closed timestamptz,
    status varchar(128),
    extra_information jsonb
);