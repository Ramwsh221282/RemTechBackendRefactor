CREATE TABLE IF NOT EXISTS identity_module.outbox(
    id uuid primary key,
    type varchar(256),
    payload jsonb,
    created timestamptz,
    was_sent boolean
);