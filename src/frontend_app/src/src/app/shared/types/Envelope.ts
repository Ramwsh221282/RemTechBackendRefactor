export type Envelope<T> = {
  code: number;
  statusInfo: string;
  data: T;
};
