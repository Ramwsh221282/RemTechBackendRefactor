import { TelemetryStatisticsResponse, PaginatedTelemetryRecordsResponse, TelemetryResponse } from './telemetry-responses';

export function createDefaultTelemetryResponse(): TelemetryResponse {
	return {
		Id: '',
		ActionName: '',
		ActionSeverity: '',
		ErrorMessage: null,
		UserEmail: null,
		UserId: null,
		UserLogin: null,
		UserPermissions: null,
		ActionTimestamp: new Date(0).toLocaleDateString('ru-Ru'),
	};
}

export function createDefaultFetchTelemetryRecordsFetchResponse(): PaginatedTelemetryRecordsResponse {
	return {
		HasNextPage: false,
		HasPreviousPage: false,
		MaxPage: 0,
		PageNumber: 0,
		PageSize: 0,
		TotalCount: 0,
		Items: [],
		PagesCount: 0,
	};
}

export function createDefaultFetchAnalyticsTelemetryRecordsResponse(): TelemetryStatisticsResponse {
	return {
		Date: new Date(0).toLocaleDateString('ru-Ru'),
		Amount: 0,
	};
}
