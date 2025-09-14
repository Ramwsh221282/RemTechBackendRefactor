export class ArrayUtils {
  public static removeItem<T>(array: T[], compareFn: (arg1: T) => boolean): T[] {
    const index: number = array.findIndex(value => compareFn(value));
    if (index === -1)  return array;
    const newArray = [...array];
    newArray.splice(index, 1);
    return newArray;
  }

  public static addItem<T>(item: T, array: T[]): T[] {
    return [item, ...array];
  }

  public static updateItemByIndex<T>(updated: T, array: T[], compareFn: (arg: T) => boolean): T[] {
    const index: number = array.findIndex(value => compareFn(value));
    if (index === -1)  return array;
    const newArray = [...array];
    newArray[index] = updated;
    return newArray;
  }

  public static getItem<T>(array: T[], compareFn: (arg: T) => boolean): T | null {
    const index: number = array.findIndex(value => compareFn(value));
    if (index === -1)  return null;
    return array[index];
  }
}
