import { Component, Input, OnInit, Output } from '@angular/core';
import { NgForm, ControlContainer } from '@angular/forms';
import { getRandomNameSuffix } from 'app.common/helpers/general';

@Component({
  selector: 'inputDate',
  template: `<div class="form-group form-row">
      <label for="{{predicate + nameSuffix}}" class="col-form-label col-sm-3"><defaultLabel [propertyAttributes]="propertyAttributes"></defaultLabel>
        <inputUnsavedChangesNotification [dataUpdated]="(dto[predicate] != null && dtoOriginal == null) || dto[predicate]?.getTime() != dtoOriginal[predicate]?.getTime()"></inputUnsavedChangesNotification>
      </label>
      <div class="col-sm-9">
        <div class="input-group">
          <input type="text" id="{{predicate + nameSuffix}}" name="{{predicate + nameSuffix}}" [(ngModel)]="dto[predicate]" #dateField="ngModel" class="form-control" inputMaskDate
               bsDatepicker="dateMaskFormat" #dp="bsDatepicker" [required]="propertyAttributes.required.value" />
          <div class="input-group-append">
            <button class="btn btn-outline-info" (click)="dp.toggle()" type="button">
              <i class="far fa-calendar-alt"></i>
            </button>
          </div>
        </div>
        <div *ngIf="f.submitted && dateField.invalid && (dateField.dirty || dateField.touched || f.submitted)" class="invalid-feedback d-block">
          <div *ngIf="dateField.errors.required">
            {{propertyAttributes | requiredErrorMessage}}
          </div>
        </div>
      </div>
    </div>`,
  viewProviders: [{ provide: ControlContainer, useExisting: NgForm }]
})
export class InputDateComponent implements OnInit {
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
