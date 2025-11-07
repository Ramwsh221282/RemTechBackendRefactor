CREATE TABLE mailing_module.posted_messages
(
    id                UUID PRIMARY KEY,
    posted_by         UUID NOT NULL,
    body              VARCHAR(1024),
    subject           VARCHAR(255),
    recipient_address VARCHAR(255),
    created_on        TIMESTAMP,
    embedding         vector(1024),
    FOREIGN KEY (posted_by) REFERENCES mailing_module.postmans (id)
);

CREATE INDEX IF NOT EXISTS
    idx_posted_messages_subject
    ON
    mailing_module.posted_messages(subject);

CREATE INDEX IF NOT EXISTS
    idx_posted_messages_body
    ON
    mailing_module.posted_messages(body);

CREATE INDEX IF NOT EXISTS
    idx_posted_messages_recipient_address
    ON
    mailing_module.posted_messages(recipient_address);

CREATE INDEX IF NOT EXISTS
    idx_posted_messages_recipient_created_on
    ON
    mailing_module.posted_messages(created_on);

CREATE INDEX IF NOT EXISTS
    idx_posted_messages_embedding_hnsw
    ON
    mailing_module.posted_messages
    USING hnsw (embedding vector_cosine_ops);