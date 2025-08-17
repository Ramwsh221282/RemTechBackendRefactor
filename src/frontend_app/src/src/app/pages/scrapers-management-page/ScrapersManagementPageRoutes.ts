import { Routes } from '@angular/router';
import { AdminAccessGuard } from '../../shared/guards/AdminAccessGuard';
import { RootExistsGuard } from '../../shared/guards/RootExistsGuard';

export const ScrapersManagementPageRoutes: Routes = [
  {
    path: 'scrapers',
    loadComponent: () =>
      import('./scrapers-management-page.component').then(
        (c) => c.ScrapersManagementPageComponent,
      ),
    canActivate: [RootExistsGuard, AdminAccessGuard],
    children: [
      {
        path: 'cleaner',
        loadComponent: () =>
          import(
            './components/cleaner-management-page/cleaner-management-page.component'
          ).then((c) => c.CleanerManagementPageComponent),
      },
      {
        path: 'greeting',
        loadComponent: () =>
          import(
            './components/scrapers-management-greeting-child-page/scrapers-management-greeting-child-page.component'
          ).then((c) => c.ScrapersManagementGreetingChildPageComponent),
      },
      {
        path: 'settings',
        loadComponent: () =>
          import(
            './components/scrapers-management-settings-page/scrapers-management-settings-page.component'
          ).then((c) => c.ScrapersManagementSettingsPageComponent),
        children: [
          {
            path: ':scraperName/:scraperType',
            loadComponent: () =>
              import(
                './components/scrapers-management-concrete-scraper-page/scrapers-management-concrete-scraper-page.component'
              ).then((c) => c.ScrapersManagementConcreteScraperPageComponent),
          },
        ],
      },
      {
        path: 'doc',
        loadComponent: () =>
          import(
            './components/scrapers-management-doc-page/scrapers-management-doc-page.component'
          ).then((c) => c.ScrapersManagementDocPageComponent),
        children: [
          {
            path: 'beginning',
            loadComponent: () =>
              import(
                './components/scrapers-management-doc-page/child-sections/scrapers-management-doc-beginning/scrapers-management-doc-beginning.component'
              ).then((c) => c.ScrapersManagementDocBeginningComponent),
          },
          {
            path: 'parser-state-edit-doc',
            loadComponent: () =>
              import(
                './components/scrapers-management-doc-page/child-sections/scrapers-management-parser-state-edit-doc/scrapers-management-parser-state-edit-doc.component'
              ).then((c) => c.ScrapersManagementParserStateEditDocComponent),
          },
          {
            path: 'parser-time-edit-doc',
            loadComponent: () =>
              import(
                './components/scrapers-management-doc-page/child-sections/scrapers-management-parser-time-edit-doc/scrapers-management-parser-time-edit-doc.component'
              ).then((c) => c.ScrapersManagementParserTimeEditDocComponent),
          },
          {
            path: 'parser-instant-enable-doc',
            loadComponent: () =>
              import(
                './components/scrapers-management-doc-page/child-sections/scrapers-management-parser-instant-enable-doc/scrapers-management-parser-instant-enable-doc.component'
              ).then(
                (c) => c.ScrapersManagementParserInstantEnableDocComponent,
              ),
          },
          {
            path: 'parser-instant-disable-doc',
            loadComponent: () =>
              import(
                './components/scrapers-management-doc-page/child-sections/scrapers-management-parser-instant-disable-doc/scrapers-management-parser-instant-disable-doc.component'
              ).then(
                (c) => c.ScrapersManagementParserInstantDisableDocComponent,
              ),
          },
          {
            path: 'parser-link-appending-doc',
            loadComponent: () =>
              import(
                './components/scrapers-management-doc-page/child-sections/scrapers-management-parser-link-appending-doc/scrapers-management-parser-link-appending-doc.component'
              ).then(
                (c) => c.ScrapersManagementParserLinkAppendingDocComponent,
              ),
          },
          {
            path: 'parser-link-editing-doc',
            loadComponent: () =>
              import(
                './components/scrapers-management-doc-page/child-sections/scrapers-management-parser-link-editing-doc/scrapers-management-parser-link-editing-doc.component'
              ).then((c) => c.ScrapersManagementParserLinkEditingDocComponent),
          },
        ],
      },
    ],
  },
];
