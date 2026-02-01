import { DatePipe, NgClass } from '@angular/common';
import { Component, computed, effect, inject, OnInit, signal, WritableSignal } from '@angular/core';
import { ChartModule } from 'primeng/chart';
import { TableModule } from 'primeng/table';
import { Select } from 'primeng/select';
import { InputText } from 'primeng/inputtext';
import { DatePicker } from 'primeng/datepicker';
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
import { MultiSelect, MultiSelectChangeEvent } from 'primeng/multiselect';
import { ActionRecordsQuery } from '../../../../shared/api/telemetry-module/telemetry-fetch.request';
import { tap } from 'rxjs';
import { Button } from 'primeng/button';
import { SortButtonsComponent, SortChangeEvent } from '../../../../shared/components/sort-buttons/sort-buttons.component';

@Component({
	selector: 'app-users-activity-monitoring-page',
	imports: [
		FormsModule,
		NgClass,
		ChartModule,
		TableModule,
		Select,
		InputText,
		DatePicker,
		Checkbox,
		DatePipe,
		PaginationComponent,
		MultiSelect,
		SortButtonsComponent,
	],
	templateUrl: './users-activity-monitoring-page.component.html',
	styleUrl: './users-activity-monitoring-page.component.css',
})
export class UsersActivityMonitoringPageComponent implements OnInit {
	selectedPeriod: any;
	customRange: [Date, Date] | undefined;

	private readonly _service: TelemetryApiService = inject(TelemetryApiService);
	private readonly _query: WritableSignal<ActionRecordsQuery> = signal(ActionRecordsQuery.create().withPage(1).withPageSize(50));

	readonly timeSelections: string[] = ['7 дней', 'месяц', 'диапазон'];
	readonly selectedTimeOption: WritableSignal<string | null> = signal('7 дней');
	readonly rangedFilterDates: WritableSignal<Date[] | undefined> = signal(undefined);
	readonly statistics: WritableSignal<TelemetryStatisticsResponse[]> = signal([]);
	readonly permissions: WritableSignal<TelemetryPermissionResponse[]> = signal([]);
	readonly statuses: WritableSignal<TelemetryActionStatus[]> = signal([]);
	readonly records: WritableSignal<TelemetryResponse[]> = signal([]);
	readonly totalCount: WritableSignal<number> = signal(0);

	readonly queryChangeEffect = effect((): void => {
		const query: ActionRecordsQuery = this._query();
		this.fetchRecords(query);
	});

	readonly chartData = computed(() => {
		const items: TelemetryStatisticsResponse[] = this.statistics();
		const data: PrimeNgChartData = defaultPrimeNgChartData();
		let dataSets: number[] = [];
		for (const item of items) {
			data.labels.push(new Date(item.Date).toLocaleDateString('ru-RU'));
			dataSets.push(item.Amount);
		}
		data.datasets = [
			...dataSets.map((): PrimeNgChartDataSet => {
				return {
					label: 'Активность',
					backgroundColor: '#3b82f6',
					borderColor: '#2563eb',
					borderWidth: 1,
					data: [],
					type: 'line',
				};
			}),
		];
		for (let i: number = 0; i < data.datasets.length; i++) {
			data.datasets[i].data = dataSets;
		}

		return data;
	});

	readonly chartOptions = computed(() => {
		const options: PrimeNgChartOptions = {
			responsive: true,
			maintainAspectRatio: false,
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
				padding: 10,
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
						color: '#e5e7eb',
						font: { size: 13 },
					},
				},
				y: {
					beginAtZero: true,
					title: {
						display: true,
						text: 'Количество действий',
						color: '#e5e7eb',
						font: { size: 13 },
						padding: 30,
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
		this._query.update((query: ActionRecordsQuery): ActionRecordsQuery => {
			return query.addSort($event);
		});
	}

	public handlePermissionSelected($event: MultiSelectChangeEvent): void {
		const permissions: TelemetryPermissionResponse[] = $event.value as TelemetryPermissionResponse[];
		const names: string[] = permissions.map((perm: TelemetryPermissionResponse) => perm.Name);
	}

	public permissionDisplayText(record: TelemetryResponse): string[] {
		const permissions: TelemetryPermissionResponse[] | null = record.UserPermissions;
		if (!permissions || permissions.length === 0) {
			return ['Нет прав / аноним'];
		}
		return permissions.map((perm: TelemetryPermissionResponse) => perm.Description);
	}

	public resolveInvokerIdDisplayValue(record: TelemetryResponse): string {
		return record.UserId ? record.UserId : 'Аноним (гость)';
	}

	public ngOnInit(): void {
		const query: ActionRecordsQuery = this._query();
		this.fetchRecords(query);
	}

	public onPeriodChange(period: string, customRange?: Date[]): void {}

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
