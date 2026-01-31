import { DatePipe, NgClass } from '@angular/common';
import { Component, computed, inject, OnInit, Signal, signal, WritableSignal } from '@angular/core';
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
	FetchTelemetryRecordsResponse,
	TelemetryResponse,
} from '../../../../shared/api/telemetry-module/telemetry-responses';
import { createDefaultFetchTelemetryRecordsFetchResponse } from '../../../../shared/api/telemetry-module/telemetry-factories';
import { TelemetryApiService } from '../../../../shared/api/telemetry-module/telemetry-api.service';
import { catchError, EMPTY, forkJoin, map, Observable, tap } from 'rxjs';
import { FetchAnalyticsTelemetryRecordsQuery } from '../../../../shared/api/telemetry-module/telemetry-fetch.request';

@Component({
	selector: 'app-users-activity-monitoring-page',
	imports: [FormsModule, NgClass, ChartModule, TableModule, Select, InputText, DatePicker, Checkbox, DatePipe, PaginationComponent],
	templateUrl: './users-activity-monitoring-page.component.html',
	styleUrl: './users-activity-monitoring-page.component.css',
})
export class UsersActivityMonitoringPageComponent implements OnInit {
	selectedPeriod: any;
	customRange: [Date, Date] | undefined;

	readonly timeSelections: string[] = ['7 дней', 'месяц', 'диапазон'];
	readonly selectedTimeOption: WritableSignal<string | null> = signal('7 дней');
	readonly rangedFilterDates: WritableSignal<Date[] | undefined> = signal(undefined);
	readonly data: WritableSignal<FetchTelemetryRecordsResponse> = signal(createDefaultFetchTelemetryRecordsFetchResponse());
	readonly _service: TelemetryApiService = inject(TelemetryApiService);
	readonly _query: WritableSignal<FetchAnalyticsTelemetryRecordsQuery> = signal(
		FetchAnalyticsTelemetryRecordsQuery.create().withPage(1).withPageSize(50),
	);

	readonly statistics: WritableSignal<TelemetryStatisticsResponse[]> = signal([]);
	readonly records: Signal<TelemetryResponse[]> = computed((): TelemetryResponse[] => {
		return this.data().Items;
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
				position: 'top', // 'bottom', 'left', 'right'
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

	public resolveInvokerIdDisplayValue(record: TelemetryResponse): string {
		return record.UserId ? record.UserId : 'Аноним (гость)';
	}

	public ngOnInit(): void {
		const dataFetch: Observable<FetchTelemetryRecordsResponse> = this._service.fetchTelemetryRecords(this._query());
		const statsFetch: Observable<TelemetryStatisticsResponse[]> = this._service.fetchTelemetryRecordsStatistics(this._query());
		forkJoin([dataFetch, statsFetch])
			.pipe(
				map((response: [FetchTelemetryRecordsResponse, TelemetryStatisticsResponse[]]) => {
					this.data.set(response[0]);
					this.statistics.set(response[1]);
				}),
				catchError(() => EMPTY),
			)
			.subscribe();
	}

	public onPeriodChange(period: string, customRange?: Date[]): void {}
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
