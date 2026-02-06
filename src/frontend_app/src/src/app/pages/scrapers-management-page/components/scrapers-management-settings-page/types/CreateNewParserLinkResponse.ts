export interface CreateNewParserLinkResponse {
  name: string;
  url: string;
  parserName: string;
  parserType: string;
  processedAmount: number;
  totalElapsedSeconds: number;
  elapsedHours: number;
  elapsedMinutes: number;
  elapsedSeconds: number;
  activity: boolean;
}
