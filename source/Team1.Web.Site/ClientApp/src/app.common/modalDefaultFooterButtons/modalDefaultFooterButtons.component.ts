import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'modalDefaultFooterButtons,[modalDefaultFooterButtons]',
  templateUrl: './modalDefaultFooterButtons.component.html',
})
export class ModalDefaultFooterButtons implements OnInit {
  constructor() {
  }

  @Input("form") f: NgForm | undefined;
  @Output() close: EventEmitter<string> = new EventEmitter();
  @Output() save: EventEmitter<any> = new EventEmitter();

  ngOnInit() {
  }

  onClose(args:any) {
    this.close.emit(args);
  }

  onSave() {
    this.save.emit();
  }
}
