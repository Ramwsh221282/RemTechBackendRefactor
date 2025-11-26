CREATE TABLE identity_module.account_tickets(
    id uuid primary key,
    account_id uuid not null,
    type varchar(256) not null,
    payload jsonb not null,
    created timestamptz not null,
    finished timestamptz,
    UNIQUE(account_id, type)
)