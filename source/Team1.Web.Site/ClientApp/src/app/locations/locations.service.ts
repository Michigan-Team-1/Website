import { Injectable, Inject } from '@angular/core';
import { Router } from "@angular/router";
import { Observable, of } from "rxjs";
import { share, map } from 'rxjs/Operators';
import { HttpClient } from '@angular/common/http';

import { NotificationsService } from 'angular2-notifications';
import { notifyUser, createToastFromServiceResponse } from "app.common/helpers/Toaster";

import { ApiCache } from 'app.common/cache/api.cache';

import { ILocation, ILocation_PrepareDto } from 'app.common/dtos/LocationDto';
import { LocationsControllerAPI } from 'app.common/apis/LocationsController';
import { SelectControlValueAccessor } from '@angular/forms';
import { ISelectOption } from 'app.common/dtos/SelectOptionDto';

@Injectable()
export class LocationsService {
  constructor(private httpClient: HttpClient, @Inject('baseUrl') private baseUrl: string,
    private toaster: NotificationsService,
    private apiCache: ApiCache) {
  }
  pushNotifications: any;
  defaultToastTitle: string = `Locations`;

  getLocations(): Observable<Array<ILocation>> {
    var apiUrl = LocationsControllerAPI.GetLocations();
    var cacheData = this.apiCache.getCachedItemIfNotExpired(apiUrl);
    if (cacheData != null) return of(cacheData);

    let observable = this.httpClient.get<Array<ILocation>>(this.baseUrl + apiUrl, { withCredentials: true })
      .pipe(share(), map((data:any, index) => {
        for (var i = 0; i < data.length; i++) {
          data[i] = ILocation_PrepareDto(data[i]);
        }
        return data;
      }));

    observable
      .subscribe((data: Array<ILocation>) => {
        this.apiCache.setCachedItem(apiUrl, data);
      },
        (error: any) => {
          console.log(error);
          createToastFromServiceResponse(error, this.pushNotifications, this.toaster, this.defaultToastTitle, "Error occurred while loading locations.")
        },
        () => {
        });

    return observable;
  }

  getLocationsForSelection(): Observable<Array<ISelectOption<number>>> {
    var apiUrl = LocationsControllerAPI.GetLocationsForSelection();
    var cacheData = this.apiCache.getCachedItemIfNotExpired(apiUrl);
    if (cacheData != null) return of(cacheData);

    let observable = this.httpClient.get<Array<ISelectOption<number>>>(this.baseUrl + apiUrl, { withCredentials: true })
      .pipe(share(), map((data:any, index) => {
        return data;
      }));

    observable
      .subscribe((data: Array<ISelectOption<number>>) => {
        this.apiCache.setCachedItem(apiUrl, data);
      },
        (error: any) => {
          console.log(error);
          createToastFromServiceResponse(error, this.pushNotifications, this.toaster, this.defaultToastTitle, "Error occurred while loading locations.")
        },
        () => {
      });

    return observable;
  }

  saveLocation(dto: ILocation): Observable<ILocation> {
    if (!dto.isUpdated) {
      return of(dto);
    }

    let observable: Observable<ILocation>;
    if (dto.locationId == 0) {
      observable = this.httpClient.post<ILocation>(this.baseUrl + LocationsControllerAPI.CreateLocation(), dto, { withCredentials: true })
        .pipe(share(), map((data:any, index) => { return ILocation_PrepareDto(data); }));
    } else {
      observable = this.httpClient.put<ILocation>(this.baseUrl + LocationsControllerAPI.UpdateLocation(), dto, { withCredentials: true })
        .pipe(share(), map((data:any, index) => { return ILocation_PrepareDto(data); }));
    }

    observable.subscribe((data: ILocation) => {
      // clear cache
      this.apiCache.clearCacheByUrl(LocationsControllerAPI.GetLocations());
    },
      (error: any) => {
        console.log(error);
        createToastFromServiceResponse(error, this.pushNotifications, this.toaster, this.defaultToastTitle, "Error occurred while saving the locations.")
      },
      () => {
      });

    return observable;
  }
}
