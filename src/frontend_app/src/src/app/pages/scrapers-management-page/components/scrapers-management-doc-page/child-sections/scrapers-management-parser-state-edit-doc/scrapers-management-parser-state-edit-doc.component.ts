import { Component } from '@angular/core';
import { BlueInfoTextComponent } from '../../../../../../shared/components/information/blue-info-text/blue-info-text.component';
import { GrayTextInfoWrapperComponent } from '../../../../../../shared/components/information/gray-text-info-wrapper/gray-text-info-wrapper.component';
import { GrayTextInfoComponent } from '../../../../../../shared/components/information/gray-text-info/gray-text-info.component';

@Component({
  selector: 'app-scrapers-management-parser-state-edit-doc',
  imports: [
    BlueInfoTextComponent,
    GrayTextInfoWrapperComponent,
    GrayTextInfoComponent,
  ],
  templateUrl: './scrapers-management-parser-state-edit-doc.component.html',
  styleUrl: './scrapers-management-parser-state-edit-doc.component.scss',
})
export class ScrapersManagementParserStateEditDocComponent {}
