export class StringUtils {
  public static isEmptyOrWhiteSpace(value: string): boolean {
    if (!value) return true;
    if (value.length === 0) return true;
    return value.trim().length === 0;
  }

  public static anyEmpty(values: string[]): boolean {
    for (const value of values) {
      if (value.trim().length === 0) return true;
    }
    return false;
  }

  public static allEmpty(values: string[]): boolean {
    for (const value of values) {
      if (!this.isEmptyOrWhiteSpace(value)) return false;
    }
    return true;
  }

  public static isEmailValid(value: string): boolean {
    const emailRegex = new RegExp(
      /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/,
    );
    return emailRegex.test(value);
  }

  public static isLessThan(value: string, length: number): boolean {
    return value.length < length;
  }

  public static isGreaterThan(value: string, length: number): boolean {
    return value.length > length;
  }
}
