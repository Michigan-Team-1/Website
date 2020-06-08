import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { IsLoggedInAuthGuard, AnyPolicyAuthGuard } from 'app.common/services/authGuards.service';
import { LoginComponent } from './account/login/login.component';
import { RegisterComponent } from './account/register/register.component';
import { ForgotPasswordComponent } from './account/forgotPassword/forgotPassword.component';
import { ResetPasswordComponent } from './account/resetPassword/resetPassword.component';
import { TwoFactorComponent } from './account/twoFactor/twoFactor.component';
import { TwoFactorSetupComponent } from './account/twoFactorSetup/twoFactorSetup.component';
import { ConfirmEmailComponent } from './account/confirmEmail/confirmEmail.component';
import { LayoutComponent } from './layouts/layout.component';

import { DashboardComponent } from './dashboard/dashboard.component';
import { UsersComponent } from './users/users.component';
import { userProfileEdit, taskAddEditDelete, eventAddEditDelete, locationAddEditDelete, announcementAddEditDelete } from 'app.common/serverConstants/PolicyNames';
import { ManageComponent } from './manage/manage.component';
import { TasksComponent } from './tasks/tasks.component';
import { EventsComponent } from './events/events.component';
import { LocationsComponent } from './locations/locations.component';
import { AnnouncementsComponent } from './announcements/announcements.component';
import { JoinUsComponent } from './joinUs/joinUs.component';
import { PicturesComponent } from './pictures/pictures.component';

// ****************************** all paths must be lower case ******************************
// ****************************** all route Urls for links must be lower case ***************
const routes: Routes = [
    {
        path: '',
        component: LayoutComponent,
        canActivate: [],
        children: [
          { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
          { path: 'login', component: LoginComponent },
          { path: 'register', component: RegisterComponent },
          { path: 'forgotpassword', component: ForgotPasswordComponent },
          { path: 'resetpassword', component: ResetPasswordComponent },
          { path: 'twofactor', component: TwoFactorComponent },
          { path: 'twofactorsetup', component: TwoFactorSetupComponent },
          { path: 'confirmemail', component: ConfirmEmailComponent },
          { path: 'dashboard', component: DashboardComponent },
          { path: 'joinus', component: JoinUsComponent },
          { path: 'announcements', component: AnnouncementsComponent, canActivate: [IsLoggedInAuthGuard, AnyPolicyAuthGuard], canLoad: [IsLoggedInAuthGuard,AnyPolicyAuthGuard], data: { policy: announcementAddEditDelete } },
          { path: 'events', component: EventsComponent, canActivate: [IsLoggedInAuthGuard, AnyPolicyAuthGuard], canLoad: [IsLoggedInAuthGuard, AnyPolicyAuthGuard], data: { policy: eventAddEditDelete } },
          { path: 'gallery', component: PicturesComponent, data: { isMyGallery: false }},
          { path: 'mygallery', component: PicturesComponent, canActivate: [IsLoggedInAuthGuard], canLoad: [IsLoggedInAuthGuard], data: { isMyGallery: true } },
          { path: 'locations', component: LocationsComponent, canActivate: [IsLoggedInAuthGuard, AnyPolicyAuthGuard], canLoad: [IsLoggedInAuthGuard,AnyPolicyAuthGuard], data: { policy: locationAddEditDelete } },
          { path: 'profile', component: ManageComponent, canActivate: [IsLoggedInAuthGuard, AnyPolicyAuthGuard], canLoad: [IsLoggedInAuthGuard,AnyPolicyAuthGuard], data: { policy: userProfileEdit } },
          { path: 'tasks', component: TasksComponent, canActivate: [IsLoggedInAuthGuard, AnyPolicyAuthGuard], canLoad: [IsLoggedInAuthGuard,AnyPolicyAuthGuard], data: { policy: taskAddEditDelete } },
          { path: 'users', component: UsersComponent },
        ],
        runGuardsAndResolvers: "always"
    },
    { path: '**', redirectTo: '' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {
    onSameUrlNavigation: 'reload',
    
  })],
    exports: [RouterModule]
})
export class AppRoutingModule { }
