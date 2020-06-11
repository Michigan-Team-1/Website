import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

import { ModalModule } from 'ngx-bootstrap/modal';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker'
import { CollapseModule } from 'ngx-bootstrap/collapse/';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';

import { LayoutComponent } from './layout.component';

import { NavLinksComponent } from './navLinks/navLinks.component';
import { TopNavMenuComponent } from './topNavMenu/topNavMenu.component';
import { SideNavMenuComponent } from './sideNavMenu/sideNavMenu.component';
import { FooterNavComponent } from './footerNav/footerNav.component';
import { SelectLanguageComponent } from './selectLanguage/selectLanguage.component';
import { PipesModule } from 'app.common/pipes';

@NgModule({
  imports: [FormsModule, CommonModule, RouterModule, CollapseModule, BsDropdownModule, PipesModule],
  declarations: [LayoutComponent, TopNavMenuComponent, SideNavMenuComponent, FooterNavComponent, SelectLanguageComponent, NavLinksComponent],
    exports: [LayoutComponent, TopNavMenuComponent, SideNavMenuComponent, FooterNavComponent, SelectLanguageComponent, NavLinksComponent],
})
export class LayoutsModule { }
