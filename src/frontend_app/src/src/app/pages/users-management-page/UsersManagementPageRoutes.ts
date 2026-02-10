import { Routes } from '@angular/router';

export const UsersManagementPageRoutes: Routes = [
	{
		path: 'users-management',
		loadComponent: () => import('./users-management-page.component').then((c) => c.UsersManagementPageComponent),
	},
	{
		path: 'users-management/monitoring',
		loadComponent: () =>
			import('./pages/users-activity-monitoring-page/users-activity-monitoring-page.component').then(
				(c) => c.UsersActivityMonitoringPageComponent,
			),
	},
];
