import { Component } from '@angular/core';
import { MessageService } from 'primeng/api';
import { ContainedItemsInfoComponent } from './components/contained-items-info/contained-items-info.component';
import { PopularCategoriesBlockComponent } from './components/popular-categories-block/popular-categories-block.component';
import { PopularBrandsBlockComponent } from './components/popular-brands-block/popular-brands-block.component';
import { RecentItemsListComponent } from './components/recent-items-list/recent-items-list.component';
import { FastNavigationComponent } from './components/fast-navigation/fast-navigation.component';

@Component({
  selector: 'app-main-page',
  imports: [
    ContainedItemsInfoComponent,
    PopularCategoriesBlockComponent,
    PopularBrandsBlockComponent,
    RecentItemsListComponent,
    FastNavigationComponent,
  ],
  templateUrl: './main-page.component.html',
  styleUrl: './main-page.component.scss',
  providers: [MessageService],
})
export class MainPageComponent {}
