import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'inputUnsavedChangesNotification,[inputUnsavedChangesNotification]',
  template: `<span *ngIf="dataUpdated" class=" ml-1 fas fa-circle text-danger"> </span>`,
})
export class InputUnsavedChangesNotification implements OnInit {
  constructor() {
  }

  @Input() dataUpdated: boolean = false;

  ngOnInit() {
  }
}
