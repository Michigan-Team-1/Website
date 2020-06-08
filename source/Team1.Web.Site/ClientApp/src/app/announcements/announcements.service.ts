import { Injectable, Inject } from '@angular/core';
import { Observable, of } from "rxjs";
import { share, map } from 'rxjs/Operators';
import { HttpClient } from '@angular/common/http';

import { NotificationsService } from 'angular2-notifications';
import { notifyUser, createToastFromServiceResponse } from "app.common/helpers/Toaster";

import { ApiCache } from 'app.common/cache/api.cache';

import { ISelectOption } from 'app.common/dtos/SelectOptionDto';
import { AnnouncementsControllerAPI } from 'app.common/apis/AnnouncementsController';
import { IAnnouncement, IAnnouncement_PrepareDto } from 'app.common/dtos/AnnouncementDto';

@Injectable()
export class AnnouncementsService {
  constructor(private httpClient: HttpClient, @Inject('baseUrl') private baseUrl: string,
    private toaster: NotificationsService,
    private apiCache: ApiCache) {
  }
  pushNotifications: any;
  defaultToastTitle: string = `Announcements`;

  getAnnouncements(): Observable<Array<IAnnouncement>> {
    var apiUrl = AnnouncementsControllerAPI.GetAnnouncements();
    var cacheData = this.apiCache.getCachedItemIfNotExpired(apiUrl);
    if (cacheData != null) return of(cacheData);

    let observable = this.httpClient.get<Array<IAnnouncement>>(this.baseUrl + apiUrl, { withCredentials: true })
      .pipe(share(), map((data:any, index) => {
        for (var i = 0; i < data.length; i++) {
          data[i] = IAnnouncement_PrepareDto(data[i]);
        }
        return data;
      }));

    observable
      .subscribe((data: Array<IAnnouncement>) => {
        this.apiCache.setCachedItem(apiUrl, data);
      },
        (error: any) => {
          console.log(error);
          createToastFromServiceResponse(error, this.pushNotifications, this.toaster, this.defaultToastTitle, "Error occurred while loading announcements.")
        },
        () => {
        });

    return observable;
  }

  getAnnouncementsForDashboard(): Observable<Array<IAnnouncement>> {
    var apiUrl = AnnouncementsControllerAPI.GetAnnouncementsForDashboard();
    var cacheData = this.apiCache.getCachedItemIfNotExpired(apiUrl);
    if (cacheData != null) return of(cacheData);

    let observable = this.httpClient.get<Array<IAnnouncement>>(this.baseUrl + apiUrl, { withCredentials: true })
      .pipe(share(), map((data:any, index) => {
        for (var i = 0; i < data.length; i++) {
          data[i] = IAnnouncement_PrepareDto(data[i]);
        }
        return data;
      }));

    observable
      .subscribe((data: Array<IAnnouncement>) => {
        this.apiCache.setCachedItem(apiUrl, data);
      },
        (error: any) => {
          console.log(error);
          createToastFromServiceResponse(error, this.pushNotifications, this.toaster, this.defaultToastTitle, "Error occurred while loading announcements.")
        },
        () => {
      });

    return observable;
  }

  saveAnnouncement(dto: IAnnouncement): Observable<IAnnouncement> {
    if (!dto.isUpdated) {
      return of(dto);
    }

    let observable: Observable<IAnnouncement>;
    if (dto.announcementId == 0) {
      observable = this.httpClient.post<IAnnouncement>(this.baseUrl + AnnouncementsControllerAPI.CreateAnnouncement(), dto, { withCredentials: true })
        .pipe(share(), map((data:any, index) => { return IAnnouncement_PrepareDto(data); }));
    } else {
      observable = this.httpClient.put<IAnnouncement>(this.baseUrl + AnnouncementsControllerAPI.UpdateAnnouncement(), dto, { withCredentials: true })
        .pipe(share(), map((data:any, index) => { return IAnnouncement_PrepareDto(data); }));
    }

    observable.subscribe((data: IAnnouncement) => {
      // clear cache
      this.apiCache.clearCacheByUrl(AnnouncementsControllerAPI.GetAnnouncements());
    },
      (error: any) => {
        console.log(error);
        createToastFromServiceResponse(error, this.pushNotifications, this.toaster, this.defaultToastTitle, "Error occurred while saving the announcement.")
      },
      () => {
      });

    return observable;
  }
}
