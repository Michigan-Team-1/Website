import { Component } from '@angular/core';
import { SimpleNotificationsComponent } from 'angular2-notifications';
import { AuthService } from 'app.common/services/auth.service';

@Component({
    selector: 'app',
    template: `<simple-notifications [options]="notificationOptions"></simple-notifications>
                <router-outlet></router-outlet>`,
})
export class AppComponent {
    constructor(private authService: AuthService) { }

    public notificationOptions = {   
        position: ["top", "right"], 
        timeOut: 5000,
        lastOnBottom: true
    }
}
