import { DatePipe, NgClass } from '@angular/common';
import { Component, signal, WritableSignal } from '@angular/core';
import { ChartModule } from 'primeng/chart';
import { TableModule } from 'primeng/table';
import { Select } from 'primeng/select';
import { InputText } from 'primeng/inputtext';
import { DatePicker } from 'primeng/datepicker';
import { FormsModule } from '@angular/forms';
import { Checkbox } from 'primeng/checkbox';
import { PaginationComponent } from '../../../../shared/components/pagination/pagination.component';

@Component({
	selector: 'app-users-activity-monitoring-page',
	imports: [FormsModule, NgClass, ChartModule, TableModule, Select, InputText, DatePicker, Checkbox, DatePipe, PaginationComponent],
	templateUrl: './users-activity-monitoring-page.component.html',
	styleUrl: './users-activity-monitoring-page.component.css',
})
export class UsersActivityMonitoringPageComponent {
	selectedPeriod: any;
	customRange: [Date, Date] | undefined;

	readonly timeSelections: string[] = ['7 дней', 'месяц', 'диапазон'];
	readonly selectedTimeOption: WritableSignal<string | null> = signal('7 дней');
	readonly rangedFilterDates: WritableSignal<Date[] | undefined> = signal(undefined);

	actionRecords: ActionRecordResponse[] = [
		{
			Id: '1',
			InvokerId: 'user-101',
			Name: 'Login',
			Severity: 'Info',
			PayloadJson: '{"ip":"192.168.1.1"}',
			OccuredDateTime: new Date(),
			Error: null,
		},
		{
			Id: '2',
			InvokerId: 'user-102',
			Name: 'UpdateProfile',
			Severity: 'Warning',
			PayloadJson: '{"fields":["email","phone"]}',
			OccuredDateTime: new Date(),
			Error: null,
		},
		{
			Id: '3',
			InvokerId: 'user-103',
			Name: 'DeleteAccount',
			Severity: 'Critical',
			PayloadJson: '{"reason":"user request"}',
			OccuredDateTime: new Date(),
			Error: 'Account deletion failed: foreign key constraint.',
		},
		{
			Id: '4',
			InvokerId: null,
			Name: 'SystemBackup',
			Severity: 'Info',
			PayloadJson: '{"size":"2GB"}',
			OccuredDateTime: new Date(),
			Error: null,
		},
		{
			Id: '5',
			InvokerId: 'user-104',
			Name: 'ChangePassword',
			Severity: 'Info',
			PayloadJson: '{"method":"reset"}',
			OccuredDateTime: new Date(),
			Error: null,
		},
	];

	activityChartData: any;
	activityChartOptions: any;

	constructor() {
		this.generateChartData('7 дней');
		this.activityChartOptions = {
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
	}

	// Генерация данных для графика по выбранному периоду
	generateChartData(period: string): void {
		let labels: string[] = [];
		let data: number[] = [];
		const now: Date = new Date();
		if (period === '7 дней') {
			// последние 7 дней
			for (let i: number = 6; i >= 0; i--) {
				const d: Date = new Date(now);
				d.setDate(now.getDate() - i);
				labels.push(d.toLocaleDateString('ru-RU'));
				const count: number = this.actionRecords.filter((r: ActionRecordResponse) => {
					return r.OccuredDateTime.toDateString() === d.toDateString();
				}).length;
				data.push(count);
			}
		} else if (period === 'месяц') {
			for (let i: number = 3; i >= 0; i--) {
				const start: Date = new Date(now);
				start.setDate(now.getDate() - i * 7);
				const end: Date = new Date(start);
				end.setDate(start.getDate() + 6);
				labels.push(`${start.toLocaleDateString('ru-RU')} - ${end.toLocaleDateString('ru-RU')}`);
				const count: number = this.actionRecords.filter((r: ActionRecordResponse) => {
					return r.OccuredDateTime >= start && r.OccuredDateTime <= end;
				}).length;
				data.push(count);
			}
		} else if (period === 'диапазон' && this.customRange && Array.isArray(this.customRange) && this.customRange.length === 2) {
			// диапазон дат
			const start: Date = new Date(this.customRange[0]);
			const end: Date = new Date(this.customRange[1]);
			const days: number = Math.ceil((end.getTime() - start.getTime()) / (1000 * 60 * 60 * 24));
			for (let i: number = 0; i <= days; i++) {
				const d: Date = new Date(start);
				d.setDate(start.getDate() + i);
				labels.push(d.toLocaleDateString('ru-RU'));
				const count: number = this.actionRecords.filter((r: ActionRecordResponse) => {
					return r.OccuredDateTime.toDateString() === d.toDateString();
				}).length;
				data.push(count);
			}
		}
		this.activityChartData = {
			labels,
			datasets: [
				{
					label: 'Активность',
					backgroundColor: '#3b82f6',
					borderColor: '#2563eb',
					borderWidth: 1,
					data,
					type: 'line',
				},
			],
		};
	}

	onPeriodChange(period: string, customRange?: Date[]): void {
		this.selectedTimeOption.set(period);
		if (period === 'диапазон' && customRange && customRange.length === 2) {
			this.customRange = [customRange[0], customRange[1]];
		}
		this.generateChartData(period);
	}
}

export interface ActionRecordResponse {
	Id: string;
	InvokerId: string | null;
	Name: string;
	Severity: string;
	PayloadJson: string;
	OccuredDateTime: Date;
	Error: string | null;
}
