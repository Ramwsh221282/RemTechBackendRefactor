import { FetchAnalyticsTelemetryRecordsResponse, FetchTelemetryRecordsResponse, TelemetryResponse } from './telemetry-responses';

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

export function createDefaultFetchTelemetryRecordsFetchResponse(): FetchTelemetryRecordsResponse {
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

export function createDefaultFetchAnalyticsTelemetryRecordsResponse(): FetchAnalyticsTelemetryRecordsResponse {
	return {
		DateByDay: new Date(0).toLocaleDateString('ru-Ru'),
		Results: [],
	};
}
