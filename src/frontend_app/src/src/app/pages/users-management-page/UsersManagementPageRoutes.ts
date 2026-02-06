import { Routes } from '@angular/router';
import { AdminAccessGuard } from '../../shared/guards/AdminAccessGuard';

export const UsersManagementPageRoutes: Routes = [
	{
		path: 'users-management',
		loadComponent: () => import('./users-management-page.component').then((c) => c.UsersManagementPageComponent),
		canActivate: [AdminAccessGuard],
	},
	{
		path: 'users-management/monitoring',
		loadComponent: () =>
			import('./pages/users-activity-monitoring-page/users-activity-monitoring-page.component').then(
				(c) => c.UsersActivityMonitoringPageComponent,
			),
	},
];
