import { ISelectOption } from './dtos/SelectOptionDto';

export const defaultCacheExpirationMinutes = 15;

export var pageSizes = [10, 25, 50, 100];
export const defaultPageSize = 10;
export const refreshTokenIntervalMinutes = 15;
export const localStorageUserIdentityToken = "UserIdentityToken";
export const localStorageUserIdentityRefreshToken = "UserIdentityRefreshToken";
export const localStorageUserInformation = "UserInformation";
export const localStorageUserIdentityTokenValidTo = "UserIdentityTokenValidTo";
export const localStorageUserRoles = "UserRoles";
export const localStorageUserPolicies = "UserPolicies";
export const sessionStoreageRememberMe = "RememberMe";
export const localStorageRefreshTokenCallStarted = "RefreshTokenCallStarted";
export const sessionStorageRefreshTokenCallRunning = "RefreshTokenCallRunning";
export const sessionStorageRefreshTokenCallStartedDateTime = "RefreshTokenCallStartedDateTime";

export const usPhoneMask = '(999) 999-9999';
export const globalPhoneMask = 'phone';

export const dateMaskFormat = "mm/dd/yyyy";
export const datePickerFormat = "MM/DD/YYYY";
export const defaultAngularDateFormat = "MM/dd/yyyy";
export const defaultAngularDateTimeFormat = "MM/dd/yyyy h:mm aa";

export var certificationLevels:Array<ISelectOption<number>> = [{ text: "0", value: 0 }, { text: "1", value: 1 }, { text: "2", value: 2 }, { text: "3", value: 3 }];
