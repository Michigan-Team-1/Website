import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

import { ModalModule } from 'ngx-bootstrap/modal';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker'
import { CollapseModule } from 'ngx-bootstrap/collapse/';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';

import { JoinUsComponent } from './joinUs.component';

import { DirectivesModule } from 'app.common/directives';
import { CgBusyModule } from 'angular-busy2';
import { PipesModule } from 'app.common/pipes';

@NgModule({
  imports: [FormsModule, RouterModule, CommonModule, CollapseModule, DirectivesModule, BsDatepickerModule, CgBusyModule, PipesModule],
    declarations: [JoinUsComponent],
    exports: [JoinUsComponent],
})
export class JoinUsModule { }
