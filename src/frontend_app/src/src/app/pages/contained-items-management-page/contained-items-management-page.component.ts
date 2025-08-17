import { Component, signal, WritableSignal } from '@angular/core';
import { ConfirmationService, MessageService } from 'primeng/api';
import { Toast } from 'primeng/toast';
import { ConfirmDialog } from 'primeng/confirmdialog';
import { Dialog } from 'primeng/dialog';
import { Button } from 'primeng/button';
import { InputText } from 'primeng/inputtext';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ContainedItemsService } from '../main-page/services/contained-items-service';
import { HttpErrorResponse } from '@angular/common/http';
import { MessageServiceUtils } from '../../shared/utils/message-service-utils';

@Component({
  selector: 'app-contained-items-management-page',
  imports: [
    Toast,
    ConfirmDialog,
    Dialog,
    Button,
    InputText,
    ReactiveFormsModule,
  ],
  templateUrl: './contained-items-management-page.component.html',
  styleUrl: './contained-items-management-page.component.scss',
  providers: [MessageService, ConfirmationService],
})
export class ContainedItemsManagementPageComponent {
  private readonly _vehiclesConfirmed: WritableSignal<boolean>;
  private readonly _sparesConfirmed: WritableSignal<boolean>;

  public readonly vehicleAuthForm: FormGroup = new FormGroup({
    email: new FormControl(''),
    name: new FormControl(''),
    password: new FormControl(''),
  });

  public readonly sparesAuthForm: FormGroup = new FormGroup({
    email: new FormControl(''),
    name: new FormControl(''),
    password: new FormControl(''),
  });

  constructor(
    private readonly _messageService: MessageService,
    private readonly _confirmationService: ConfirmationService,
    private readonly _service: ContainedItemsService,
  ) {
    this._vehiclesConfirmed = signal(false);
    this._sparesConfirmed = signal(false);
  }

  public get vehiclesConfirmed(): boolean {
    return this._vehiclesConfirmed();
  }

  public get sparesConfirmed(): boolean {
    return this._sparesConfirmed();
  }

  public submitVehicles(): void {
    const formValues = this.vehicleAuthForm.value;
    const email: string = formValues.email as string;
    const name: string = formValues.name as string;
    const password: string = formValues.password as string;
    this._service.deleteVehicles(email, name, password).subscribe({
      next: (deleted: number): void => {
        const message = `Удалено ${deleted} техники.`;
        MessageServiceUtils.showSuccess(this._messageService, message);
        this.vehicleAuthForm.reset();
        this._vehiclesConfirmed.set(false);
      },
      error: (err: HttpErrorResponse): void => {
        const message = err.error.message;
        MessageServiceUtils.showError(this._messageService, message);
        this.vehicleAuthForm.reset();
        this._vehiclesConfirmed.set(false);
      },
    });
  }

  public submitSpares(): void {
    const formValues = this.sparesAuthForm.value;
    const email: string = formValues.email as string;
    const name: string = formValues.name as string;
    const password: string = formValues.password as string;
    this._service.deleteVehicles(email, name, password).subscribe({
      next: (deleted: number): void => {
        const message = `Удалено ${deleted} запчастей.`;
        MessageServiceUtils.showSuccess(this._messageService, message);
        this.sparesAuthForm.reset();
        this._sparesConfirmed.set(false);
      },
      error: (err: HttpErrorResponse): void => {
        const message = err.error.message;
        MessageServiceUtils.showError(this._messageService, message);
        this.sparesAuthForm.reset();
        this._sparesConfirmed.set(false);
      },
    });
  }

  public cancelVehicles(): void {
    this._vehiclesConfirmed.set(false);
  }

  public cancelSpares(): void {
    this._sparesConfirmed.set(false);
  }

  public confirmVehicles($event: Event): void {
    this._confirmationService.confirm({
      target: $event.target as EventTarget,
      message: 'Удалить данные о технике?',
      header: 'Подтверждение',
      closable: true,
      closeOnEscape: true,
      icon: 'pi pi-exclamation-triangle',
      rejectButtonProps: {
        label: 'Отмена',
        severity: 'secondary',
        outlined: true,
      },
      acceptButtonProps: {
        label: 'Подтвердить',
      },
      accept: () => {
        this._vehiclesConfirmed.set(true);
      },
      reject: () => {
        this._vehiclesConfirmed.set(false);
      },
    });
  }

  public confirmSpares($event: Event): void {
    this._confirmationService.confirm({
      target: $event.target as EventTarget,
      message: 'Удалить данные о запчастях?',
      header: 'Подтверждение',
      closable: true,
      closeOnEscape: true,
      icon: 'pi pi-exclamation-triangle',
      rejectButtonProps: {
        label: 'Отмена',
        severity: 'secondary',
        outlined: true,
      },
      acceptButtonProps: {
        label: 'Подтвердить',
      },
      accept: () => {
        this._sparesConfirmed.set(true);
      },
      reject: () => {
        this._sparesConfirmed.set(false);
      },
    });
  }
}
