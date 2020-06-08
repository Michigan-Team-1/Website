import { Component, Input, OnInit, Output } from '@angular/core';
import { NgForm, ControlContainer } from '@angular/forms';
import { getRandomNameSuffix } from 'app.common/helpers/general';
import { EventEmitter } from 'protractor';

@Component({
  selector: 'inputNaturalInteger',
  template: `<div class="form-group form-row">
      <label for="{{predicate + nameSuffix}}" class="col-form-label col-sm-3"><defaultLabel [propertyAttributes]="propertyAttributes"></defaultLabel>
        <inputUnsavedChangesNotification [dataUpdated]="(dto[predicate] != null && dtoOriginal == null) || dto[predicate] != dtoOriginal[predicate]"></inputUnsavedChangesNotification>
      </label>
      <div class="col-sm-9">
        <input type="text" id="{{predicate + nameSuffix}}" name="{{predicate + nameSuffix}}" [(ngModel)]="dto[predicate]" #inputInteger="ngModel" class="form-control"
               [required]="propertyAttributes.required.value" inputNaturalInteger />
        <div *ngIf="f.submitted && inputInteger.invalid && (inputInteger.dirty || inputInteger.touched || f.submitted)" class="invalid-feedback d-block">
          <div *ngIf="inputInteger.errors.required">
            {{propertyAttributes | requiredErrorMessage}}
          </div>
        </div>
      </div>
    </div>`,
  viewProviders: [{ provide: ControlContainer, useExisting: NgForm }]
})
export class InputNaturalIntegerComponent implements OnInit {
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
