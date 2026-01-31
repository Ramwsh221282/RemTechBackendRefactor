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
	Items: TelemetryResponse[];
}

export interface TelemetryStatisticsResponse {
	Date: string;
	Amount: number;
}

export interface TelemetryPermissionResponse {
	Id: string;
	Name: string;
	Description: string;
}

export type IsoDateString = string;
