export interface ScraperJournalResponse {
  id: string;
  name: string;
  type: string;
  createdAt: Date;
  completedAt: Date | null;
}
