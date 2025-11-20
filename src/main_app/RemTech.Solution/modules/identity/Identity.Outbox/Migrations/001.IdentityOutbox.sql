CREATE TABLE IF NOT EXISTS identity_module.outbox
(
    id UUID PRIMARY KEY,
    type VARCHAR(255) NOT NULL,
    body jsonb NOT NULL,
    queue VARCHAR(255) NOT NULL,
    exchange VARCHAR(255) NOT NULL,
    routing_key VARCHAR(255) NOT NULL,
    created_at timestamptz NOT NULL,
    processed_at timestamptz,
    retry_count INT NOT NULL
);

CREATE INDEX idx_outbox_processed_at ON identity_module.outbox(processed_at) WHERE processed_at IS NULL;
CREATE INDEX idx_outbox_retry_count ON identity_module.outbox(retry_count);
CREATE INDEX idx_outbox_retry_count_processed_at ON identity_module.outbox(retry_count, processed_at) WHERE processed_at IS NULL;