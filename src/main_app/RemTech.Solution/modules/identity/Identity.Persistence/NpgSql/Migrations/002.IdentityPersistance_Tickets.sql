CREATE TABLE IF NOT EXISTS identity_module.tickets
(
  id UUID PRIMARY KEY,
  creator_id UUID,
  type VARCHAR(128) not null,
  created timestamp not null,
  closed timestamptz,
  active boolean,
  CONSTRAINT fk_subjects
    FOREIGN KEY (creator_id) REFERENCES identity_module.subjects(id)
        ON DELETE CASCADE ON UPDATE CASCADE
);