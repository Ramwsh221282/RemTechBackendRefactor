import { MessageService } from 'primeng/api';

export class MessageServiceUtils {
  public static showSuccess(messageService: MessageService, text: string) {
    messageService.add({
      severity: 'success',
      summary: 'Успешно',
      detail: text,
    });
  }

  public static showStickySuccess(messageService: MessageService, text: string) {
    messageService.add({
      severity: 'success',
      summary: 'Успешно',
      detail: text,
      sticky: true,
    });
  }

  public static showInfo(messageService: MessageService, text: string) {
    messageService.add({
      severity: 'info',
      summary: 'Информация',
      detail: text,
    });
  }

  public static showStickyInfo(messageService: MessageService, text: string) {
    messageService.add({
      severity: 'info',
      summary: 'Информация',
      detail: text,
      sticky: true,
    });
  }

  public static showWarn(messageService: MessageService, text: string) {
    messageService.add({
      severity: 'warn',
      summary: 'Предупреждение',
      detail: text,
    });
  }

  public static showError(messageService: MessageService, text: string) {
    messageService.add({ severity: 'error', summary: 'Ошибка', detail: text });
  }

  public static showContrast(messageService: MessageService, text: string) {
    messageService.add({
      severity: 'contrast',
      summary: 'Ошибка',
      detail: text,
    });
  }

  public static showSecondary(messageService: MessageService, text: string) {
    messageService.add({
      severity: 'secondary',
      summary: 'Информация',
      detail: text,
    });
  }
}
