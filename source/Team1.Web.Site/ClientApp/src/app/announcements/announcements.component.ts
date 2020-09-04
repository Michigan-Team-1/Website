import { Component, OnInit } from '@angular/core';

import { BsModalService, BsModalRef } from 'ngx-bootstrap';

import { AnnouncementAddEditComponent } from './announcementAddEdit.component'
import { AnnouncementsService } from './announcements.service';

import { AuthService } from 'app.common/services/auth.service';
import { defaultPageSize } from 'app.common/constants';
import { SmartTable, from } from 'smart-table-ng';
import * as crud from 'smart-table-crud';
import { IAnnouncement, IAnnouncement_PropertyAttributes } from 'app.common/dtos/AnnouncementDto';

@Component({
  selector: 'announcements',
  templateUrl: './announcements.component.html',
  providers: [{
    provide: SmartTable,
    useFactory: (announcementsService: AnnouncementsService) => from(announcementsService.getAnnouncements(), {
      search: {},
      sort: { pointer: "title", direction: "asc" },
      filter: {
        isActive: [{ operator: "equals", type: "string", value: "true" }]
      },
      slice: { page: 1, size: defaultPageSize }
    }, <any>crud),
    deps: [AnnouncementsService]
  }]
})
export class AnnouncementsComponent implements OnInit {
  constructor(private modalService: BsModalService,
    private authService: AuthService, private table: SmartTable<IAnnouncement>
  ) { }

  columnCount: number = 3;
  isCollapsedFilters: boolean = true;
  isBusy:boolean = false;
  dtoPropertyAttributes = IAnnouncement_PropertyAttributes;

  ngOnInit() {
  }

  edit(index: number | undefined) {
    const initialState = {
      dtoOriginal: index != null ? (<any>this.table).get(index) : undefined
    };

    let subcription = this.modalService.onHide.subscribe((reason: string) => {
      if (modal.content.success) {
        let dto = modal.content.dto;
        if (index == null) {
          (<any>this.table).insert(dto);
        }
        else {
          (<any>this.table).update(index, dto);
        }
      }

      subcription.unsubscribe();
    });

    let modal: BsModalRef = this.modalService.show(AnnouncementAddEditComponent, { initialState, class: 'modal-full-screen' });
  }
}
