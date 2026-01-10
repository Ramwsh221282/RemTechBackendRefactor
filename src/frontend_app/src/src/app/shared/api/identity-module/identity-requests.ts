export type AuthenticateRequest = {
  password: string;
  email?: string | null | undefined;
  login?: string | null | undefined;
}

export type GivePermissionsRequest = {
  permissionIds: string[]
}

export type RegisterAccountRequest = {
  email: string;
  login: string;
  password: string;
}

export type ChangePasswordRequest = {
  NewPassword: string;
  CurrentPassword: string;
}
