export class ConfirmationDialogUtils {
  public static createBasicConfirmationProps(
    bodyMessage: string,
    headerMessage: string,
    actionOnAccept: () => void,
    actionOnReject: () => void,
  ): object {
    return {
      message: bodyMessage,
      header: headerMessage,
      icon: 'pi pi-info-circle',
      rejectLabel: 'Отменить',
      rejectButtonProps: {
        label: 'Отменить',
        severity: 'secondary',
        outlined: true,
      },
      acceptButtonProps: {
        label: 'Подтвердить',
        severity: 'danger',
      },
      accept: actionOnAccept,
      reject: actionOnReject,
    };
  }
}
