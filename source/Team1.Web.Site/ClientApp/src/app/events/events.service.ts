import { Injectable, Inject } from '@angular/core';
import { Router } from "@angular/router";
import { Observable, of } from "rxjs";
import { share, map } from 'rxjs/Operators';
import { HttpClient } from '@angular/common/http';

import { NotificationsService } from 'angular2-notifications';
import { notifyUser, createToastFromServiceResponse } from "app.common/helpers/Toaster";

import { ApiCache } from 'app.common/cache/api.cache';

import { EventsControllerAPI } from 'app.common/apis/EventsController';
import { IEvent, IEvent_PrepareDto } from 'app.common/dtos/EventDto';

@Injectable()
export class EventsService {
  constructor(private httpClient: HttpClient, @Inject('baseUrl') private baseUrl: string,
    private toaster: NotificationsService,
    private apiCache: ApiCache) {
  }
  pushNotifications: any;
  defaultToastTitle: string = `Events`;

  getEvents(): Observable<Array<IEvent>> {
    var apiUrl = EventsControllerAPI.GetEvents();
    var cacheData = this.apiCache.getCachedItemIfNotExpired(apiUrl);
    if (cacheData != null) return of(cacheData);

    let observable = this.httpClient.get<Array<IEvent>>(this.baseUrl + apiUrl, { withCredentials: true })
      .pipe(share(), map((data, index) => {
        for (var i = 0; i < data.length; i++) {
          data[i] = IEvent_PrepareDto(data[i]);
        }
        return data;
      }));

    observable
      .subscribe((data: Array<IEvent>) => {
        this.apiCache.setCachedItem(apiUrl, data);
      },
        (error: any) => {
          console.log(error);
          createToastFromServiceResponse(error, this.pushNotifications, this.toaster, this.defaultToastTitle, "Error occurred while loading events.")
        },
        () => {
        });

    return observable;
  }

  getEventsForDashboard(): Observable<Array<IEvent>> {
    var apiUrl = EventsControllerAPI.GetEventsForDashboard();
    var cacheData = this.apiCache.getCachedItemIfNotExpired(apiUrl);
    if (cacheData != null) return of(cacheData);

    let observable = this.httpClient.get<Array<IEvent>>(this.baseUrl + apiUrl, { withCredentials: true })
      .pipe(share(), map((data, index) => {
        for (var i = 0; i < data.length; i++) {
          data[i] = IEvent_PrepareDto(data[i]);
        }
        return data;
      }));

    observable
      .subscribe((data: Array<IEvent>) => {
        this.apiCache.setCachedItem(apiUrl, data);
      },
        (error: any) => {
          console.log(error);
          createToastFromServiceResponse(error, this.pushNotifications, this.toaster, this.defaultToastTitle, "Error occurred while loading events.")
        },
        () => {
        });

    return observable;
  }

  saveEvent(dto: IEvent): Observable<IEvent> {
    if (!dto.isUpdated) {
      return of(dto);
    }

    let observable: Observable<IEvent>;
    if (dto.eventId == 0) {
      observable = this.httpClient.post<IEvent>(this.baseUrl + EventsControllerAPI.CreateEvent(), dto, { withCredentials: true })
        .pipe(share(), map((data, index) => { data = IEvent_PrepareDto(data); return data; }));
    } else {
      observable = this.httpClient.put<IEvent>(this.baseUrl + EventsControllerAPI.UpdateEvent(), dto, { withCredentials: true })
        .pipe(share(), map((data, index) => { data = IEvent_PrepareDto(data); return data; }));
    }

    observable.subscribe((data: IEvent) => {
      // clear cache
      this.apiCache.clearCacheByPath(EventsControllerAPI.GetEvents());
    },
      (error: any) => {
        console.log(error);
        createToastFromServiceResponse(error, this.pushNotifications, this.toaster, this.defaultToastTitle, "Error occurred while saving the event.")
      },
      () => {
      });

    return observable;
  }
}
