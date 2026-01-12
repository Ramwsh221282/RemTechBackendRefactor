import { AccountResponse } from './identity-responses';
import {
  ChangePasswordRequest,
  CommitPasswordResetRequest,
  ResetPasswordRequest,
} from './identity-requests';
import { StringUtils } from '../../utils/string-utils';

export const DefaultAccountResponse = (): AccountResponse => {
  return { Id: '', Email: '', Login: '', IsActivated: false, Permissions: [] };
};

export const CreateChangePasswordRequest = (
  newPassword: string,
  currentPassword: string
): ChangePasswordRequest => {
  return { NewPassword: newPassword, CurrentPassword: currentPassword };
};

export const CreateResetPasswordRequest = (
  login?: string | null | undefined,
  email?: string | null | undefined
): ResetPasswordRequest => {
  const requestLogin: string | null = !!login
    ? StringUtils.isEmptyOrWhiteSpace(login)
      ? null
      : login
    : null;

  const requestEmail: string | null = !!email
    ? StringUtils.isEmptyOrWhiteSpace(email)
      ? null
      : email
    : null;

  return { Login: requestLogin, Email: requestEmail };
};

export const CreateCommitPasswordResetRequest = (
  newPassword: string
): CommitPasswordResetRequest => {
  return { NewPassword: newPassword };
};
