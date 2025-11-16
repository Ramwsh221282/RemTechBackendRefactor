CREATE TABLE IF NOT EXISTS identity_module.tickets
(
    id UUID primary key,
    creator_id UUID NOT NULL,
    type varchar(128) NOT NULL,
    created timestamptz NOT NULL,
    closed timestamptz,
    active boolean
);