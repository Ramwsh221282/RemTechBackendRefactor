import { Component } from '@angular/core';
import { Accordion, AccordionContent, AccordionHeader, AccordionPanel } from 'primeng/accordion';

@Component({
  selector: 'app-scrapers-management-greeting-child-page',
  imports: [Accordion, AccordionPanel, AccordionHeader, AccordionContent],
  templateUrl: './scrapers-management-greeting-child-page.component.html',
  styleUrl: './scrapers-management-greeting-child-page.component.scss',
})
export class ScrapersManagementGreetingChildPageComponent {}
