export interface TelemetryResponse {
	Id: string;
	UserId: string | null;
	UserLogin: string | null;
	UserEmail: string | null;
	UserPermissions: TelemetryPermissionResponse[] | null;
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
	Results: TelemetryResponse[];
}

export interface TelemetryPermissionResponse {
	Id: string;
	Name: string;
	Description: string;
}

export type IsoDateString = string;
