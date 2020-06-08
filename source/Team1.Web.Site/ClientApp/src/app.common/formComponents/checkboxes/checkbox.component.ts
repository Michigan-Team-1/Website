import { Component, Input, OnInit, Output } from '@angular/core';
import { NgForm, ControlContainer } from '@angular/forms';
import { getRandomNameSuffix } from 'app.common/helpers/general';

@Component({
  selector: 'spudCheckbox',
  template: `<div class="form-group form-row">
      <div class="offset-sm-3 col-sm-9">
        <div class="form-check">
          <label class="form-check-label">
            <input type="checkbox" id="{{predicate + nameSuffix}}" name="{{predicate + nameSuffix}}" [(ngModel)]="dto[predicate]" class="form-check-input" />
            {{propertyAttributes.name}}
              <inputUnsavedChangesNotification [dataUpdated]="(dto[predicate] != null && dtoOriginal == null) || dto[predicate] != dtoOriginal[predicate]"></inputUnsavedChangesNotification>
          </label>
        </div>
      </div>
    </div>`,
  viewProviders: [{ provide: ControlContainer, useExisting: NgForm }]
})
export class SpudCheckboxComponent implements OnInit {
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
