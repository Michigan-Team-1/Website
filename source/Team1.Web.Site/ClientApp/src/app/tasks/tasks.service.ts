import { Injectable, Inject } from '@angular/core';
import { Router } from "@angular/router";
import { Observable, of } from "rxjs";
import { share, map } from 'rxjs/Operators';
import { HttpClient } from '@angular/common/http';

import { NotificationsService } from 'angular2-notifications';
import { notifyUser, createToastFromServiceResponse } from "app.common/helpers/Toaster";

import { ApiCache } from 'app.common/cache/api.cache';

import { ITask, ITask_PrepareDto } from 'app.common/dtos/TaskDto';
import { TasksControllerAPI } from 'app.common/apis/TasksController';

@Injectable()
export class TasksService {
  constructor(private httpClient: HttpClient, @Inject('baseUrl') private baseUrl: string,
    private toaster: NotificationsService,
    private apiCache: ApiCache) {
  }
  pushNotifications: any;
  defaultToastTitle: string = `Tasks`;

  getTasks(): Observable<Array<ITask>> {
    var apiUrl = TasksControllerAPI.GetTasks();
    var cacheData = this.apiCache.getCachedItemIfNotExpired(apiUrl);
    if (cacheData != null) return of(cacheData);

    let observable = this.httpClient.get<Array<ITask>>(this.baseUrl + apiUrl, { withCredentials: true })
      .pipe(share(), map((data, index) => {
        for (var i = 0; i < data.length; i++) {
          data[i] = ITask_PrepareDto(data[i]);
        }
        return data;
      }));

    observable
      .subscribe((data: Array<ITask>) => {
        this.apiCache.setCachedItem(apiUrl, data);
      },
        (error: any) => {
          console.log(error);
          createToastFromServiceResponse(error, this.pushNotifications, this.toaster, this.defaultToastTitle, "Error occurred while loading tasks.")
        },
        () => {
        });

    return observable;
  }

  saveTask(dto: ITask): Observable<ITask> {
    if (!dto.isUpdated) {
      return of(dto);
    }

    let observable: Observable<ITask>;
    if (dto.taskId == 0) {
      observable = this.httpClient.post<ITask>(this.baseUrl + TasksControllerAPI.CreateTask(), dto, { withCredentials: true })
        .pipe(share(), map((data, index) => { return data; }));
    } else {
      observable = this.httpClient.put<ITask>(this.baseUrl + TasksControllerAPI.UpdateTask(), dto, { withCredentials: true })
        .pipe(share(), map((data, index) => { return data; }));
    }

    observable.subscribe((data: ITask) => {
      // clear cache
      this.apiCache.clearCacheByUrl(TasksControllerAPI.GetTasks());
    },
      (error: any) => {
        console.log(error);
        createToastFromServiceResponse(error, this.pushNotifications, this.toaster, this.defaultToastTitle, "Error occurred while saving the task.")
      },
      () => {
      });

    return observable;
  }
}
