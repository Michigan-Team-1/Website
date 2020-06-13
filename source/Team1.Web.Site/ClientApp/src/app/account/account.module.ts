import { NgModule, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

import { CgBusyModule } from 'angular-busy2';

import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { ForgotPasswordComponent } from './forgotPassword/forgotPassword.component';
import { ResetPasswordComponent } from './resetPassword/resetPassword.component';
import { TwoFactorComponent } from './twoFactor/twoFactor.component';
import { TwoFactorAppsComponent } from './twoFactorApps/twoFactorApps.component';
import { TwoFactorSetupComponent } from './twoFactorSetup/twoFactorSetup.component';
import { ConfirmEmailComponent } from './confirmEmail/confirmEmail.component';

import { ComponentsModule } from 'app.common/components';
import { DirectivesModule } from 'app.common/directives';
import { AuthService } from 'app.common/services/auth.service';
import { IsLoggedInAuthGuard, AnyPolicyAuthGuard } from 'app.common/services/authGuards.service';
import { PipesModule } from 'app.common/pipes';
import { BsDatepickerModule, PopoverModule } from 'ngx-bootstrap';

@NgModule({
  imports: [FormsModule, CgBusyModule, CommonModule, RouterModule, ComponentsModule, DirectivesModule, PipesModule, BsDatepickerModule, PopoverModule],
  declarations: [LoginComponent, RegisterComponent, ForgotPasswordComponent, ResetPasswordComponent, TwoFactorComponent, TwoFactorAppsComponent, TwoFactorSetupComponent, ConfirmEmailComponent],
  exports: [LoginComponent, RegisterComponent, ForgotPasswordComponent, ResetPasswordComponent, TwoFactorComponent, TwoFactorAppsComponent, TwoFactorSetupComponent, ConfirmEmailComponent],
  providers: [AuthService, IsLoggedInAuthGuard, AnyPolicyAuthGuard]
})
export class AccountModule { }
