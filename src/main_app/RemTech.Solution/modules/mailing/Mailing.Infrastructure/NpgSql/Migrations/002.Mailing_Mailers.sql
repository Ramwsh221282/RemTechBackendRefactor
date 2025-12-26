CREATE SCHEMA IF NOT EXISTS mailing_module;

CREATE TABLE mailing_module.mailers
(
  id uuid primary key,
  hashed_password varchar(256) NOT NULL,
  service varchar(128) NOT NULL,
  email varchar(256) NOT NULL,
  send_limit INT NOT NULL,
  send_current INT NOT NULL    
);