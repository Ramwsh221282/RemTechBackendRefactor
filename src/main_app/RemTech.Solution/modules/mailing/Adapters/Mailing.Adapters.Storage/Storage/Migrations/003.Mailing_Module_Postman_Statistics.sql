CREATE TABLE mailing_module.postman_statistics
(
    id            UUID PRIMARY KEY,
    current_sent  INT NOT NULL,
    current_limit INT NOT NULL
);