export type ParserLinkResponse = {
  Id: string,
  IsActive: boolean,
  UrlName: string,
  UrlValue: string
}

export type ParserResponse = {
  Id: string,
  Domain: string,
  ServiceType: string,
  FinishedAt?: Date | null,
  StartedAt?: Date | null,
  NextRun?: Date | null,
  WaitDays?: number | null,
  State: string,
  ParsedCount: number,
  ElapsedHours: number,
  ElapsedSeconds: number,
  ElapsedMinutes: number,
  Links: ParserLinkResponse[]
}
