import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

import { ModalModule } from 'ngx-bootstrap/modal';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker'
import { CollapseModule } from 'ngx-bootstrap/collapse/';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { CgBusyModule } from 'angular-busy2';

import { SmartTableModule } from 'smart-table-ng';

import { PicturesComponent } from './pictures.component';
import { PictureAddEditComponent } from './pictureAddEdit.component';
import { PicturesService } from './pictures.service';

import { DirectivesModule } from 'app.common/directives';
import { PipesModule } from 'app.common/pipes';
import { ComponentsModule } from 'app.common/components';

@NgModule({
  imports: [FormsModule, CommonModule, PipesModule, DirectivesModule, CollapseModule, RouterModule,
    BsDropdownModule, BsDatepickerModule, ModalModule, CgBusyModule, ComponentsModule, SmartTableModule],
  declarations: [PicturesComponent, PictureAddEditComponent],
  exports: [PicturesComponent, PictureAddEditComponent],
  providers: [PicturesService],
  entryComponents: [PictureAddEditComponent]
})
export class PicturesModule { }
