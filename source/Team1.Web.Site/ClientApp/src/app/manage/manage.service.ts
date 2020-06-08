import { Injectable, Inject } from '@angular/core';
import { Router } from "@angular/router";
import { Observable, of } from "rxjs";
import { share, map } from 'rxjs/Operators';
import { HttpClient } from '@angular/common/http';

import { NotificationsService } from 'angular2-notifications';
import { notifyUser, createToastFromServiceResponse } from "app.common/helpers/Toaster";

import { ApiCache } from 'app.common/cache/api.cache';

import { IUser } from 'app.common/dtos/UserDto';
import { IRole } from 'app.common/dtos/RoleDto';
import { jsonParseAndToCamelCase } from 'app.common/helpers/object';
import { IUserProfile, IUserProfile_PrepareDto } from 'app.common/dtos/UserProfileDto';
import { AddressesControllerAPI } from 'app.common/apis/AddressesController';
import { ManageControllerAPI } from 'app.common/apis/ManageController';
import { UsersControllerAPI } from 'app.common/apis/UsersController';

@Injectable()
export class ManageService {
  constructor(private httpClient: HttpClient, @Inject('baseUrl') private baseUrl: string,
    private toaster: NotificationsService,
    private apiCache: ApiCache) {
  }
  pushNotifications: any;
  defaultToastTitle: string = `User Profile`;

  getUserProfile(): Observable<IUserProfile> {
    var apiUrl = ManageControllerAPI.GetUserProfile();
    var cacheData = this.apiCache.getCachedItemIfNotExpired(apiUrl);
    if (cacheData != null) return of(cacheData);

    let observable = this.httpClient.get<IUserProfile>(this.baseUrl + apiUrl, { withCredentials: true })
      .pipe(share(), map((data: any, index: any) => { return IUserProfile_PrepareDto(data); }));

    observable
      .subscribe((data: IUserProfile) => {
        this.apiCache.setCachedItem(apiUrl, data);
      },
        (error: any) => {
          console.log(error);
          createToastFromServiceResponse(error, this.pushNotifications, this.toaster, this.defaultToastTitle, "Error occurred while loading the user profile.")
        },
        () => {
        });

    return observable;
  }

  saveUserProfile(dto: IUserProfile): Observable<IUserProfile> {
    if (!dto.isUpdated) {
      return of(dto);
    }

    let observable = this.httpClient.put<IUserProfile>(this.baseUrl + ManageControllerAPI.UpdateUserProfile(), dto, { withCredentials: true })
      .pipe(share(), map((data: any, index: any) => { return IUserProfile_PrepareDto(data); }));

    observable.subscribe((data: IUser) => {
      // clear cache
      this.toaster.success(this.defaultToastTitle, "Save completed.");
      this.apiCache.clearCacheByUrl(UsersControllerAPI.GetUsers());
      this.apiCache.clearCacheByUrl(ManageControllerAPI.GetUserProfile());
      this.apiCache.clearCacheByUrl(AddressesControllerAPI.GetUserAddresses(<number>dto.userId));
    },
      (error: any) => {
        console.log(error);
        createToastFromServiceResponse(error, this.pushNotifications, this.toaster, this.defaultToastTitle, "Error occurred while saving the user profile.")
      },
      () => {
      });

    return observable;
  }

}
