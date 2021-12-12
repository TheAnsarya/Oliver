
// TODO: Clean this all up and centralize preferences more

import { ILocale } from "./ILocale";
import { ILocaleFull } from "./ILocaleFull";
import { IThemeName } from "./IThemeName";

export const defaultLocaleCode = "en" as ILocale;

export const defaultLocaleCodeFull = "en-us" as ILocaleFull;

export const defaultTheme = "theme-dark";

function defaultIfNotValid(theme: string | null): IThemeName {
	if ((theme === null) || (!["theme-dark"].includes(theme))) {
		theme = defaultTheme;
	}

	return theme as IThemeName;
}

export function setTheme(theme: IThemeName): void {
	localStorage.setItem("theme", defaultIfNotValid(theme) as string);
}

export function getTheme(): IThemeName {
	const theme = defaultIfNotValid(localStorage.getItem("theme"));
	return theme;
}

export function themeExists(): boolean {
	return !!localStorage.getItem("theme");
}

export function localeUses24HourTime(langCode: string): boolean {
	return (
		(Intl && Intl.DateTimeFormat)
			? new Intl.DateTimeFormat(langCode, { hour: "numeric" })
				.formatToParts(new Date(2020, 0, 1, 13))
				.find(part => part.type === "hour")?.value.length === 2
			?? false
			: false
	);
}
