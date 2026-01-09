import {ParserLinkResponse, ParserResponse} from './parsers-responses';

export const DefaultParserResponse = (): ParserResponse => {
  return {
    Domain: '',
    ServiceType: '',
    Id: '',
    FinishedAt: null,
    State: '',
    ParsedCount: 0,
    Links: [],
    ElapsedHours: 0,
    ElapsedMinutes: 0,
    ElapsedSeconds: 0,
    NextRun: null,
    StartedAt: null,
    WaitDays: null }
}

export const DefaultParserLinkResponse = (): ParserLinkResponse => {
  return { Id: '', IsActive: false, UrlName: '', UrlValue: '' }
}

export const NewParserLink = (name: string, url: string): ParserLinkResponse => {
  return { Id: '', IsActive: true, UrlName: name, UrlValue: url }
}
