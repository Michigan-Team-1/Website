import { NgModule, Inject } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { UrlSerializer } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { PopoverModule } from 'ngx-bootstrap/popover';
import { ModalModule } from 'ngx-bootstrap/modal';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker'
import { CollapseModule } from 'ngx-bootstrap/collapse/';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { SimpleNotificationsModule } from 'angular2-notifications';
import { CgBusyModule, CgBusyDefaults } from 'angular-busy2';
//import { SmartTableModule } from 'smart-table-ng';

// inputmask node_modules\inputmask\index.js phone js files (not ".extensions") uncommented so global phone inputmask works.
// when inputmask adds typings can remove typings.d.ts entry for it.
//import Inputmask from 'inputmask';

import { LowerCaseUrlSerializer } from 'app.common/helpers/LowerCaseUrlSerializer';
 
import { AppComponent } from './app.component';
import { AppRoutingModule } from './appRouting.module';

import { LayoutsModule } from './layouts/layouts.module';
import { AccountModule } from './account/account.module';
import { DashboardModule } from './dashboard/dashboard.module';
import { UsersModule } from './users/users.module';

// app.common stuff
import { UserIdentityJWTInterceptor } from 'app.common/userIdentityToken.interceptor';
import { AuthService } from 'app.common/services/auth.service';
import { IsLoggedInAuthGuard, AnyPolicyAuthGuard } from 'app.common/services/authGuards.service';
import { ComponentsModule } from 'app.common/components'; 
import { DirectivesModule } from 'app.common/directives';
import { PipesModule } from 'app.common/pipes';
import { ApiCache } from 'app.common/cache/api.cache';

import { datePickerFormat } from 'app.common/constants'
import { ManageModule } from './manage/manage.module';
import { TasksModule } from './tasks/tasks.module';
import { EventsModule } from './events/events.module';
import { LocationsModule } from './locations/locations.module';
import { AnnouncementsModule } from './announcements/announcements.module';
import { JoinUsModule } from './joinUs/joinUs.module';
import { PicturesModule } from './pictures/pictures.module';


@NgModule({
  declarations: [
    AppComponent,
  ],
  providers: [AuthService, IsLoggedInAuthGuard, AnyPolicyAuthGuard, ApiCache, 
    {
      provide: HTTP_INTERCEPTORS,
      useClass: UserIdentityJWTInterceptor,
      multi: true
    },
    {
      provide: UrlSerializer,
      useClass: LowerCaseUrlSerializer
    }
  ],
  imports: [
    CommonModule,
    HttpClientModule,
    FormsModule,
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    BrowserAnimationsModule,

    // ngx-bootstrap
    BsDatepickerModule.forRoot(),
    BsDropdownModule.forRoot(),
    CollapseModule.forRoot(),
    ModalModule.forRoot(),
    PopoverModule.forRoot(),

    //SmartTableModule.forRoot(),

   // PushNotificationsModule,
    SimpleNotificationsModule.forRoot(),
    CgBusyModule.forRoot(),
    //Inputmask, // re-add once Inputmask can be imported properly

    AppRoutingModule,

    ComponentsModule,
    DirectivesModule,
    PipesModule,

    LayoutsModule,
    AccountModule,
    AnnouncementsModule,
    DashboardModule,
    JoinUsModule,
    ManageModule,
    EventsModule,
    PicturesModule,
    LocationsModule,
    TasksModule,
    UsersModule
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
  constructor(private busyDefaults: CgBusyDefaults, private datePickerConfig: BsDatepickerConfig) {
    this.busyDefaults.delay = 150;
    this.busyDefaults.minDuration = 500;
    this.busyDefaults.message = "Working...";

    this.datePickerConfig.containerClass = "theme-default";
    this.datePickerConfig.dateInputFormat = datePickerFormat;
    this.datePickerConfig.showWeekNumbers = false;
  }
}
