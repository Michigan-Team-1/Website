import { NotificationsService, NotificationType } from 'angular2-notifications';

import { getErrorMessageFromServerResponse } from './general';

export function notifyUser(pushNotifications: any, toaster: NotificationsService, type: NotificationType | undefined, title: string, content: string, usePushNotification?: boolean | undefined) {
  // comment out the below if statement if push notifications are not wanted
  //if (pushNotifications.isSupported() && pushNotifications.permission === "default")
  //  pushNotifications.requestPermission();
   
  var typesToUsePushNotifications = ["error"];
  //usePushNotification = pushNotifications.permission === "granted" && (typesToUsePushNotifications.some((value) => { return value === type; }) || usePushNotification);

  //if (usePushNotification) {
  //  pushNotifications.create(title, { body: content.replace("<br/>", "\n"), sticky: type === "error" }).subscribe((res: any) => { }, (err: any) => {
  //    toaster.create(title, content, type);
  //    console.log(err)
  //  });
  //}
  //else {
    toaster.create(title, content, type);
  //}
}

// service call error general helper
export function createToastFromServiceResponse(response: any, pushNotifications: any, toaster: NotificationsService, title: string, defaultMessage: string) {
  var msg = getErrorMessageFromServerResponse(response);

  switch (response.status) {
    case 400: // badrequest
      notifyUser(pushNotifications, toaster, NotificationType.Error, title, msg);
      break;
    case 409: // conflict
    case 501: // not implemented
    case 401: // unauthorized
      notifyUser(pushNotifications, toaster, NotificationType.Error, title, msg);
      break;
    case 500: // server error
      notifyUser(pushNotifications, toaster, NotificationType.Error, title, defaultMessage);
      break;
  }
};
