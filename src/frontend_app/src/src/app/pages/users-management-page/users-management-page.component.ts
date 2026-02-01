import { Component } from '@angular/core';

import { ConfirmationService, MessageService } from 'primeng/api';
import { Button } from 'primeng/button';
import { RouterLink } from '@angular/router';

@Component({
	selector: 'app-users-management-page',
	imports: [Button, RouterLink],
	templateUrl: './users-management-page.component.html',
	styleUrl: './users-management-page.component.scss',
	providers: [ConfirmationService, MessageService],
})
export class UsersManagementPageComponent {
	constructor() {}
}
