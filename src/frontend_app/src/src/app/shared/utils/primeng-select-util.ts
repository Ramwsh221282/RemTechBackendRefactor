import { SelectChangeEvent } from 'primeng/select';

export class PrimeNgSelectUtil {
  public static convertToObject<T>($event: SelectChangeEvent): T {
    const value = $event.value;
    return value as T;
  }
}
