export type ChangeMailerRequest = {
  SmtpPassword: string;
  Email: string;
}

export type AddMailerRequest = {
  SmtpPassword: string;
  Email: string;
}

export type SendTestMessageRequest = {
  Recipient: string;
}
