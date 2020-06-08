import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

import { ModalModule } from 'ngx-bootstrap/modal';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker'
import { CollapseModule } from 'ngx-bootstrap/collapse/';
import { CarouselModule } from 'ngx-bootstrap/carousel';

import { DashboardComponent } from './dashboard.component';

import { DirectivesModule } from 'app.common/directives';
import { CgBusyModule } from 'angular-busy2';
import { PipesModule } from 'app.common/pipes';
import { ImageViewerComponent } from 'app.common/imageViewer/imageVIewer.component';

@NgModule({
  imports: [FormsModule, CommonModule, CollapseModule, DirectivesModule, BsDatepickerModule, CgBusyModule, PipesModule, CarouselModule],
    declarations: [DashboardComponent],
  exports: [DashboardComponent],
  entryComponents: [ImageViewerComponent]
})
export class DashboardModule { }
