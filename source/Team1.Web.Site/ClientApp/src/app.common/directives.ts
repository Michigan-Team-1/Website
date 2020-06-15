import { NgModule, Directive, ElementRef, Input, Output, HostListener, HostBinding, EventEmitter, AfterViewInit } from '@angular/core';

import { dateMaskFormat, globalPhoneMask, usPhoneMask } from 'app.common/constants';
// inputmask node_modules\inputmask\index.js phone js files (not ".extensions") uncommented so global phone inputmask works.
// when inputmask adds typings can import
//import Inputmask from 'inputmask';
var Inputmask = require('inputmask');

@Directive({
    selector: '[inputMaskPhone]'
})
export class InputMaskPhone {
  constructor(private el: ElementRef) {
    var inputmask = Inputmask(this.mask, this.options);
    inputmask.mask(this.el.nativeElement);
  }

  mask: string = usPhoneMask;
  options: any = { showTooltip: true, autoUnmask: true };
}

// for global phone inputmask to work, requires extra phone extensions.
// within the index.js file inside  node_modules\inputmask uncomment phone.js for this to work.
@Directive({
  selector: '[inputMaskPhoneGlobal]',
})
export class InputMaskPhoneGlobal {
  constructor(private el: ElementRef) {
    var self = this;
    this.options = {
      showTooltip: true, autoUnmask: true, onUnMask: this.onUnMask,
      onBeforeWrite: this.onBeforeWrite
    };

    this.inputmask = Inputmask(this.mask, this.options);
    this.inputmask.mask(this.el.nativeElement);
  }

  @Output("inputMaskPhoneGlobal") phoneValueChanged: EventEmitter<any> = new EventEmitter<any>();

  onUnMask = (maskedValue: string, unmaskedValue: string) => {
    var maskRemoved = maskedValue.replace(/\D/g, '');
    return maskRemoved;
  };

  onBeforeWrite = (event: any, buffer: any, caretPos: any, opts: any) => {
    var metaData = this.el.nativeElement.inputmask.getmetadata();
    this.emitValues(metaData);
  }

  emitValues(inputmaskMetaData:any) {
    let countryCode: string | undefined = undefined;
    if (inputmaskMetaData.mask) {
      countryCode = inputmaskMetaData.mask.replace(/\D/g, '');
    }
    if (!countryCode) {
      countryCode = inputmaskMetaData.replace(/\D/g, '').substr(0, 1);
    }
    this.phoneValueChanged.emit({ countryCode: countryCode });
  }

  inputmask: any;
  mask: string = globalPhoneMask;
  options: any;
}

@Directive({
  selector: '[inputMask]'
})
export class InputMaskGeneral {
  constructor(private el: ElementRef) {
  }

  @Input("inputMask") mask: string | undefined;
  options: any = { showTooltip: true, autoUnmask: true };

  ngOnChanges(): void {
    var inputmask = Inputmask(this.mask, this.options);
    inputmask.mask(this.el.nativeElement);
  }
}

@Directive({
  selector: '[inputMaskDate]'
})
export class InputMaskDate {
  constructor(private el: ElementRef<HTMLInputElement>) {
    var inputmask = Inputmask(this.mask, this.options);
    inputmask.mask(this.el.nativeElement);
  }

  mask: string = 'datetime'
  options: any = { showTooltip: true, autoUnmask: true, inputFormat: dateMaskFormat };

  @Output() ngModelChange: EventEmitter<any> = new EventEmitter(false);

  @HostListener("keyup") onkeyup() {

  }

  @HostListener("focus") onfocus() {

  }

  @HostListener("blur") onblur() {
    if (this.el.nativeElement.value.endsWith("0000")) {
      this.el.nativeElement.value = "";
      this.ngModelChange.emit(undefined);
    }
  }
}

@Directive({
  selector: '[inputDecimal]'
})
export class InputDecimal {
  constructor(private el: ElementRef) {
    var inputmask = Inputmask(this.mask, this.options);
    inputmask.mask(this.el.nativeElement);
  }

  mask: string = 'decimal';
  options: any = { showTooltip: true, autoUnmask: true, groupSeparator: ',', autoGroup: true };
}

@Directive({
  selector: '[inputInteger]'
})
export class InputInteger {
  constructor(private el: ElementRef) {
    var inputmask = Inputmask(this.mask, this.options);
    inputmask.mask(this.el.nativeElement);
  }

  mask: string = 'integer';
  options: any = { showTooltip: true, autoUnmask: true, groupSeparator: ',', autoGroup: true };
}

// positive integers only + 0
@Directive({
  selector: '[inputNaturalInteger]'
})
export class InputNaturalInteger {
  constructor(private el: ElementRef) {
    this.options = { showTooltip: true, autoUnmask: true, groupSeparator: ',', autoGroup: true, allowMinus: false, onUnMask: this.onUnMask, rightAlign: false };
    var inputmask = Inputmask(this.mask, this.options);
    inputmask.mask(this.el.nativeElement);
  }

  mask: string = 'integer';
  options: any;

  onUnMask = (maskedValue: string, unmaskedValue: string) => {
    if (unmaskedValue != null)
      return parseInt(unmaskedValue);
    return 0;
  };
}

@Directive({
  selector: '[autofocus]'
})
export class AutofocusDirective implements AfterViewInit {
  private _autofocus: boolean = true;
  constructor(private el: ElementRef) {
  }

  ngAfterViewInit() {
    if (this._autofocus)
      this.el.nativeElement.focus();
  }

  @Input() set autofocus(condition: boolean) {
    this._autofocus = condition != false;
  }
}

// must come last
@NgModule({
  //imports: [], //Inputmask
  declarations: [InputMaskPhone, InputMaskPhoneGlobal, InputMaskGeneral, InputMaskDate, InputDecimal, AutofocusDirective, InputInteger, InputNaturalInteger],
  exports: [InputMaskPhone, InputMaskPhoneGlobal, InputMaskGeneral, InputMaskDate, InputDecimal, AutofocusDirective, InputInteger, InputNaturalInteger]
})
export class DirectivesModule { }
