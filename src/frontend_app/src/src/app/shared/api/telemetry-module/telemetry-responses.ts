export interface TelemetryResponse {
	Id: string;
	UserId: string | null;
	UserLogin: string | null;
	UserEmail: string | null;
	UserPermissions: string[] | null;
	ActionName: string;
	ActionSeverity: string;
	ActionTimestamp: IsoDateString;
	ErrorMessage: string | null;
}

export interface FetchTelemetryRecordsResponse {
	TotalCount: number;
	MaxPage: number;
	PagesCount: number;
	PageNumber: number;
	PageSize: number;
	HasNextPage: boolean;
	HasPreviousPage: boolean;
	Items: FetchAnalyticsTelemetryRecordsResponse[];
}

export interface FetchAnalyticsTelemetryRecordsResponse {
	DateByDay: string;
	Results: FetchTelemetryRecordsResponse[];
}

export type IsoDateString = string;
