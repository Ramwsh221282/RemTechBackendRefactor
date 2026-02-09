import { Component } from '@angular/core';
import {
  BlueInfoTextComponent
} from "../../../../../../shared/components/information/blue-info-text/blue-info-text.component";
import {
  GrayTextInfoComponent
} from "../../../../../../shared/components/information/gray-text-info/gray-text-info.component";
import { GrayTextInfoWrapperComponent } from '../../../../../../shared/components/information/gray-text-info-wrapper/gray-text-info-wrapper.component';

@Component({
  selector: 'app-scrapers-management-parser-link-appending-doc',
  imports: [
    BlueInfoTextComponent,
    GrayTextInfoComponent,
    GrayTextInfoWrapperComponent
  ],
  templateUrl: './scrapers-management-parser-link-appending-doc.component.html',
  styleUrl: './scrapers-management-parser-link-appending-doc.component.scss'
})
export class ScrapersManagementParserLinkAppendingDocComponent {

}
