import { Routes } from '@angular/router';
import { MainPageRoutes } from './pages/main-page/main-page-routes';
import { RegistrationConfirmationRoute } from './pages/registration-confirmation-page/registration-confirmation-route';
import { ForbiddenPageRoutes } from './pages/forbidden-page/forbidden-page-routes';
import { MailingManagementPageRoutes } from './pages/mailing-management-page/routing/MailingManagementPageRoutes';
import { SignInPageRoutes } from './pages/sign-in-page/SignInPareRoutes';
import { ScrapersManagementPageRoutes } from './pages/scrapers-management-page/ScrapersManagementPageRoutes';
import { SparesPageRoutes } from './pages/spares-page/spares-page-routes';
import { AllCategoriesRoute } from './pages/all-categories-page/AllCategoriesRoute';
import { AllBrandsPageRoutes } from './pages/all-brands-page/AllBrandsPageRoutes';
import { VehiclesPageRoutes } from './pages/vehicles-page/VehiclesPageRoutes';
import { ConcreteVehicleRoute } from './pages/concrete-vehicle-page/ConcreteVehicleRoute';
import { CreateRootUserRoutes } from './pages/create-root-user-page/CreateRootUserRoutes';
import { SomethingWentWrongRoutes } from './pages/something-went-wrong-page/SomethingWentWrongRoutes';
import { ContainedItemsRoutes } from './pages/contained-items-management-page/ContainedItemsRoutes';
import { UserInfoPageRoutes } from './pages/user-info-page/user-info-page-routes';
import { UsersManagementPageRoutes } from './pages/users-management-page/UsersManagementPageRoutes';
import { PasswordResetRoutes } from './pages/PasswordResetConfirmationPage/PasswordResetRoutes';

export const routes: Routes = [
  ...UsersManagementPageRoutes,
  ...UserInfoPageRoutes,
  ...ContainedItemsRoutes,
  ...SomethingWentWrongRoutes,
  ...CreateRootUserRoutes,
  ...VehiclesPageRoutes,
  ...ConcreteVehicleRoute,
  ...AllBrandsPageRoutes,
  ...AllCategoriesRoute,
  ...SparesPageRoutes,
  ...MainPageRoutes,
  ...RegistrationConfirmationRoute,
  ...ForbiddenPageRoutes,
  ...MailingManagementPageRoutes,
  ...SignInPageRoutes,
  ...ScrapersManagementPageRoutes,
  ...PasswordResetRoutes,
];
