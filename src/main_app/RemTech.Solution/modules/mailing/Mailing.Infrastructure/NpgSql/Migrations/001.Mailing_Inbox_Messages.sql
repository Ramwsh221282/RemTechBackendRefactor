CREATE SCHEMA IF NOT EXISTS mailing_module;

CREATE TABLE mailing_module.inbox_messages
(
  id uuid primary key,
  recipient_email varchar(256) NOT NULL,
  subject varchar(128) NOT NULL,
  body varchar(512) NOT NULL    
);