import { HttpErrorResponse } from '@angular/common/http';

export type EnvelopeError = {
  code: number;
  statusInfo: string;
};

export class EnvelopeErrorFactory {
  public static fromHttpErrorResponse(error: HttpErrorResponse): EnvelopeError {
    try {
      const code = error.error.code as number;
      const statusInfo = error.error.statusInfo as string;
      return { code, statusInfo };
    } catch {
      return { code: 500, statusInfo: 'Ошибка на уровне приложения.' };
    }
  }
}
