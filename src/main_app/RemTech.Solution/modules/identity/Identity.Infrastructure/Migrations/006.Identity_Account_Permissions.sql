CREATE TABLE IF NOT EXISTS identity_module.account_permissions(
  account_id uuid,
  permission_id uuid,
  PRIMARY KEY(account_id, permission_id)  
);