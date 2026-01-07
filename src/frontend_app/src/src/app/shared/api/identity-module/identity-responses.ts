export type AccountPermissionsResponse = {
  id: string;
  name: string;
  description: string;
}

export type AccountResponse = {
  id: string;
  login: string;
  email: string;
  isActivated: boolean;
  permissions: AccountPermissionsResponse[];
}
