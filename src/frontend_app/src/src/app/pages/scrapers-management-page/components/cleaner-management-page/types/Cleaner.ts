export interface Cleaner {
  id: string;
  cleanedAmount: number;
  lastRun: Date;
  nextRun: Date;
  waitDays: number;
  state: string;
  hours: number;
  minutes: number;
  seconds: number;
  itemsThreshold: number;
}
