export type MailerResponse = {
  Id: string;
  Email: string;
  SmtpHost: string;
}

export const DefaultMailerResponse: () => MailerResponse = (): MailerResponse => {
  return { Id: '', Email: '', SmtpHost: '' };
}
