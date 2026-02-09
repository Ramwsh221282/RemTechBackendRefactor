export type Sorting = {
  mode: string;
};

export class SortingFactory {
  public static default(): Sorting {
    return { mode: 'NONE' };
  }
}
