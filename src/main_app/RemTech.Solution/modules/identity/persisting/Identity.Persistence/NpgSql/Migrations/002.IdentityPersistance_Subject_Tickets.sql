CREATE TABLE IF NOT EXISTS identity_module.subject_tickets
(
  id UUID PRIMARY KEY,
  creator_id UUID NOT NULL,
  type VARCHAR(128) not null,  
  active boolean,
  UNIQUE (creator_id, type),
  CONSTRAINT fk_subjects
    FOREIGN KEY (creator_id) REFERENCES identity_module.subjects(id)
        ON DELETE CASCADE ON UPDATE CASCADE
);