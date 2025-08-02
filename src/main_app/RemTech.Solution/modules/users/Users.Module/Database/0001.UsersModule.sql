CREATE SCHEMA IF NOT EXISTS users_module;

CREATE TABLE IF NOT EXISTS users_module.users(
    id      UUID PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE,
    password VARCHAR(100) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    email_confirmed BOOLEAN NOT NULL  
);

CREATE TABLE IF NOT EXISTS users_module.roles(
    id     UUID PRIMARY KEY,
    name   VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS users_module.user_roles(
    user_id UUID,
    role_id UUID,
    PRIMARY KEY(user_id, role_id),
    FOREIGN KEY(user_id) REFERENCES users_module.users(id),
    FOREIGN KEY(role_id) REFERENCES users_module.roles(id)
);

CREATE INDEX IF NOT EXISTS idx_users_module_user_nickName ON users_module.users(name);
CREATE INDEX IF NOT EXISTS idx_users_module_user_email ON users_module.users(email);
CREATE INDEX IF NOT EXISTS idx_users_module_roles_name ON users_module.roles(name);
CREATE INDEX IF NOT EXISTS idx_users_module_user_roles_user_id ON users_module.user_roles(user_id);
CREATE INDEX IF NOT EXISTS idx_users_module_user_roles_role_id ON users_module.user_roles(role_id);
