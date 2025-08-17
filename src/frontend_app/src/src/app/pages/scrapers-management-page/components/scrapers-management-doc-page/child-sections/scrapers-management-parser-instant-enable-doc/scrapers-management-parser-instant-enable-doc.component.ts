import { Component } from '@angular/core';
import {
  BlueInfoTextComponent
} from "../../../../../../shared/components/information/blue-info-text/blue-info-text.component";
import {
  GrayTextInfoComponent
} from "../../../../../../shared/components/information/gray-text-info/gray-text-info.component";
import { GrayTextInfoWrapperComponent } from '../../../../../../shared/components/information/gray-text-info-wrapper/gray-text-info-wrapper.component';

@Component({
  selector: 'app-scrapers-management-parser-instant-enable-doc',
  imports: [
    BlueInfoTextComponent,
    GrayTextInfoComponent,
    GrayTextInfoWrapperComponent
  ],
  templateUrl: './scrapers-management-parser-instant-enable-doc.component.html',
  styleUrl: './scrapers-management-parser-instant-enable-doc.component.scss'
})
export class ScrapersManagementParserInstantEnableDocComponent {

}
