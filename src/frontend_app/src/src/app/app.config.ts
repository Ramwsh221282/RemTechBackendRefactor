import { ApplicationConfig, inject, provideEnvironmentInitializer, provideZoneChangeDetection } from '@angular/core';
import { provideRouter, withComponentInputBinding, withViewTransitions } from '@angular/router';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { providePrimeNG } from 'primeng/config';
import Aura from '@primeng/themes/aura';

import { routes } from './app.routes';
import { AppMenuService } from './shared/components/app-menu/app-menu.service';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { definePreset } from '@primeng/themes';
import { MailingManagementService } from './pages/mailing-management-page/services/MailingManagementService';
import { SparesService } from './pages/spares-page/services/SparesService';
import { ContainedItemsService } from './pages/main-page/services/contained-items-service';
import { PopularBrandsService } from './pages/main-page/services/popular-brands-service';
import { PopularCategoriesService } from './pages/main-page/services/popular-categories-service';
import { AllCategoriesService } from './pages/all-categories-page/services/AllCategoriesService';
import { AllBrandsService } from './pages/all-brands-page/services/AllBrandsService';
import { CatalogueVehiclesService } from './pages/vehicles-page/services/CatalogueVehiclesService';
import { UserInfoService } from './shared/services/UserInfoService';
import { authInterceptor } from './shared/middleware/auth-interceptor.interceptor';
import { OnApplicationStartupAuthVerificationService } from './shared/services/OnApplicationStartupAuthVerification.service';
import { ForbiddenInterceptor } from './shared/middleware/forbidden.interceptor';
import { PermissionsStatusService } from './shared/services/PermissionsStatus.service';

const myPreset = definePreset(Aura, {
	semantic: {
		primary: {
			50: '{yellow.50}',
			100: '{yellow.100}',
			200: '{yellow.200}',
			300: '{yellow.300}',
			400: '{yellow.400}',
			500: '{yellow.500}',
			600: '{yellow.600}',
			700: '{yellow.700}',
			800: '{yellow.800}',
			900: '{yellow.900}',
			950: '{yellow.950}',
		},
	},
});

export const appConfig: ApplicationConfig = {
	providers: [
		provideAnimationsAsync(),
		providePrimeNG({
			theme: {
				preset: myPreset,
			},
			translation: {
				accept: 'Да',
				reject: 'Нет',
				addRule: 'Добавить правило',
				clear: 'Очистить',
				prevYear: 'Предыдущий год',
				nextYear: 'Следующий год',
				prevDecade: 'Предыдущее десятилетие',
				nextDecade: 'Следующее десятилетие',
				today: 'Сегодня',
				nextMonth: 'Следующий месяц',
				prevMonth: 'Предыдущий месяц',
				weekHeader: 'Нед',
				after: 'После',
				before: 'До',
				apply: 'Применить',
				monthNames: [
					'Январь',
					'Февраль',
					'Март',
					'Апрель',
					'Май',
					'Июнь',
					'Июль',
					'Август',
					'Сентябрь',
					'Октябрь',
					'Ноябрь',
					'Декабрь',
				],
				monthNamesShort: ['Янв', 'Фев', 'Мар', 'Апр', 'Май', 'Июн', 'Июл', 'Авг', 'Сен', 'Окт', 'Ноя', 'Дек'],
				dayNamesShort: ['Вс', 'Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб'],
				dayNamesMin: ['Вс', 'Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб'],
				aria: {
					trueLabel: 'Истина',
					falseLabel: 'Ложь',
					browseFiles: 'Просмотр файлов',
					cancelEdit: 'Отменить редактирование',
				},
				chooseDate: 'Выбрать дату',
				chooseMonth: 'Выбрать месяц',
				chooseYear: 'Выбрать год',
				choose: 'Выбрать',
			},
		}),
		provideZoneChangeDetection({ eventCoalescing: true }),
		provideRouter(routes, withViewTransitions(), withComponentInputBinding()),
		provideHttpClient(withInterceptors([authInterceptor, ForbiddenInterceptor])),
		provideEnvironmentInitializer(() => {
			inject(OnApplicationStartupAuthVerificationService).ngOnInit();
			inject(PermissionsStatusService).ngOnInit();
		}),
		UserInfoService,
		CatalogueVehiclesService,
		AllBrandsService,
		AllCategoriesService,
		PopularBrandsService,
		PopularCategoriesService,
		ContainedItemsService,
		SparesService,
		MailingManagementService,
		AppMenuService,
	],
};
