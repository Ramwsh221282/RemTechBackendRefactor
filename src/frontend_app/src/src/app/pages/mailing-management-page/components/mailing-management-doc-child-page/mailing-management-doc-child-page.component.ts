import { Component } from '@angular/core';
import { Accordion } from 'primeng/accordion';
import { MailingManagementAccordionEntry } from './types/MailingManagementAccordionEntry';
import { MailingManagementDocAccordionEntryComponent } from './mailing-management-doc-accordion-entry/mailing-management-doc-accordion-entry.component';

@Component({
  selector: 'app-mailing-management-doc-child-page',
  imports: [Accordion, MailingManagementDocAccordionEntryComponent],
  templateUrl: './mailing-management-doc-child-page.component.html',
  styleUrl: './mailing-management-doc-child-page.component.scss',
})
export class MailingManagementDocChildPageComponent {
  public readonly entries: MailingManagementAccordionEntry[] = [
    {
      serviceName: 'Google',
      guidanceName: 'Подключение почты от google.com:',
      iconClasses: 'w-6 h-6 text-yellow-600 dark:text-yellow-400',
      iconFill: 'currentColor',
      iconViewBox: '0 0 24 24',
      value: 0,
      guidances: [
        {
          guidance:
            'Создайте почтовый ящик gmail.com или войдите в существующий.',
          extraLink: 'https://workspace.google.com/intl/ru/gmail/',
        },
        {
          guidance:
            'Создайте SMTP пароль для приложения. Скопируйте созданный пароль.',
          extraLink: 'https://myaccount.google.com/apppasswords',
        },
        {
          guidance:
            'Пройдите процедуру подключения на странице "Настройка" и проверьте работоспособность.',
        },
      ],
    },
    {
      serviceName: 'Yandex',
      guidanceName: 'Подключение почты от yandex.ru:',
      iconClasses: 'w-6 h-6 text-red-600 dark:text-red-400',
      iconFill: 'currentColor',
      iconViewBox: '0 0 24 24',
      value: 1,
      guidances: [
        {
          guidance:
            'Создайте почтовый ящик yandex.ru или войдите в существующий.',
          extraLink: 'https://passport.yandex.ru/auth/reg/portal?mode=register',
        },
        {
          guidance:
            'Создайте IMAP, POP3, SMTP пароль для приложения. Скопируйте созданный пароль.',
          extraLink: 'https://id.yandex.ru/security/app-passwords',
        },
        {
          guidance:
            'Пройдите процедуру подключения на странице "Настройка" и проверьте работоспособность.',
        },
      ],
    },
    {
      serviceName: 'Mail.ru',
      guidanceName: 'Подключение почты от mail.ru:',
      iconClasses: 'w-6 h-6 text-blue-600 dark:text-blue-400',
      iconFill: 'currentColor',
      iconViewBox: '0 0 24 24',
      value: 2,
      guidances: [
        {
          guidance:
            'Создайте почтовый ящик mail.ru или войдите в существующий.',
          extraLink: 'https://new.mail.ru/',
        },
        {
          guidance: 'Перейдите в управление безопасностью аккаунта.',
          extraLink: 'https://id.mail.ru/security',
        },
        {
          guidance:
            'Создайте пароль SMTP для внешнего приложения. Скопируйте созданный пароль.',
          extraLink: 'https://account.mail.ru/user/2-step-auth/passwords',
        },
        {
          guidance:
            'Пройдите процедуру подключения на странице "Настройка" и проверьте работоспособность.',
        },
      ],
    },
  ];
}
