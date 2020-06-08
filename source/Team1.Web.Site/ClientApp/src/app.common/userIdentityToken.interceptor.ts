import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';
import { AuthService } from './services/auth.service';
import { Observable } from 'rxjs';
import { localStorageUserIdentityToken, localStorageUserIdentityTokenValidTo } from "app.common/constants";
import * as moment from 'moment';

@Injectable()
export class UserIdentityJWTInterceptor implements HttpInterceptor {
    authService: AuthService | undefined;

    constructor(private injector: Injector) {
    }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        let requestToForward = req;
        if (req.withCredentials) {
            if (this.authService == null)
                this.authService = this.injector.get(AuthService);
            
            if (this.authService && this.authService.token !== "") {
                let tokenValue = "Bearer " + this.authService.token;
                requestToForward = req.clone({ setHeaders: { "Authorization": tokenValue } });
            } else {
                console.debug("token not found");
            }
        }

        return next.handle(requestToForward);
    }
}
