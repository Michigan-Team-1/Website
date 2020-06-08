import { Component, OnInit, Input } from '@angular/core';
import { NgForm } from '@angular/forms';

import { BsModalRef } from 'ngx-bootstrap';

import { getErrorMessageFromServerResponse } from "app.common/helpers/general";
import { setIsUpdatedIfChanged, deepClone } from "app.common/helpers/object";
import { CommonService } from 'app.common/services/common.service';

import { AnnouncementsService } from './announcements.service';
import { IAnnouncement, IAnnouncement_PropertyAttributes } from 'app.common/dtos/AnnouncementDto';
import { dateMaskFormat } from 'app.common/constants';

@Component({
  selector: 'announcementAddEdit',
  templateUrl: './announcementAddEdit.component.html',
})
export class AnnouncementAddEditComponent implements OnInit {
  constructor(private announcementsService: AnnouncementsService,
    private activeModal: BsModalRef,
    private commonService: CommonService) { }

  isBusy: number = 0;
  @Input() dtoOriginal: IAnnouncement | undefined;
  defaultDto: IAnnouncement = { isActive: true, isUpdated: true };
  dto: IAnnouncement = deepClone(this.defaultDto);
  dtoPropertyAttributes = IAnnouncement_PropertyAttributes;
  message: string | undefined;
  success: boolean = false;
  dtoUpdated: boolean = false;
  dataUpdated: boolean = false;
  dateMaskFormat: string = dateMaskFormat;

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
    this.announcementsService.saveAnnouncement(this.dto).subscribe((data) => {
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
