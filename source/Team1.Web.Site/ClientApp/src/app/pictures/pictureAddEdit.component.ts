import { Component, OnInit, Input, ViewChild, ElementRef } from '@angular/core';
import { NgForm } from '@angular/forms';

import { BsModalRef } from 'ngx-bootstrap';

import { getErrorMessageFromServerResponse } from "app.common/helpers/general";
import { setIsUpdatedIfChanged, deepClone } from "app.common/helpers/object";
import { CommonService } from 'app.common/services/common.service';

import { PicturesService } from './pictures.service';
import { IPicture , IPicture_PropertyAttributes} from 'app.common/dtos/PictureDto';
import { AuthService } from 'app.common/services/auth.service';
import { allowedFileExtensions } from 'app.common/serverConstants/AllowedFileExtensions';
import { PicturesControllerAPI } from 'app.common/apis/PicturesController';

@Component({
  selector: 'pictureAddEdit',
  templateUrl: './pictureAddEdit.component.html',
})
export class PictureAddEditComponent implements OnInit {
  constructor(private picturesService: PicturesService,
    private activeModal: BsModalRef,
    public authService: AuthService,
    private commonService: CommonService) { }

  isBusy: number = 0;
  @Input() dtoOriginal: IPicture | undefined;
  defaultDto: IPicture = { isActive: true, isUpdated: true };
  dto: IPicture = deepClone(this.defaultDto);
  dtoPropertyAttributes = IPicture_PropertyAttributes;
  message: string | undefined;
  success: boolean = false;
  dtoUpdated: boolean = false;
  dataUpdated: boolean = false;
  newSrc: any;
  pictureUrlFunction = PicturesControllerAPI.GetPictureForViewing;
  acceptedFileExtensions: Array<string> = allowedFileExtensions.pictureDocuments.split(",");

  @ViewChild('fileInput') fileInput: ElementRef = {
    nativeElement: {}
  };

  ngOnInit() {
    if (this.dtoOriginal == null) this.dtoOriginal = deepClone(this.defaultDto);
    this.dto = this.dtoOriginal != null ? deepClone(this.dtoOriginal) : this.dto;
  }
  
  ngDoCheck() {
    this.dataIsUpdated();
  }

  dataIsUpdated(): boolean {
    this.dtoUpdated = setIsUpdatedIfChanged(this.dto, this.dtoOriginal);

    this.dataUpdated = this.dtoUpdated;
    return this.dataUpdated;
  }

  acceptedFileExtensionString() {
    if (this.acceptedFileExtensions != null) return this.acceptedFileExtensions.join(', ');
    return null;
  }

  handleFileInput(files: FileList | undefined) {
    if (files == null || files.length == 0) return;

    this.message = undefined;
    var reader = new FileReader();

    for (var i = 0; i < files.length; i++) {
      let extension = files[i].name.split('.').pop();
      if (this.acceptedFileExtensions == null ||
        (this.acceptedFileExtensions != null && this.acceptedFileExtensions.some((value, index) => { return value.toLowerCase() == extension!.toLowerCase(); }))) {
        this.dto.document = {
          documentDisplayName: files[i].name,
          documentFilename: files[i].name,
          mimeType: files[i].type,
        };
        this.dto.fileUpload = files[i];
        this.dto.isUpdated = true;
        this.dto.isActive = true;
        reader.onload = () => {
          this.newSrc = reader.result;
        };
        reader.readAsDataURL(files[i]);
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

  save(form: NgForm) {
    if (!form.valid) {
      return;
    }

    setIsUpdatedIfChanged(this.dto, this.dtoOriginal);

    this.isBusy++;
    this.picturesService.savePicture(this.dto).subscribe((data) => {
      this.isBusy--;
      this.dto = data;
      this.success = true;
      this.activeModal.hide();
    }, (error) => {
      this.isBusy--;
      this.message = getErrorMessageFromServerResponse(error);
    }, () => {
    });
  }

  close(closeMethod: string) {
    this.activeModal.hide();
  }
}
