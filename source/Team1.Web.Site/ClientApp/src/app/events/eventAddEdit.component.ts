import { Component, OnInit, Input } from '@angular/core';
import { NgForm } from '@angular/forms';

import { BsModalRef } from 'ngx-bootstrap';

import { getErrorMessageFromServerResponse } from "app.common/helpers/general";
import { setIsUpdatedIfChanged, deepClone } from "app.common/helpers/object";
import { CommonService } from 'app.common/services/common.service';

import { EventsService } from './events.service';
import * as moment from 'moment';
import { IEvent, IEvent_PropertyAttributes } from 'app.common/dtos/EventDto';
import { dateMaskFormat } from 'app.common/constants';
import { LocationsService } from 'app/locations/locations.service';
import { ISelectOption } from 'app.common/dtos/SelectOptionDto';

@Component({
  selector: 'eventAddEdit',
  templateUrl: './eventAddEdit.component.html',
})
export class EventAddEditComponent implements OnInit {
  constructor(private eventsService: EventsService, private locationsService: LocationsService,
    private activeModal: BsModalRef,
    private commonService: CommonService) { }

  isBusy: number = 0;
  @Input() dtoOriginal: IEvent | undefined;
  defaultDto: IEvent = { isActive: true, isUpdated: true };
  dto: IEvent = deepClone(this.defaultDto);
  dtoPropertyAttributes = IEvent_PropertyAttributes;
  locations: Array<ISelectOption<number>> = [];
  message: string | undefined;
  success: boolean = false;
  dtoUpdated: boolean = false;
  dataUpdated: boolean = false;
  dateMaskFormat: string = dateMaskFormat;

  ngOnInit() {
    if (this.dtoOriginal == null) this.dtoOriginal = deepClone(this.defaultDto);
    this.dto = this.dtoOriginal != null ? deepClone(this.dtoOriginal) : this.dto;
    this.isBusy++;
    this.locationsService.getLocationsForSelection().subscribe((data) => {
      this.isBusy--;
      this.locations = data;
    }, (error) => {
      this.isBusy--;
      this.message = getErrorMessageFromServerResponse(error);
    }, () => {
    });
  }
  
  ngDoCheck() {
    this.dataIsUpdated();
  }

  dataIsUpdated(): boolean {
    this.dtoUpdated = setIsUpdatedIfChanged(this.dto, this.dtoOriginal);

    this.dataUpdated = this.dtoUpdated;
    return this.dataUpdated;
  }

  addLocation() {
    if (this.dto.eventLocations == null) this.dto.eventLocations = [];
    this.dto.eventLocations.push({locationId:undefined});
  }

  removeLocation(index: number) {
    if (this.dto.eventLocations == null) return;

    var location = this.dto.eventLocations[index];
    if (!location.locationId) {
      this.dto.eventLocations.splice(index, 1);
      return;
    }

    location.isDeleted = true;
  }

  save(form: NgForm) {
    if (!form.valid) {
      return;
    }

    setIsUpdatedIfChanged(this.dto, this.dtoOriginal);

    this.isBusy++;
    this.eventsService.saveEvent(this.dto).subscribe((data) => {
      this.isBusy--;
      this.dto = data;
      this.success = true;
      this.activeModal.hide();
    }, (error) => {
      this.isBusy--;
      this.message = getErrorMessageFromServerResponse(error);
    }, () => {
    });
  }

  close(closeMethod: string) {
    this.activeModal.hide();
  }
}
