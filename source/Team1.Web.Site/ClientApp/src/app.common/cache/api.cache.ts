import * as moment from 'moment';

import { defaultCacheExpirationMinutes } from 'app.common/constants';

import { ICache, ICacheItem } from './cache.interface';
import { Injectable } from "@angular/core";

@Injectable()
export class ApiCache {
  constructor() {
  }

  cache: ICache = {};
  
  /**
   * gets the cached data if it has not expired
   * if minutesToExpire not supplied then uses default Expiration minutes in the constants file
   * @param key url key.  typically does NOT include the baseApiUrl
   * @param minutesToExpire minutes the cache will last.  if left out, uses the app default
   */
  getCachedItemIfNotExpired(key: string, minutesToExpire?: number): any {
    if (this.cache[key] == null) {
      return null;
    }

    var minutes = minutesToExpire || defaultCacheExpirationMinutes;
    var cachedItem = <ICacheItem>this.cache[key];

    if (cachedItem.createdDateTime.add(minutes, 'minutes').isBefore(moment())) {
      cachedItem.data = null;
    }

    return cachedItem.data;
  }

  /**
   * set the cached item
   * @param key url key.  typically does NOT include the baseApiUrl
   * @param data data to cache
   */
  setCachedItem(key: string, data: any) {
    this.cache[key] = { data: data, createdDateTime: moment() };
  }

  /**
   * clears the cache for the specified url
   * @param key url key.  typically does NOT include the baseApiUrl
   */
  clearCacheByUrl(key: string) {
    this.cache[key] = null;
  }

  /**
   * clears the cache for the specified starting url path
   * this can result in multiple caches being cleared
   * pass in "api/user" and the following would be cleared "api/users", "api/user/4", "api/user/1", etc.
   * the following would not be cleared "api/company", "api/5/user"
   * @param path partial url
   */
  clearCacheByPath(path: string) {
    for (let key in this.cache) {
      if (key.startsWith(path)) {
        this.cache[key] = null;
      }
    }
  }

  /** clears all cache */
  clearCache() {
    this.cache = {};
  }
}
