import { StringUtils } from './string-utils';

export function readInputTextFromInputElementEvent($event: Event): string | null | undefined {
	const input: HTMLInputElement = $event.target as HTMLInputElement;
	const text: string = input.value;
	if (StringUtils.isEmptyOrWhiteSpace(text)) return undefined;
	return text;
}
