import { DatePipe, NgClass } from '@angular/common';
import { Component, computed, effect, inject, OnInit, signal, WritableSignal } from '@angular/core';
import { ChartModule } from 'primeng/chart';
import { TableModule } from 'primeng/table';
import { FormsModule } from '@angular/forms';
import { Checkbox } from 'primeng/checkbox';
import { PaginationComponent } from '../../../../shared/components/pagination/pagination.component';
import {
	TelemetryStatisticsResponse,
	TelemetryResponse,
	TelemetryPermissionResponse,
	ActionRecordsPageResponse,
	TelemetryActionStatus,
} from '../../../../shared/api/telemetry-module/telemetry-responses';
import { TelemetryApiService } from '../../../../shared/api/telemetry-module/telemetry-api.service';
import { ActionRecordsQuery } from '../../../../shared/api/telemetry-module/telemetry-fetch.request';
import { tap } from 'rxjs';
import { SortButtonsComponent, SortChangeEvent } from '../../../../shared/components/sort-buttons/sort-buttons.component';
import { ActionRecordInvokerLoginSearchInputComponent } from './components/action-record-invoker-login-search-input/action-record-invoker-login-search-input.component';
import { ActionRecordSelectFilterComponent } from './components/action-record-select-filter/action-record-select-filter.component';
import { ActionRecordsMultiselectFilterComponent } from './components/action-records-multiselect-filter/action-records-multiselect-filter.component';
import { ActionRecordDateRangeSelectComponent } from './components/action-record-date-range-select/action-record-date-range-select.component';
import { AuthenticationStatusService } from '../../../../shared/services/AuthenticationStatusService';

@Component({
	selector: 'app-users-activity-monitoring-page',
	imports: [
		FormsModule,
		NgClass,
		ChartModule,
		TableModule,
		Checkbox,
		DatePipe,
		PaginationComponent,
		SortButtonsComponent,
		ActionRecordInvokerLoginSearchInputComponent,
		ActionRecordSelectFilterComponent,
		ActionRecordsMultiselectFilterComponent,
		ActionRecordDateRangeSelectComponent,
		FormsModule,
	],
	templateUrl: './users-activity-monitoring-page.component.html',
	styleUrl: './users-activity-monitoring-page.component.css',
})
export class UsersActivityMonitoringPageComponent implements OnInit {
	selectedPeriod: any;
	customRange: [Date, Date] | undefined;

	private readonly _authService: AuthenticationStatusService = inject(AuthenticationStatusService);
	private readonly _service: TelemetryApiService = inject(TelemetryApiService);
	readonly query: WritableSignal<ActionRecordsQuery> = signal(ActionRecordsQuery.create().withPage(1).withPageSize(50));

	readonly usingWeekRangeForChart: WritableSignal<boolean> = signal(true);
	readonly ignoringRequestInvokerId: WritableSignal<boolean> = signal(false);
	readonly ignoringAnonymous: WritableSignal<boolean> = signal(false);
	readonly timeSelections: string[] = ['7 дней', 'месяц', 'диапазон'];
	readonly selectedTimeOption: WritableSignal<string | null> = signal('7 дней');
	readonly rangedFilterDates: WritableSignal<Date[] | undefined> = signal(undefined);
	readonly statistics: WritableSignal<TelemetryStatisticsResponse[]> = signal([]);
	readonly permissions: WritableSignal<TelemetryPermissionResponse[]> = signal([]);
	readonly statuses: WritableSignal<TelemetryActionStatus[]> = signal([]);
	readonly records: WritableSignal<TelemetryResponse[]> = signal([]);
	readonly totalCount: WritableSignal<number> = signal(0);

	readonly queryChangeEffect = effect((): void => {
		const query: ActionRecordsQuery = this.query();
		this.fetchRecords(query);
	});

	readonly chartData = computed(() => {
		const items: TelemetryStatisticsResponse[] = this.statistics();

		const dataSet: PrimeNgChartDataSet = {
			label: 'Активность',
			backgroundColor: '#3b82f6',
			borderColor: '#2563eb',
			borderWidth: 1,
			data: items.map((item: TelemetryStatisticsResponse) => item.Amount),
			type: 'line',
		};

		const data: PrimeNgChartData = {
			...defaultPrimeNgChartData(),
			labels: items.map((item: TelemetryStatisticsResponse) => new Date(item.Date).toLocaleDateString('ru-RU')),
			datasets: [dataSet],
		};

		return data;
	});

	readonly chartOptions = computed(() => {
		const options: PrimeNgChartOptions = {
			responsive: true,
			maintainAspectRatio: true,
			tooltip: {
				enabled: true,
				callbacks: {
					label: (context: any): string => `Значение: ${context.parsed.y}`,
				},
			},
			legend: {
				display: true,
				position: 'top',
				labels: {
					color: '#333',
					font: { size: 14 },
				},
			},
			layout: {
				padding: 1,
			},
			plugins: {
				legend: {
					display: false,
				},
				title: {
					display: true,
					text: 'Активность пользователей',
				},
			},
			scales: {
				x: {
					title: {
						display: true,
						text: 'Дата',
						color: '#ffffff',
						font: { size: 16 },
					},
				},
				y: {
					beginAtZero: true,
					title: {
						display: true,
						text: 'Количество действий',
						color: '#ffffff',
						font: { size: 16 },
						padding: 10,
					},
					ticks: {
						precision: 0,
					},
				},
			},
		};
		return options;
	});

	public handleDateSortSelected($event: SortChangeEvent): void {
		this.query.update((query: ActionRecordsQuery): ActionRecordsQuery => {
			return query.addSort($event);
		});
	}

	public handleEmailSearchChanged($event: string | null): void {
		this.query.update((query: ActionRecordsQuery): ActionRecordsQuery => {
			return query.withEmail($event);
		});
	}

	public handleLoginSearchChanged($event: string | null): void {
		this.query.update((query: ActionRecordsQuery): ActionRecordsQuery => {
			return query.withLogin($event);
		});
	}

	public handleActionNameSearchChanged($event: string | null): void {
		this.query.update((query: ActionRecordsQuery): ActionRecordsQuery => {
			return query.withActionNameSearch($event);
		});
	}

	public handleOperationStatusSelectChange($event: TelemetryActionStatus | null): void {
		const status: string | null = !!$event ? $event.Status : null;
		this.query.update((query: ActionRecordsQuery): ActionRecordsQuery => {
			return query.withStatus(status);
		});
	}

	public handlePermissionSelected($event: TelemetryPermissionResponse[] | null): void {
		this.query.update((query: ActionRecordsQuery): ActionRecordsQuery => {
			return query.withPermissions($event ? $event.map((perm: TelemetryPermissionResponse) => perm.Id) : null);
		});
	}

	public permissionDisplayText(record: TelemetryResponse): string[] {
		const permissions: TelemetryPermissionResponse[] | null = record.UserPermissions;
		if (!permissions || permissions.length === 0) {
			return ['Нет прав / аноним'];
		}
		return permissions.map((perm: TelemetryPermissionResponse) => perm.Description);
	}

	public handleDateRangeSelect($event: Date[] | null): void {
		this.query.update((query: ActionRecordsQuery): ActionRecordsQuery => {
			return query.withDateRange($event);
		});
	}

	public resolveInvokerIdDisplayValue(record: TelemetryResponse): string {
		return record.UserId ? record.UserId : 'Аноним (гость)';
	}

	public ngOnInit(): void {
		const query: ActionRecordsQuery = this.query();
		this.fetchRecords(query);
	}

	public handleChartDateRangeSelected(date: Date[] | null): void {
		this.query.update((query: ActionRecordsQuery): ActionRecordsQuery => {
			return query.withoutChartWeekDateRange().withChartDateRange(date);
		});
	}

	public handleChartWeekDateRangeSelected(): void {
		const usingWeekRangeForChart: boolean = this.usingWeekRangeForChart();
		if (usingWeekRangeForChart) {
			this.query.update((query: ActionRecordsQuery): ActionRecordsQuery => {
				return query.withChartWeekDateRange().withChartDateRange(null);
			});
		}
	}

	public handleChangeIgnoreInvokerId(): void {
		const isIgnoring: boolean = this.ignoringRequestInvokerId();
		if (isIgnoring) {
			const id: string | null = this._authService.userId();
			if (!id) return;
			this.query.update((query: ActionRecordsQuery): ActionRecordsQuery => {
				return query.withCallerId(id);
			});
			return;
		}

		this.query.update((query: ActionRecordsQuery): ActionRecordsQuery => {
			return query.withCallerId(null);
		});
	}

	public handleAnonymousIgnore(): void {
		const isIgnoring: boolean = this.ignoringAnonymous();
		this.query.update((query: ActionRecordsQuery): ActionRecordsQuery => {
			return query.withAnonymousIgnore(isIgnoring ? true : null);
		});
	}

	public get canSelectCustomDateRangeForChart(): boolean {
		const isWeekSelected: boolean = this.usingWeekRangeForChart();
		return !isWeekSelected;
	}

	public handlePageChanged(page: number): void {
		this.query.update((query: ActionRecordsQuery): ActionRecordsQuery => {
			return query.withPage(page);
		});
	}

	private fetchRecords(query: ActionRecordsQuery): void {
		this._service
			.fetchTelemetryPageInfo(query)
			.pipe(
				tap((response: ActionRecordsPageResponse): void => {
					const records: TelemetryResponse[] = response.Records.Items;
					const statistics: TelemetryStatisticsResponse[] = response.Statistics;
					const totalCount: number = response.Records.TotalCount;
					const statuses: TelemetryActionStatus[] | null = response.Statuses;
					const permissions: TelemetryPermissionResponse[] | null = response.Permissions;
					this.records.set(records);
					this.statistics.set(statistics);
					this.totalCount.set(totalCount);
					if (statuses) this.statuses.set(statuses);
					if (permissions) this.permissions.set(permissions);
				}),
			)
			.subscribe();
	}
}

interface PrimeNgChartData {
	labels: string[];
	datasets: PrimeNgChartDataSet[];
}

function defaultPrimeNgChartData(): PrimeNgChartData {
	return { labels: [], datasets: [] };
}

function defaultPrimeNgChartDataSet(): PrimeNgChartDataSet {
	return { label: '', backgroundColor: '', borderColor: '', borderWidth: 1, data: [], type: 'line' };
}

interface PrimeNgChartDataSet {
	label: string;
	backgroundColor: string;
	borderColor: string;
	borderWidth: number;
	data: number[];
	type: string;
}

interface PrimeNgChartOptions {
	responsive: boolean;
	maintainAspectRatio: boolean;
	tooltip: {
		enabled: boolean;
		callbacks: {
			label: (context: any) => string;
		};
	};
	legend: {
		display: boolean;
		position: 'top' | 'bottom' | 'left' | 'right';
		labels: {
			color: string;
			font: { size: number };
		};
	};
	layout: {
		padding: number;
	};
	plugins: {
		legend: {
			display: boolean;
		};
		title: {
			display: boolean;
			text: string;
		};
	};
	scales: {
		x: {
			title: {
				display: boolean;
				text: string;
				color: string;
				font: { size: number };
			};
		};
		y: {
			beginAtZero: boolean;
			title: {
				display: boolean;
				text: string;
				color: string;
				font: { size: number };
				padding: number;
			};
			ticks: {
				precision: number;
			};
		};
	};
}
