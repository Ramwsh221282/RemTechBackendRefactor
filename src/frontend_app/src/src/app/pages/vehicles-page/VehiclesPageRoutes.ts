import { Routes } from '@angular/router';

export const VehiclesPageRoutes: Routes = [
	{
		path: 'vehicles',
		loadComponent: () => import('./vehicles-page.component').then((c) => c.VehiclesPageComponent),
	},
];
