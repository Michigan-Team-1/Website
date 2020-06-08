import { Component } from '@angular/core';

// comment out/remove side-nav-menu if not needed, also comment out associated body-content CSS within site.scss 
@Component({
    selector: 'layout',
  template: `<topNavMenu></topNavMenu>
            <!--<sideNavMenu></sideNavMenu>-->
            <div class="container-fluid body-content">
                <router-outlet></router-outlet>
            </div>
            <footerNav></footerNav>`,
    styles: []
})
export class LayoutComponent { }
