import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'unsavedChangesNotification,[unsavedChangesNotification]',
  templateUrl: './unsavedChangesNotification.component.html',
})
export class UnsavedChangesNotification implements OnInit {
  constructor() {
  }

  @Input() dataUpdated: boolean = false;

  ngOnInit() {
  }
}
