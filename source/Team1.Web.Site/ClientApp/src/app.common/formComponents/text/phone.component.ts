import { Component, Input, OnInit, Output } from '@angular/core';
import { NgForm, ControlContainer } from '@angular/forms';
import { getRandomNameSuffix } from 'app.common/helpers/general';
import { EventEmitter } from 'protractor';

@Component({
  selector: 'spudPhone',
  template: `<div class="form-group form-row">
      <label for="{{predicate + nameSuffix}}" class="col-form-label col-sm-3"><defaultLabel [propertyAttributes]="propertyAttributes"></defaultLabel>
        <inputUnsavedChangesNotification [dataUpdated]="(dto[predicate] != null && dtoOriginal == null) || dto[predicate] != dtoOriginal[predicate]"></inputUnsavedChangesNotification>
      </label>
      <div class="col-sm-9">
        <input type="text" id="{{predicate + nameSuffix}}" name="{{predicate + nameSuffix}}" [(ngModel)]="dto[predicate]" #phone="ngModel" class="form-control"
               [required]="propertyAttributes.required.value" inputMaskPhone />
        <div *ngIf="f.submitted && phone.invalid && (phone.dirty || phone.touched || f.submitted)" class="invalid-feedback d-block">
          <div *ngIf="phone.errors.required">
            {{propertyAttributes | requiredErrorMessage}}
          </div>
        </div>
      </div>
    </div>`,
  viewProviders: [{ provide: ControlContainer, useExisting: NgForm }]
})
export class SpudPhoneComponent implements OnInit {
  constructor(
  ) {
  }

  @Input("form") f: NgForm = new NgForm([], []);
  @Input() dto: any;
  @Input() dtoOriginal: any;
  @Input() predicate: string = '';
  @Input() propertyAttributes: any;
  @Input() nameSuffix: string = getRandomNameSuffix();

  ngOnInit() {
  }
}
