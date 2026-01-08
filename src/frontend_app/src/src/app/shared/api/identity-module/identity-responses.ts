export type AccountPermissionsResponse = {
  Id: string;
  Name: string;
  Description: string;
}

export type AccountResponse = {
  Id: string;
  Login: string;
  Email: string;
  IsActivated: boolean;
  Permissions: AccountPermissionsResponse[];
}
