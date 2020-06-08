import * as moment from 'moment';

export interface ICache {
  /** key: if api url generally going to start with "api/...".  No need to store the baseApiUrl with it. */
  [key: string]: ICacheItem | null;
}

export interface ICacheItem {
  data: any;
  createdDateTime: moment.Moment;
}
