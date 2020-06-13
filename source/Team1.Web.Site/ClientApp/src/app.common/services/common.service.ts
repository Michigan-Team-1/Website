import { Injectable, Inject } from '@angular/core';
import { Router } from "@angular/router";
import { Observable, of } from "rxjs";
import { map, share } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';

import { NotificationsService } from 'angular2-notifications';

import { notifyUser, createToastFromServiceResponse } from "app.common/helpers/Toaster";

import { AddressesControllerAPI } from 'app.common/apis/AddressesController'
import { CountriesControllerAPI } from 'app.common/apis/CountriesController'
import { GoverningDistrictsControllerAPI } from 'app.common/apis/GoverningDistrictsController'

import { IAddress } from 'app.common/dtos/AddressDto';
import { ICountry } from 'app.common/dtos/CountryDto';
import { IGoverningDistrict } from 'app.common/dtos/GoverningDistrictDto';

import { ApiCache } from 'app.common/cache/api.cache';
import { ISelectOption } from '../dtos/SelectOptionDto';
import { MobileCarriersControllerAPI } from '../apis/MobileCarriersController';

@Injectable()
export class CommonService {
  constructor(private httpClient: HttpClient, @Inject('baseUrl') private baseUrl: string,
    private toaster: NotificationsService,
    private apiCache: ApiCache) {
  }

  pushNotifications: any;

  getUserAddresses(userId: number): Observable<Array<IAddress>> {
    var apiUrl = AddressesControllerAPI.GetUserAddresses(userId);
    var cacheData = this.apiCache.getCachedItemIfNotExpired(apiUrl);
    if (cacheData != null) return of(cacheData);

    let observable = this.httpClient.get<Array<IAddress>>(this.baseUrl + apiUrl, { withCredentials: true })
      .pipe(share(), map((data: any, index: any) => { return data; }));

    observable.subscribe((data: Array<IAddress>) => {
      this.apiCache.setCachedItem(apiUrl, data);
    },
      (error: any) => {
        console.log(error);
        createToastFromServiceResponse(error, this.pushNotifications, this.toaster, "Addresses", "Error occurred while loading addresses.")
      },
      () => {
      });

    return observable;
  }

  getCountries(): Observable<Array<ICountry>> {
    var apiUrl = CountriesControllerAPI.GetCountries();
    var cacheData = this.apiCache.getCachedItemIfNotExpired(apiUrl, 60);
    if (cacheData != null) return of(cacheData);

    let observable = this.httpClient.get<Array<ICountry>>(this.baseUrl + apiUrl, { withCredentials: false })
      .pipe(share(), map((data: any, index: any) => { return data; }));

    observable.subscribe((data: Array<ICountry>) => {
      this.apiCache.setCachedItem(apiUrl, data);
    },
      (error: any) => {
        console.log(error);
        createToastFromServiceResponse(error, this.pushNotifications, this.toaster, "Countries", "Error occurred while loading countries.")
      },
      () => {
      });

    return observable;
  }

  getGoverningDistricts(): Observable<Array<IGoverningDistrict>> {
    var apiUrl = GoverningDistrictsControllerAPI.GetGoverningDistricts();
    var cacheData = this.apiCache.getCachedItemIfNotExpired(apiUrl, 60);
    if (cacheData != null) return of(cacheData);

    let observable = this.httpClient.get<Array<IGoverningDistrict>>(this.baseUrl + apiUrl, { withCredentials: false })
      .pipe(share(), map((data: any, index: any) => { return data; }));

    observable.subscribe((data: Array<IGoverningDistrict>) => {
      this.apiCache.setCachedItem(apiUrl, data);
    },
      (error: any) => {
        console.log(error);
        createToastFromServiceResponse(error, this.pushNotifications, this.toaster, "Governing Districts", "Error occurred while loading governing districts.")
      },
      () => {
      });

    return observable;
  }

  getMobileCarriers(): Observable<Array<ISelectOption<number>>> {
    var apiUrl = MobileCarriersControllerAPI.GetMobileCarriers();
    var cacheData = this.apiCache.getCachedItemIfNotExpired(apiUrl, 60);
    if (cacheData != null) return of(cacheData);

    let observable = this.httpClient.get<Array<ISelectOption<number>>>(this.baseUrl + apiUrl, { withCredentials: false })
      .pipe(share(), map((data: any, index: any) => { return data; }));

    observable.subscribe((data: Array<ISelectOption<number>>) => {
      this.apiCache.setCachedItem(apiUrl, data);
    },
      (error: any) => {
        console.log(error);
        createToastFromServiceResponse(error, this.pushNotifications, this.toaster, "Countries", "Error occurred while loading mobile carriers.")
      },
      () => {
      });

    return observable;
  }
}
