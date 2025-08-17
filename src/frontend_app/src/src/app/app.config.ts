import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import {
  provideRouter,
  withComponentInputBinding,
  withViewTransitions,
} from '@angular/router';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { providePrimeNG } from 'primeng/config';
import Aura from '@primeng/themes/aura';

import { routes } from './app.routes';
import { AppMenuService } from './shared/components/app-menu/app-menu.service';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { definePreset } from '@primeng/themes';
import { CookieService } from 'ngx-cookie-service';
import { MailingManagementService } from './pages/mailing-management-page/services/MailingManagementService';
import { JwtTokenManagingInterceptor } from './shared/middleware/JwtTokenManagingInterceptor';
import { VehicleScrapersService } from './pages/scrapers-management-page/components/scrapers-management-settings-page/services/vehicle-scrapers.service';
import { SparesService } from './pages/spares-page/services/SparesService';
import { ContainedItemsService } from './pages/main-page/services/contained-items-service';
import { PopularBrandsService } from './pages/main-page/services/popular-brands-service';
import { PopularCategoriesService } from './pages/main-page/services/popular-categories-service';
import { AllCategoriesService } from './pages/all-categories-page/services/AllCategoriesService';
import { AllBrandsService } from './pages/all-brands-page/services/AllBrandsService';
import { CatalogueVehiclesService } from './pages/vehicles-page/services/CatalogueVehiclesService';
import { UsersService } from './pages/sign-in-page/services/UsersService';
import { TokensService } from './shared/services/TokensService';

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
    }),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes, withViewTransitions(), withComponentInputBinding()),
    provideHttpClient(withInterceptors([JwtTokenManagingInterceptor])),
    TokensService,
    UsersService,
    CatalogueVehiclesService,
    AllBrandsService,
    AllCategoriesService,
    PopularBrandsService,
    PopularCategoriesService,
    ContainedItemsService,
    SparesService,
    VehicleScrapersService,
    MailingManagementService,
    AppMenuService,
    CookieService,
  ],
};
