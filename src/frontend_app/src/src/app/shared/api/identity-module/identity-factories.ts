import {AccountResponse} from './identity-responses';
import {ChangePasswordRequest} from './identity-requests';

export const DefaultAccountResponse = (): AccountResponse => {
  return { Id: '', Email: '', Login: '', IsActivated: false, Permissions: [] };
}

export const CreateChangePasswordRequest = (newPassword: string, currentPassword: string): ChangePasswordRequest => {
  return { NewPassword: newPassword, CurrentPassword: currentPassword };
}
