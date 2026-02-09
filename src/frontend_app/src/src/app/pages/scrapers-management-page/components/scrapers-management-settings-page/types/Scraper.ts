import { ScraperLink } from './ScraperLink';

export interface Scraper {
  name: string;
  type: string;
  state: string;
  domain: string;
  processed: number;
  totalSeconds: number;
  hours: number;
  minutes: number;
  seconds: number;
  waitDays: number;
  lastRun: Date;
  nextRun: Date;
  links: ScraperLink[];
}
