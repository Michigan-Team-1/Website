import { Component, OnInit } from '@angular/core';

import { BsModalService, BsModalRef } from 'ngx-bootstrap';

import { EventAddEditComponent } from './eventAddEdit.component'
import { EventsService } from './events.service';

import { AuthService } from 'app.common/services/auth.service';
import { defaultPageSize } from 'app.common/constants';
import { SmartTable, from } from 'smart-table-ng';
import * as crud from 'smart-table-crud';
import { IEvent, IEvent_PropertyAttributes } from 'app.common/dtos/EventDto';

@Component({
  selector: 'events',
  templateUrl: './events.component.html',
  providers: [{
    provide: SmartTable,
    useFactory: (eventsService: EventsService) => from(eventsService.getEvents(), {
      search: {},
      sort: { pointer: "name", direction: "asc" },
      filter: {
        isActive: [{ operator: "equals", type: "string", value: "true" }]
      },
      slice: { page: 1, size: defaultPageSize }
    }, <any>crud),
    deps: [EventsService]
  }]
})
export class EventsComponent implements OnInit {
  constructor(private eventsService: EventsService, private modalService: BsModalService,
    private authService: AuthService, private table: SmartTable<IEvent>
  ) { }

  columnCount: number = 4;
  isCollapsedFilters: boolean = true;
  isBusy:boolean = false;
  dtoPropertyAttributes = IEvent_PropertyAttributes;

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

    let modal: BsModalRef = this.modalService.show(EventAddEditComponent, { initialState, class: 'modal-lg' });
  }
}
