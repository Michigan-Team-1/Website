import { Injectable, Inject } from '@angular/core';
import { Router } from "@angular/router";
import { Observable, of } from "rxjs";
import { share, map } from 'rxjs/Operators';
import { HttpClient } from '@angular/common/http';

import { NotificationsService } from 'angular2-notifications';
import { createToastFromServiceResponse } from "app.common/helpers/Toaster";

import { ApiCache } from 'app.common/cache/api.cache';

import { IPicture, IPicture_PrepareDto } from 'app.common/dtos/PictureDto';
import { PicturesControllerAPI } from 'app.common/apis/PicturesController';
import { getFormDataForDtoAndFileUpload } from 'app.common/helpers/general';
import { ISelectOption } from 'app.common/dtos/SelectOptionDto';

@Injectable()
export class PicturesService {
  constructor(private httpClient: HttpClient, @Inject('baseUrl') private baseUrl: string,
    private toaster: NotificationsService,
    private apiCache: ApiCache) {
  }
  pushNotifications: any;
  defaultToastTitle: string = `Pictures`;
  
  getPictures(galleryType:number ): Observable<Array<IPicture>> {
    var apiUrl = PicturesControllerAPI.GetPictures(galleryType);
    var cacheData = this.apiCache.getCachedItemIfNotExpired(apiUrl);
    if (cacheData != null) return of(cacheData);

    let observable = this.httpClient.get<Array<IPicture>>(this.baseUrl + apiUrl, { withCredentials: true })
      .pipe(share(), map((data: Array<IPicture>, index) => {
        for (var i = 0; i < data.length; i++) {
          data[i] = IPicture_PrepareDto(data[i]);
        }
        return data;
      }));

    observable
      .subscribe((data: Array<IPicture>) => {
        this.apiCache.setCachedItem(apiUrl, data);
      },
        (error: any) => {
          console.log(error);
          createToastFromServiceResponse(error, this.pushNotifications, this.toaster, this.defaultToastTitle, "Error occurred while loading pictures.")
        },
        () => {
        });

    return observable;
  }

  getRandomPictures(): Observable<Array<IPicture>> {
    var apiUrl = PicturesControllerAPI.GetRandomPictures();

    let observable = this.httpClient.get<Array<IPicture>>(this.baseUrl + apiUrl, { withCredentials: true })
      .pipe(share(), map((data: Array<IPicture>, index) => {
        for (var i = 0; i < data.length; i++) {
          data[i] = IPicture_PrepareDto(data[i]);
        }
        return data;
      }));

    observable
      .subscribe((data: Array<IPicture>) => {
      },
        (error: any) => {
          console.log(error);
          createToastFromServiceResponse(error, this.pushNotifications, this.toaster, this.defaultToastTitle, "Error occurred while loading pictures.")
        },
        () => {
        });

    return observable;
  }

  getGalleryTypes(): Observable<Array<ISelectOption<number>>> {
    var apiUrl = PicturesControllerAPI.GetGalleryTypes();
    var cacheData = this.apiCache.getCachedItemIfNotExpired(apiUrl, 60);
    if (cacheData != null) return of(cacheData);

    let observable = this.httpClient.get<Array<ISelectOption<number>>>(this.baseUrl + apiUrl, { withCredentials: true })
      .pipe(share(), map((data, index) => {
        return data;
      }));

    observable
      .subscribe((data: Array<ISelectOption<number>>) => {
      },
        (error: any) => {
          console.log(error);
          createToastFromServiceResponse(error, this.pushNotifications, this.toaster, this.defaultToastTitle, "Error occurred while loading gallery types.")
        },
        () => {
        });

    return observable;
  }

  savePicture(dto: IPicture): Observable<IPicture> {
    if (!dto.isUpdated) {
      return of(dto);
    }

    var formData = getFormDataForDtoAndFileUpload(dto);

    let observable: Observable<IPicture>;
    if (dto.pictureId == 0) {
      observable = this.httpClient.post<IPicture>(this.baseUrl + PicturesControllerAPI.CreatePicture(), formData, { withCredentials: true })
        .pipe(share(), map((data: IPicture, index) => { data = IPicture_PrepareDto(data); return data; }));
    } else {
      observable = this.httpClient.put<IPicture>(this.baseUrl + PicturesControllerAPI.UpdatePicture(), formData, { withCredentials: true })
        .pipe(share(), map((data: IPicture, index) => { data = IPicture_PrepareDto(data); return data; }));
    }

    observable.subscribe((data: IPicture) => {
      // clear cache
      this.apiCache.clearCacheByPath(PicturesControllerAPI.GetPictures(0).replace("/0",""));
    },
      (error: any) => {
        console.log(error);
        createToastFromServiceResponse(error, this.pushNotifications, this.toaster, this.defaultToastTitle, "Error occurred while saving the picture.")
      },
      () => {
      });

    return observable;
  }
}
