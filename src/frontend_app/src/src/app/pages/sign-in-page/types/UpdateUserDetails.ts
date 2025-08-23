export type UpdateUserDetails = {
  newUserEmail: string | null;
  newUserName: string | null;
  newUserRole: string | null;
  isPasswordUpdateRequired: boolean;
};
