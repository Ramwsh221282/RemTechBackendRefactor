import { Component } from '@angular/core';
import { Tab, TabList, TabPanel, TabPanels, Tabs } from 'primeng/tabs';
import { SignInFormComponent } from './components/sign-in-form/sign-in-form.component';
import { SignUpFormComponent } from './components/sign-up-form/sign-up-form.component';

@Component({
  selector: 'app-sign-in-page',
  imports: [
    Tabs,
    Tab,
    TabPanels,
    TabList,
    TabPanel,
    SignInFormComponent,
    SignUpFormComponent,
  ],
  templateUrl: './sign-in-page.component.html',
  styleUrl: './sign-in-page.component.scss',
})
export class SignInPageComponent {}
