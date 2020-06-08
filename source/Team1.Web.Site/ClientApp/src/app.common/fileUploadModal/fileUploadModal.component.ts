import { Component, OnInit, Input, ViewChild, ElementRef } from '@angular/core';
import { NgForm, FormControl } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap';

import { CommonService } from 'app.common/services/common.service';

@Component({
  selector: 'fileUploadModal',
  templateUrl: './fileUploadModal.component.html',
})
export class FileUploadModalComponent implements OnInit {
  constructor(private commonService: CommonService,
    private activeModal: BsModalRef) { }

  @Input() allowMultiple: boolean = false;
  @Input() title: string = "";
  @Input() acceptedFileExtensions: Array<string> | undefined;

  @ViewChild('fileInput') fileInput: ElementRef = {
    nativeElement: {}
  };

  files: Array<File> = [];
  isBusy: Array<any> = [];
  message: string | undefined;
  success: boolean = false;

  ngOnInit() {
  }

  acceptedFileExtensionString() {
    if (this.acceptedFileExtensions != null) return this.acceptedFileExtensions.join(', ');
    return null;
  }

  handleFileInput(files: FileList | undefined) {
  if (files == null || files.length == 0) return;

    this.message = undefined;

    for (var i = 0; i < files.length; i++) {
      let extension = files[i].name.split('.').pop();
      if (this.acceptedFileExtensions == null ||
        (this.acceptedFileExtensions != null && this.acceptedFileExtensions.some((value, index) => { return value.toLowerCase() == extension!.toLowerCase(); }))) {
        this.files.push(files[i]);
      }
      else {
        var fileExtString = this.acceptedFileExtensions.join(', ');
        fileExtString = fileExtString.replace(/,(?=[^,]*$)/, ' or ');

        this.message = `${files[i].name} is not an allowed file type.`;
        this.message += '<br/><br/>File must be a ' + fileExtString + ' file';
        break;
      }
    }

    this.fileInput.nativeElement.value = "";
  }

  remove(index: number) {
    this.files.splice(index, 1);
  }

  add(form: NgForm) {
    if (this.files.length == 0) {
      this.message = "You must select at least one file";
      return;
    }

    this.success = true;
    this.activeModal.hide();
  }

  close(closeMethod: string) {
    this.activeModal.hide();
  }
}
