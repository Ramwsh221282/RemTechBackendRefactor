export interface ScraperJournalRecordResponse {
  id: string;
  journalId: string;
  action: string;
  text: string;
  createdAt: Date;
}
