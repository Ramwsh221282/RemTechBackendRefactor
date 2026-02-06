export interface MailingManagementAccordionEntry {
  serviceName: string;
  guidanceName: string;
  guidances: MailingManagementAccordionEntryGuidance[];
  iconClasses: string;
  iconViewBox: string;
  iconFill: string;
  value: number;
}

export interface MailingManagementAccordionEntryGuidance {
  guidance: string;
  extraLink?: string | null;
}
