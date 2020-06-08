import { Component, Input, OnInit, Output } from '@angular/core';
import { NgForm, ControlContainer } from '@angular/forms';
import { getRandomNameSuffix } from 'app.common/helpers/general';

@Component({
  selector: 'textAreaMax',
  template: `<div class="form-group form-row">
      <label for="{{predicate + nameSuffix}}" class="col-form-label col-sm-3"><defaultLabel [propertyAttributes]="propertyAttributes"></defaultLabel>
        <inputUnsavedChangesNotification [dataUpdated]="(dto[predicate] != null && dtoOriginal == null) || dto[predicate] != dtoOriginal[predicate]"></inputUnsavedChangesNotification>
      </label>
      <div class="col-sm-9">
        <textarea type="text" id="{{predicate + nameSuffix}}" name="{{predicate + nameSuffix}}" [(ngModel)]="dto[predicate]" #textareaMax="ngModel" class="form-control" rows="3"
               [attr.maxlength]="propertyAttributes.stringLength.maxLength" [required]="propertyAttributes.required.value" ></textarea>
        <div *ngIf="f.submitted && textareaMax.invalid && (textareaMax.dirty || textareaMax.touched || f.submitted)" class="invalid-feedback d-block">
          <div *ngIf="textareaMax.errors.required">
            {{propertyAttributes | requiredErrorMessage}}
          </div>
        </div>
      </div>
    </div>`,
  viewProviders: [{ provide: ControlContainer, useExisting: NgForm }]
})
export class TextAreaMaxComponent implements OnInit {
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
