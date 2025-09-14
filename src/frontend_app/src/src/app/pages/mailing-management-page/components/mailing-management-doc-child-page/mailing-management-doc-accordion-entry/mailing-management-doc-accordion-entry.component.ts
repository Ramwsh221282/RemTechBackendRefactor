import { Component, Input } from '@angular/core';
import { MailingManagementAccordionEntry } from '../types/MailingManagementAccordionEntry';
import {
  AccordionContent,
  AccordionHeader,
  AccordionPanel,
} from 'primeng/accordion';
import { NgIf } from '@angular/common';
import { StyleClass } from 'primeng/styleclass';

@Component({
  selector: 'app-mailing-management-doc-accordion-entry',
  imports: [AccordionContent, AccordionHeader, NgIf, AccordionPanel],
  templateUrl: './mailing-management-doc-accordion-entry.component.html',
  styleUrl: './mailing-management-doc-accordion-entry.component.scss',
})
export class MailingManagementDocAccordionEntryComponent {
  @Input({ required: true }) set guidance_entry_setter(
    value: MailingManagementAccordionEntry,
  ) {
    this.entry = value;
  }

  public entry: MailingManagementAccordionEntry = {
    value: 0,
    guidanceName: '',
    guidances: [],
    iconFill: '',
    iconViewBox: '',
    iconClasses: '',
    serviceName: '',
  };
}
