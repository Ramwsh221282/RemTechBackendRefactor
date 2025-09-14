export interface ParserWaitDaysUpdateResult {
  parserName: string;
  parserType: string;
  newWaitDays: number;
  nextRun: Date;
}
