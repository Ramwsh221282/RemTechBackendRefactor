export type CustomHttpError = {
  code: number;
  message: string;
};

export class CustomHttpErrorFactory {
  public static AsHttpError(err: any): CustomHttpError {
    try {
      const code: number = err.error.code as number;
      const message: string = err.error.statusInfo as string;
      return { code: code, message: message };
    } catch {
      return { code: 500, message: 'Ошибка на уровне приложения.' };
    }
  }
}
