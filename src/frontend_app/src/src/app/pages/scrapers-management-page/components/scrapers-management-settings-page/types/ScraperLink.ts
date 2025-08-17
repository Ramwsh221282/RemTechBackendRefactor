export interface ScraperLink {
  name: string;
  parserName: string;
  parserType: string;
  url: string;
  activity: boolean;
  processed: number;
  totalSeconds: number;
  hours: number;
  minutes: number;
  seconds: number;
}
