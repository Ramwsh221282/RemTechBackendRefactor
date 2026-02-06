import { PreviousUserDetails } from './PreviousUserDetails';
import { UpdateUserDetails } from './UpdateUserDetails';

export type UpdateUserRequest = {
  previousDetails: PreviousUserDetails;
  updateUserDetails: UpdateUserDetails;
};
