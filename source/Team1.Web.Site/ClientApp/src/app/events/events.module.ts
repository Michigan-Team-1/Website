import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

import { ModalModule } from 'ngx-bootstrap/modal';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker'
import { CollapseModule } from 'ngx-bootstrap/collapse/';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { CgBusyModule } from 'angular-busy2';

import { SmartTableModule } from 'smart-table-ng';

import { EventsComponent } from './events.component';
import { EventAddEditComponent } from './eventAddEdit.component';
import { EventsService } from './events.service';

import { DirectivesModule } from 'app.common/directives';
import { PipesModule } from 'app.common/pipes';
import { ComponentsModule } from 'app.common/components';

@NgModule({
  imports: [FormsModule, CommonModule, PipesModule, DirectivesModule, CollapseModule,
    BsDropdownModule, BsDatepickerModule, ModalModule, CgBusyModule, ComponentsModule, SmartTableModule],
  declarations: [EventsComponent, EventAddEditComponent],
  exports: [EventsComponent, EventAddEditComponent],
  providers: [EventsService],
  entryComponents: [EventAddEditComponent]
})
export class EventsModule { }
