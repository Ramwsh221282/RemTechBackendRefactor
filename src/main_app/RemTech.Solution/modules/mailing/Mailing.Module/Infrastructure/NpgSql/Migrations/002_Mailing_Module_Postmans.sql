CREATE TABLE mailing_module.postmans
(
    id            UUID PRIMARY KEY,
    email         varchar(255) not null UNIQUE,
    password      varchar(512) not null,
    current_sent  INT          NOT NULL,
    current_limit INT          NOT NULL
);