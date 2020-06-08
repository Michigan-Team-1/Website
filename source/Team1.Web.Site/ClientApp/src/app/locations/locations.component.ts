import { Component, OnInit } from '@angular/core';

import { BsModalService, BsModalRef } from 'ngx-bootstrap';

import { LocationAddEditComponent } from './locationAddEdit.component'
import { LocationsService } from './locations.service';

import { AuthService } from 'app.common/services/auth.service';
import { defaultPageSize } from 'app.common/constants';
import { SmartTable, from } from 'smart-table-ng';
import * as crud from 'smart-table-crud';
import { ILocation_PropertyAttributes, ILocation } from 'app.common/dtos/LocationDto';

@Component({
  selector: 'locations',
  templateUrl: './locations.component.html',
  providers: [{
    provide: SmartTable,
    useFactory: (locationsService: LocationsService) => from(locationsService.getLocations(), {
      search: {},
      sort: { pointer: "locationName", direction: "asc" },
      filter: {
        isActive: [{ operator: "equals", type: "string", value: "true" }]
      },
      slice: { page: 1, size: defaultPageSize }
    }, <any>crud),
    deps: [LocationsService]
  }]
})
export class LocationsComponent implements OnInit {
  constructor(private modalService: BsModalService,
    private authService: AuthService, private table: SmartTable<ILocation>
  ) { }

  columnCount: number = 3;
  isCollapsedFilters: boolean = true;
  isBusy:boolean = false;
  dtoPropertyAttributes = ILocation_PropertyAttributes;

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

    let modal: BsModalRef = this.modalService.show(LocationAddEditComponent, { initialState, class: 'modal-lg' });
  }
}
