import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {
    CanActivate, Router, CanLoad, Route,
    ActivatedRouteSnapshot,
    RouterStateSnapshot
} from '@angular/router';

import { AuthService } from './auth.service';

@Injectable()
export class IsLoggedInAuthGuard implements CanActivate, CanLoad {
    constructor(private authService: AuthService, private router: Router) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
        let url: string = state.url;
        return this.checkLogin(url);
    }

    canLoad(route: Route): boolean {
        let url = `/${route.path}`;
        return this.checkLogin(url);
    }

    checkLogin(url: string): boolean {
        if (this.authService.isAuthenticated) {
            return true;
        }
        
        // Store the attempted URL for redirecting
        this.authService.redirectUrl = url;

        // Navigate to the login page with extras
        this.router.navigate(['/login']);
        return false;
    }
}

/**
 * Only need one role in order to succeed and be allowed access
 */
@Injectable()
export class AnyPolicyAuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    let policyRequired = route.data["policy"] as string;

    let url: string = state.url;
    return this.checkAuth(url, policyRequired);
  }

  checkAuth(url: string, policyRequired: string): boolean {
    if (this.authService.checkPolicyAccess(policyRequired)) {
      return true;
    }
    // Navigate to the login page with extras
    this.router.navigate(['/accessdenied']);
    return false;
  }
}
