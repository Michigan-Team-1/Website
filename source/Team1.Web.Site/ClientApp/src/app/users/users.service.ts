import { Injectable, Inject } from '@angular/core';
import { Router } from "@angular/router";
import { Observable, of } from "rxjs";
import { share, map } from 'rxjs/Operators';
import { HttpClient } from '@angular/common/http';

import { NotificationsService } from 'angular2-notifications';
import { notifyUser, createToastFromServiceResponse } from "app.common/helpers/Toaster";

import { ApiCache } from 'app.common/cache/api.cache';

import { AccountControllerAPI } from 'app.common/apis/AccountController'
import { UsersControllerAPI } from 'app.common/apis/UsersController'
import { RolesControllerAPI } from 'app.common/apis/RolesController'
import { AddressesControllerAPI } from 'app.common/apis/AddressesController'

import { IUser, IUser_PrepareDto } from 'app.common/dtos/UserDto';
import { IRole } from 'app.common/dtos/RoleDto';
import { jsonParseAndToCamelCase } from 'app.common/helpers/object';

@Injectable()
export class UsersService {
  constructor(private httpClient: HttpClient, @Inject('baseUrl') private baseUrl: string,
    private toaster: NotificationsService,
    private apiCache: ApiCache) {
  }
  pushNotifications: any;
  defaultToastTitle: string = `Users`;

  getUsers(): Observable<Array<IUser>> {
    var apiUrl = UsersControllerAPI.GetUsers();
    var cacheData = this.apiCache.getCachedItemIfNotExpired(apiUrl);
    if (cacheData != null) return of(cacheData);

    let observable = this.httpClient.get<Array<IUser>>(this.baseUrl + apiUrl, { withCredentials: true })
      .pipe(share(), map((data, index) => {
        for (var i = 0; i < data.length; i++) {
          data[i] = IUser_PrepareDto(data[i]);
        }
        return data; }));

    observable
      .subscribe((data: Array<IUser>) => {
        this.apiCache.setCachedItem(apiUrl, data);
      },
        (error: any) => {
          console.log(error);
          createToastFromServiceResponse(error, this.pushNotifications, this.toaster, this.defaultToastTitle, "Error occurred while loading users.")
        },
        () => {
        });

    return observable;
  }
  
  getBoardOfDirectors(): Observable<Array<IUser>> {
    var apiUrl = UsersControllerAPI.GetBoardOfDirectors();
    var cacheData = this.apiCache.getCachedItemIfNotExpired(apiUrl);
    if (cacheData != null) return of(cacheData);

    let observable = this.httpClient.get<Array<IUser>>(this.baseUrl + apiUrl, { withCredentials: true })
      .pipe(share(), map((data, index) => {
        for (var i = 0; i < data.length; i++) {
          data[i] = IUser_PrepareDto(data[i]);
        }
        return data;
      }));

    observable
      .subscribe((data: Array<IUser>) => {
        this.apiCache.setCachedItem(apiUrl, data);
      },
        (error: any) => {
          console.log(error);
          //createToastFromServiceResponse(error, this.pushNotifications, this.toaster, this.defaultToastTitle, "Error occurred while loading users.")
        },
        () => {
        });

    return observable;
  }

  saveUser(dto: IUser): Observable<IUser> {
    if (!dto.isUpdated) {
      return of(dto);
    }

    let observable: Observable<IUser>;
    if (dto.userId == 0) {
      observable = this.httpClient.post<IUser>(this.baseUrl + UsersControllerAPI.CreateUser(), dto, { withCredentials: true })
        .pipe(share(), map((data, index) => { return IUser_PrepareDto(data); }));
    } else {
      observable = this.httpClient.put<IUser>(this.baseUrl + UsersControllerAPI.UpdateUser(), dto, { withCredentials: true })
        .pipe(share(), map((data, index) => { return IUser_PrepareDto(data); }));
    }

    observable.subscribe((data: IUser) => {
      // clear cache
      this.apiCache.clearCacheByUrl(UsersControllerAPI.GetUsers());
      this.apiCache.clearCacheByUrl(UsersControllerAPI.GetBoardOfDirectors());
      this.apiCache.clearCacheByUrl(AddressesControllerAPI.GetUserAddresses(<number>dto.userId));
    },
      (error: any) => {
        console.log(error);
        createToastFromServiceResponse(error, this.pushNotifications, this.toaster, this.defaultToastTitle, "Error occurred while saving the user.")
      },
      () => {
      });

    return observable;
  }

  getGrantableRoles(): Observable<Array<IRole>> {
    var apiUrl = RolesControllerAPI.GetGrantableRoles();
    var cacheData = this.apiCache.getCachedItemIfNotExpired(apiUrl, 60);
    if (cacheData != null) return of(cacheData);

    let observable = this.httpClient.get<Array<IRole>>(this.baseUrl + apiUrl, { withCredentials: true }).pipe(share());

    observable.pipe(map((data, index) => {
      for (var i = 0; i < data.length; i++) {
        if (data[i].data)
          data[i].dataObj = jsonParseAndToCamelCase(data[i].data);
      }
      return data;
    }))
      .subscribe((data: Array<IRole>) => {
        this.apiCache.setCachedItem(apiUrl, data);
      },
        (error: any) => {
          console.log(error);
          createToastFromServiceResponse(error, this.pushNotifications, this.toaster, this.defaultToastTitle, "Error occurred while loading roles.")
        },
        () => {
        });

    return observable;
  }

  sendPasswordResetEmail(userId: number, isNew: boolean): Observable<boolean> {
    let observable = this.httpClient.get<boolean>(this.baseUrl + AccountControllerAPI.SendPasswordResetEmail(userId, isNew), { withCredentials: true })
      .pipe(share(), map((data, index) => { return data; }));

    observable.subscribe((data: boolean) => {
      if (data) {
        this.toaster.success(this.defaultToastTitle, "Email sent successfully!");
      }
      else {
        this.toaster.error(this.defaultToastTitle, "Email not sent.");
      }
      return data;
    },
      (error: any) => {
        console.log(error);
        createToastFromServiceResponse(error, this.pushNotifications, this.toaster, this.defaultToastTitle, "Error occurred while sending the password reset email.")
      },
      () => {
      });

    return observable;
  }
}
