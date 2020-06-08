import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

import { ModalModule } from 'ngx-bootstrap/modal';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker'
import { CollapseModule } from 'ngx-bootstrap/collapse/';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { CgBusyModule } from 'angular-busy2';
import { EditorModule } from '@tinymce/tinymce-angular';
import { SmartTableModule } from 'smart-table-ng';

import { AnnouncementsComponent } from './announcements.component';
import { AnnouncementAddEditComponent } from './announcementAddEdit.component';
import { AnnouncementsService } from './announcements.service';

import { DirectivesModule } from 'app.common/directives';
import { PipesModule } from 'app.common/pipes';
import { ComponentsModule } from 'app.common/components';

@NgModule({
  imports: [FormsModule, CommonModule, PipesModule, DirectivesModule, CollapseModule, EditorModule,
    BsDropdownModule, BsDatepickerModule, ModalModule, CgBusyModule, ComponentsModule, SmartTableModule],
  declarations: [AnnouncementsComponent, AnnouncementAddEditComponent],
  exports: [AnnouncementsComponent, AnnouncementAddEditComponent],
  providers: [AnnouncementsService],
  entryComponents: [AnnouncementAddEditComponent]
})
export class AnnouncementsModule { }
