import { Injectable, Inject } from '@angular/core';
import { Router } from "@angular/router";
import { Observable, of } from 'rxjs';
import { map, share } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { AccountControllerAPI } from 'app.common/apis/AccountController'

import { NotificationsService, NotificationType } from 'angular2-notifications';
import { notifyUser, createToastFromServiceResponse } from "app.common/helpers/Toaster";
import {
  localStorageUserIdentityToken, localStorageUserInformation, localStorageUserIdentityTokenValidTo,
  localStorageUserIdentityRefreshToken, refreshTokenIntervalMinutes, localStorageUserPolicies,
  sessionStoreageRememberMe,
  localStorageUserRoles,
  sessionStorageRefreshTokenCallStartedDateTime,
  sessionStorageRefreshTokenCallRunning,
  localStorageRefreshTokenCallStarted
} from "app.common/constants";
import * as moment from 'moment';

import { ILogin, IForgotPassword, IResetPassword, IRegister, IConfirmEmail, ITwoFactorSetup, ITwoFactorCode, IChangePassword } from 'app.common/dtos/AccountDtos';
import { IToken, IRefreshToken } from 'app.common/dtos/TokenDtos';
import { ITokenUser } from 'app.common/dtos/TokenUserDto';
import { ApiCache } from 'app.common/cache/api.cache';
import { canImpersonate, userAddEditDelete, taskAddEditDelete, announcementAddEditDelete, eventAddEditDelete, locationAddEditDelete, pictureAddEditDelete, canApprovePicture } from 'app.common/serverConstants/PolicyNames';
import { admin, registrar, lco } from 'app.common/serverConstants/Role';
import { IUserPolicies } from 'app.common/dtos/UserPoliciesDto';
import { Local } from 'protractor/built/driverProviders';
import { UsersControllerAPI } from '../apis/UsersController';


@Injectable()
export class AuthService {
  constructor(private httpClient: HttpClient, @Inject('baseUrl') private baseUrl: string, private router: Router,
    private toaster: NotificationsService, private apiCache: ApiCache) {

    window.addEventListener("storage", this.storageEventHandler)
    //create timer and setup time interval based on existing expire date
    this.setupRefreshTokenTimer();
  }

  pushNotifications: any;
  // store the URL so we can redirect after logging in
  redirectUrl: string = '/';
  refreshTokenIntervalId: NodeJS.Timer | undefined;
  refreshTokenObservable: Observable<IToken> | undefined;
  private _userInfo: ITokenUser | undefined;
  private _userPolicies: IUserPolicies | undefined;
  private _userRoles: Array<string> | undefined;

  login(dto: ILogin): Observable<IToken> {
    var self = this;
    var observable = this.httpClient.post<IToken>(self.baseUrl + AccountControllerAPI.Login(), dto)
      .pipe(share(), map((data: any, index: any) => { return data; }));

    observable.subscribe((data: IToken) => {
      this.apiCache.clearCacheByUrl(UsersControllerAPI.GetUsers());
      if (data.token == "2fa") {
        self.rememberMe = dto.rememberMe;
        self.router.navigate(["twofactor"]);
      }
      else if (data.token == "2faSetup") {
        self.rememberMe = dto.rememberMe;
        self._userInfo = data.userInfo;
        self.router.navigate(["twofactorsetup"]);
      }
      else {
        notifyUser(self.pushNotifications, self.toaster, NotificationType.Info, "Login", "Successful");

        self.storeTokenInfo(data);
      }
    },
      (error: any) => {
        console.log(error);
        createToastFromServiceResponse(error, self.pushNotifications, self.toaster, "Login", "Error occurred while logging in.")
      },
      () => {
      });

    return observable;
  }

  refreshUserToken() {
    if (this.refreshToken == null) {
      console.log("No refresh token");
    }

    // if refresh already happening then don't start it again.
    if (this.isRefreshTokenApiAlreadyRunning()) {
      return;
    }

    this.refreshTokenApiCall(1);

    var self = this;
    var dto: IRefreshToken = {
      refreshToken: self.refreshToken
    };
    this.refreshTokenObservable = this.httpClient.post<IToken>(self.baseUrl + AccountControllerAPI.RefreshToken(), dto)
      .pipe(share(), map((data: any, index: any) => { return data; }));

    this.refreshTokenObservable.subscribe((data: IToken) => {
      self.storeTokenInfo(data);
      this.refreshTokenApiCall(0);
    },
      (error: any) => {
        this.logout(true).subscribe((data: any) => {
          this.router.navigate(['/']);
        },
          error => {
          },
          () => {
            this.refreshTokenApiCall(0);
          });
      },
      () => {
        this.refreshTokenObservable = undefined;
      });
  }

  forgotPassword(dto: IForgotPassword): Observable<any> {
    var self = this;
    var observable = this.httpClient.post(self.baseUrl + AccountControllerAPI.ForgotPassword(), dto, { responseType: "text" })
      .pipe(share(), map((data: any, index: any) => { return data; }));

    observable.subscribe((data: IToken) => {
      notifyUser(self.pushNotifications, self.toaster, NotificationType.Info, "Forgot Password", "Email sent to reset password.");
    },
      (error: any) => {
        console.log(error);
        createToastFromServiceResponse(error, self.pushNotifications, self.toaster, "Forgot Password", "Error occurred, try again.")
      },
      () => {
      });

    return observable;
  }

  resetPassword(dto: IResetPassword): Observable<any> {
    var self = this;
    var observable = this.httpClient.post(self.baseUrl + AccountControllerAPI.ResetPassword(), dto, { responseType: "text" })
      .pipe(share(), map((data: any, index: any) => { return data; }));

    observable.subscribe((data: IToken) => {
      notifyUser(self.pushNotifications, self.toaster, NotificationType.Info, "Reset Password", "Password has been reset.");
    },
      (error: any) => {
        console.log(error);
        createToastFromServiceResponse(error, self.pushNotifications, self.toaster, "Reset Password", "Error trying to reset password.")
      },
      () => {
      });

    return observable;
  }

  changePassword(dto: IChangePassword): Observable<any> {
    var self = this;
    var observable = this.httpClient.post(self.baseUrl + AccountControllerAPI.ChangePassword(), dto, { responseType: "text", withCredentials:true })
      .pipe(share(), map((data, index) => { return data; }));

    observable.subscribe((data) => {
      notifyUser(self.pushNotifications, self.toaster, NotificationType.Info, "Change Password", "Password has been changed.");
    },
      (error: any) => {
        console.log(error);
        createToastFromServiceResponse(error, self.pushNotifications, self.toaster, "Change Password", "Error trying to change password.")
      },
      () => {
      });

    return observable;
  }

  logout(suppressNotifications: boolean): Observable<any> {
    var self = this;
    var observable = this.httpClient.post(self.baseUrl + AccountControllerAPI.Logout(), undefined, { responseType: "text", withCredentials: true  })
      .pipe(share(), map((data: any, index: any) => { return data; }));

    observable.subscribe((data: any) => {
      this.apiCache.clearCache();
      if (!suppressNotifications) {
        notifyUser(self.pushNotifications, self.toaster, NotificationType.Warn, "Logout", "You are logged out!");
      }
    },
      (error: any) => {
        console.log(error);
        if (!suppressNotifications) {
          createToastFromServiceResponse(error, self.pushNotifications, self.toaster, "Logout", "Error occurred while logging out.")
        }
      },
      () => {
        this.clearInfo();
      });

    return observable;
  }

  register(dto: IRegister): Observable<any> {
    var self = this;

    var observable = this.httpClient.post(self.baseUrl + AccountControllerAPI.Register(), dto, { responseType: "text" })
      .pipe(share(), map((data: any, index: any) => { return data; }));

    observable.subscribe((data: IToken) => {
      this.apiCache.clearCacheByPath(UsersControllerAPI.GetUsers());
      notifyUser(self.pushNotifications, self.toaster, NotificationType.Info, "Register", "Verification email has been sent.");
    },
      (error: any) => {
        console.log(error);
        createToastFromServiceResponse(error, self.pushNotifications, self.toaster, "Register", "Error occurred, please try again.")
      },
      () => {
      });

    return observable;
  }

  confirmEmail(dto: IConfirmEmail): Observable<any> {
    var self = this;
    var observable = this.httpClient.get(self.baseUrl + AccountControllerAPI.ConfirmUserEmail((<number>dto.userId).toString(), <string>dto.code), { responseType: "text" })
      .pipe(share(), map((data: any, index: any) => { return data; }));

    observable.subscribe((data: IToken) => {
    },
      (error: any) => {
        console.log(error);
        createToastFromServiceResponse(error, self.pushNotifications, self.toaster, "Confirm Email", "Error occurred, please try again.")
      },
      () => {
      });

    return observable;
  }

  setup2fa(dto: ITwoFactorSetup): Observable<any>  {
    var self = this;
    var observable = this.httpClient.post(self.baseUrl + AccountControllerAPI.TwoFactorSetup(), dto, { responseType: "text" })
      .pipe(share(), map((data: any, index: any) => { return data; }));

    observable.subscribe((data: string | undefined) => {
      notifyUser(self.pushNotifications, self.toaster, NotificationType.Info, "Two Factor Setup", "Two Factor Setup has been completed successfully.");

      this.router.navigate(["twofactor"]);
    },
      (error: any) => {
        console.log(error);
        createToastFromServiceResponse(error, self.pushNotifications, self.toaster, "Two Factor Setup", "Error occurred, please try again.")
      },
      () => {
      });

    return observable;
  }

  loginWith2fa(dto: ITwoFactorCode): Observable<any> {
    var self = this;
    dto.rememberMe = this.rememberMe;
    var observable = this.httpClient.post(self.baseUrl + AccountControllerAPI.LoginWith2fa(), dto)
      .pipe(share(), map((data: any, index: any) => { return data; }));

    observable.subscribe((data: IToken) => {
      notifyUser(self.pushNotifications, self.toaster, NotificationType.Info, "Two Factor Login", "Successful");
      this.rememberMe = undefined;

      self.storeTokenInfo(data);
    },
      (error: any) => {
        console.log(error);
        createToastFromServiceResponse(error, self.pushNotifications, self.toaster, "Two Factor Login", "Error occurred, please try again.")
      },
      () => {
      });

    return observable;
  }

  /**
   * Impersonation.  Validated on the server!
   * @param companyId companyId to impersonate if passed in
   * @param userId userId to impersonate if passed in.
   */
  impersonate(companyId: number | undefined, userId: number | undefined): Observable<IToken> {
    // role check for UX only.  validated on server
    if (!this.canImpersonate) {
      return of();
    }
    var self = this;
    var observable = this.httpClient.post<IToken>(self.baseUrl + AccountControllerAPI.Impersonate(), { companyId: companyId, userId: userId }, { withCredentials: true })
      .pipe(share(), map((data: any, index: any) => { return data; }));

    observable.subscribe((data: IToken) => {
      self.storeTokenInfo(data);
      let message:string = "";
      if (this.userInfo != null) {
        message = "Impersonation successful";
        if (this.userInfo.isImpersonatingCompany) {
          message = `${message}<br/>Company:  ${this.userInfo.companyName}`;
        }
        if (this.userInfo.isImpersonatingUser) {
          message = `${message}<br/>Name:  ${this.userInfo.firstName} ${this.userInfo.lastName}`;
        }

        if (!this.userInfo.isImpersonatingCompany && !this.userInfo.isImpersonatingUser){
          message = "Impersonation turned off successfully.";
        }
      }
      else {
        message = "User information not found."
      }

      self.apiCache.clearCache();

      notifyUser(self.pushNotifications, self.toaster, NotificationType.Info, "Impersonate", message); 

      self.router.navigate(["/"]);
    },
      (error: any) => {
        console.log(error);
        createToastFromServiceResponse(error, self.pushNotifications, self.toaster, "Impersonate", "Error occurred, please try again.")
      },
      () => {
      });

    return observable;
  }

  get isAuthenticated(): boolean {
    return this.token != null && this.validTo != null && this.validTo.utc().isSameOrAfter(moment().utc());
  }

  private isRefreshTokenApiAlreadyRunning(): boolean {
    if (sessionStorage.getItem(sessionStorageRefreshTokenCallRunning) === "1") {
      var callStartDateTime = sessionStorage.getItem(sessionStorageRefreshTokenCallStartedDateTime);
      if (callStartDateTime != null) {
        var lastCallDateTime = moment(callStartDateTime);
        return lastCallDateTime.add(1, "minute") > moment();
      }
    }
    return false;
  }

  private storageEventHandler(storageEvent: StorageEvent) {
    if (storageEvent.key === localStorageRefreshTokenCallStarted) {
      var value = storageEvent.newValue || "0";
      sessionStorage.setItem(sessionStorageRefreshTokenCallRunning, value);
      sessionStorage.removeItem(sessionStorageRefreshTokenCallStartedDateTime)
      if (value === "1") {
        sessionStorage.setItem(sessionStorageRefreshTokenCallStartedDateTime, new Date().toISOString());
      }
    }
  }

  private refreshTokenApiCall(status: number) {
    sessionStorage.removeItem(sessionStorageRefreshTokenCallStartedDateTime)
    switch (status) {
      case 0:
      default:
        localStorage.removeItem(localStorageRefreshTokenCallStarted);
        break;
      case 1:
        localStorage.setItem(localStorageRefreshTokenCallStarted, status.toString());
        sessionStorage.setItem(sessionStorageRefreshTokenCallStartedDateTime, new Date().toISOString());
        break;
    }
  }

  private setupRefreshTokenTimer(): void {
    if (this.refreshTokenIntervalId != null) {
      clearInterval(this.refreshTokenIntervalId);
    }

    if (this.validTo == null) return;

    // less than X minutes left, refresh now
    if (this.validTo.utc().isBefore(moment().utc().add(refreshTokenIntervalMinutes, "minute"))) {
      this.refreshUserToken();
      return;
    }

    this.refreshTokenIntervalId = setInterval(() => {
      this.refreshUserToken();
    }, refreshTokenIntervalMinutes * 60 * 1000);
  }

  storeTokenInfo(data: IToken) {
    if (data != null) {
      localStorage.setItem(localStorageUserIdentityToken, data.token || "");
      localStorage.setItem(localStorageUserIdentityTokenValidTo, (<Date>data.validTo).toString());
      localStorage.setItem(localStorageUserIdentityRefreshToken, data.refreshToken || "");

      this._userRoles = data.roles;
      localStorage.setItem(localStorageUserRoles, JSON.stringify(data.roles));

      this._userPolicies = data.userPolicies;
      localStorage.setItem(localStorageUserPolicies, JSON.stringify(data.userPolicies));

      this._userInfo = data.userInfo;
      localStorage.setItem(localStorageUserInformation, JSON.stringify(data.userInfo));

      this.setupRefreshTokenTimer();
    }
  }

  clearInfo(): void {
    if (this.refreshTokenIntervalId != null) {
      clearInterval(this.refreshTokenIntervalId);
    }

    localStorage.removeItem(localStorageUserIdentityToken);
    localStorage.removeItem(localStorageUserIdentityTokenValidTo);
    localStorage.removeItem(localStorageUserIdentityRefreshToken);
    localStorage.removeItem(localStorageUserInformation);
    localStorage.removeItem(localStorageUserRoles);
    localStorage.removeItem(localStorageUserPolicies);
    localStorage.removeItem(localStorageUserInformation);

    this._userRoles = undefined;
    this._userPolicies = undefined;
    this._userInfo = undefined;
  }

  /** NEVER pass to the server */
  get userRoles(): Array<string> | undefined {
    if (this._userRoles) return this._userRoles;

    var value = localStorage.getItem(localStorageUserRoles);
    if (value != null) {
      this._userRoles = JSON.parse(value);
    }
    return this._userRoles;
  }

  /** Server MUST validate, for UX only */
  get isAdmin(): boolean {
    var value = this.userRoles;
    if (value != null)
      return value.findIndex((item) => { return item === admin; }) > -1;
    return false;
  }

  /** Server MUST validate, for UX only */
  get isRegistrar(): boolean {
    var value = this.userRoles;
    if (value != null)
      return value.findIndex((item) => { return item === registrar; }) > -1;
    return false;
  }

  /** Server MUST validate, for UX only */
  get isLCO(): boolean {
    var value = this.userRoles;
    if (value != null)
      return value.findIndex((item) => { return item === lco; }) > -1;
    return false;
  }

  /** NEVER pass to the server */
  get userPolicies(): IUserPolicies | undefined {
    if (this._userPolicies) return this._userPolicies;

    var value = localStorage.getItem(localStorageUserPolicies);
    if (value != null) {
      this._userPolicies = JSON.parse(value);
    }
    return this._userPolicies;
  }

  /** NEVER pass to the server */
  get userInfo(): ITokenUser | null | undefined {
    if (this._userInfo) return this._userInfo;

    var value = localStorage.getItem(localStorageUserInformation);
    if (value) {
      return JSON.parse(value);
    }
    return null;
  }

  /** Server MUST validate, for UX only */
  get canApprovePicture(): boolean {
    return this.checkPolicyAccess(canApprovePicture);
  }

  /** Server MUST validate, for UX only */
  get canImpersonate(): boolean {
    return this.checkPolicyAccess(canImpersonate);
  }

  /** Server MUST validate, for UX only */
  get announcementAddEditDelete(): boolean {
    return this.checkPolicyAccess(announcementAddEditDelete);
  }
  
  /** Server MUST validate, for UX only */
  get eventAddEditDelete(): boolean {
    return this.checkPolicyAccess(eventAddEditDelete);
  }

  /** Server MUST validate, for UX only */
  get locationAddEditDelete(): boolean {
    return this.checkPolicyAccess(locationAddEditDelete);
  }

  /** Server MUST validate, for UX only */
  get pictureAddEditDelete(): boolean {
    return this.checkPolicyAccess(pictureAddEditDelete);
  }

  /** Server MUST validate, for UX only */
  get taskAddEditDelete(): boolean {
    return this.checkPolicyAccess(taskAddEditDelete);
  }

  /** Server MUST validate, for UX only */
  get userAddEditDelete(): boolean {
    return this.checkPolicyAccess(userAddEditDelete);
  }

  /** Server MUST validate, for UX only */
  checkPolicyAccess(policyName: string): boolean {
    if (this.userPolicies == null) return false;
    policyName = policyName.charAt(0).toLowerCase() + policyName.substring(1);
    return (<any>this.userPolicies)[policyName] || false;
  }

  get token(): string {
    var value = localStorage.getItem(localStorageUserIdentityToken);
    if (value != null)
      return value;
    return "";
  }

  get refreshToken(): string {
    var value = localStorage.getItem(localStorageUserIdentityRefreshToken);
    if (value != null)
      return value;
    return "";
  }

  get validTo(): moment.Moment | undefined {
    var dateString = localStorage.getItem(localStorageUserIdentityTokenValidTo)
    if (dateString != null) {
      var validTo = moment(dateString);
      return validTo;
    }

    return undefined;
  }

  set rememberMe(value: boolean | undefined) {
    if (value)
      sessionStorage.setItem(sessionStoreageRememberMe, value.toString());
    else
      sessionStorage.removeItem(sessionStoreageRememberMe);
  }

  get rememberMe(): boolean | undefined {
    var value = sessionStorage.getItem(sessionStoreageRememberMe)
    return value == "true" || undefined;
  }

  getRedirectUrl(): string {
    console.log("redirect Url:  " + this.redirectUrl);
    return this.redirectUrl || '/';
  }
}
