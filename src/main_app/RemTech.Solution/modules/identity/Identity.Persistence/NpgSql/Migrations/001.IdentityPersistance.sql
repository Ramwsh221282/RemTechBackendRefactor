CREATE SCHEMA IF NOT EXISTS identity_module;

CREATE TABLE IF NOT EXISTS identity_module.subjects
(
    id UUID PRIMARY KEY,
    email VARCHAR(256) NOT NULL,
    login VARCHAR(128) NOT NULL,
    password VARCHAR(512) NOT NULL,
    activation_date timestamptz,
    UNIQUE (email),
    UNIQUE (login)
);

CREATE TABLE IF NOT EXISTS identity_module.permissions
(
    id UUID PRIMARY KEY,
    name VARCHAR(128) NOT NULL,
    UNIQUE (name)
);

CREATE TABLE IF NOT EXISTS identity_module.subject_permissions
(
    subject_id uuid,
    permission_id uuid,    
    PRIMARY KEY (subject_id, permission_id),
    CONSTRAINT fk_subject_permissions_subject_id 
        FOREIGN KEY (subject_id) 
            REFERENCES identity_module.subjects(id) ON DELETE CASCADE  ON UPDATE CASCADE,
    CONSTRAINT fk_subject_permissions_permission_id 
        FOREIGN KEY (permission_id) 
            REFERENCES identity_module.permissions(id) ON DELETE CASCADE  ON UPDATE CASCADE
);