import { Component, OnInit, Input } from '@angular/core';
import { NgForm } from '@angular/forms';

import { BsModalRef } from 'ngx-bootstrap';

import { getErrorMessageFromServerResponse } from "app.common/helpers/general";
import { setIsUpdatedIfChanged, deepClone } from "app.common/helpers/object";
import { CommonService } from 'app.common/services/common.service';

import { LocationsService } from './locations.service';
import { ILocation_PropertyAttributes,ILocation } from 'app.common/dtos/LocationDto';

@Component({
  selector: 'locationAddEdit',
  templateUrl: './locationAddEdit.component.html',
})
export class LocationAddEditComponent implements OnInit {
  constructor(private locationsService: LocationsService,
    private activeModal: BsModalRef,
    private commonService: CommonService) { }

  isBusy: number = 0;
  @Input() dtoOriginal: ILocation | undefined;
  defaultDto: ILocation = { isActive: true, isUpdated: true };
  dto: ILocation = deepClone(this.defaultDto);
  dtoPropertyAttributes = ILocation_PropertyAttributes;
  message: string | undefined;
  success: boolean = false;
  dtoUpdated: boolean = false;
  dataUpdated: boolean = false;
  addressIsRequired: boolean = true;

  ngOnInit() {
    if (this.dtoOriginal == null) this.dtoOriginal = deepClone(this.defaultDto);
    this.dto = this.dtoOriginal != null ? deepClone(this.dtoOriginal) : this.dto;
  }
  
  ngDoCheck() {
    this.dataIsUpdated();
  }

  dataIsUpdated(): boolean {
    this.dtoUpdated = setIsUpdatedIfChanged(this.dto, this.dtoOriginal);

    this.dataUpdated = this.dtoUpdated;
    return this.dataUpdated;
  }

  save(form: NgForm) {
    if (!form.valid) {
      return;
    }

    setIsUpdatedIfChanged(this.dto, this.dtoOriginal);

    this.isBusy++;
    this.locationsService.saveLocation(this.dto).subscribe((data) => {
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
