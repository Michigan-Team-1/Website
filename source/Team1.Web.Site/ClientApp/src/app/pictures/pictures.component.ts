import { Component, OnInit } from '@angular/core';

import { BsModalService, BsModalRef } from 'ngx-bootstrap';

import { PictureAddEditComponent } from './pictureAddEdit.component'
import { PicturesService } from './pictures.service';

import { AuthService } from 'app.common/services/auth.service';
import { defaultPageSize } from 'app.common/constants';
import { SmartTable, of } from 'smart-table-ng';
import * as crud  from 'smart-table-crud';
import { IPicture, IPicture_PropertyAttributes } from 'app.common/dtos/PictureDto';
import { ActivatedRoute } from '@angular/router';
import { PicturesControllerAPI } from 'app.common/apis/PicturesController';
import { pageSizes } from 'app.common/constants';
import { deepClone } from 'app.common/helpers/object';
import { TableState } from 'smart-table-core';
import { ISelectOption } from 'app.common/dtos/SelectOptionDto';
import { forkJoin } from 'rxjs';
import { ImageViewerComponent } from 'app.common/imageViewer/imageVIewer.component';
 
var defaultTableState: TableState = {
  search: {},
  sort: { pointer: "name", direction: <any>"asc" },
  filter: {
  },
  slice: { page: 1, size: defaultPageSize }
};

@Component({
  selector: 'pictures',
  templateUrl: './pictures.component.html',
  providers: [{
    provide: SmartTable,
    useValue: of([], deepClone(defaultTableState), <any>crud)
  }]
})
export class PicturesComponent implements OnInit {
    
  constructor(private picturesService: PicturesService, private modalService: BsModalService,
    public authService: AuthService, private table: SmartTable<IPicture>, private route: ActivatedRoute
  ) { }

  isBusy:boolean = false;
  dtoPropertyAttributes = IPicture_PropertyAttributes;
  pictureUrlFunction = PicturesControllerAPI.GetPictureForViewing;
    pageSizes: number[] = deepClone(pageSizes);
  galleryType: number = 1;
  galleryTypes: Array<ISelectOption<number>> = [];

  ngOnInit() {
    this.isBusy = true;
    forkJoin(this.picturesService.getGalleryTypes(), this.picturesService.getPictures(this.galleryType)).subscribe((data) => {
      this.galleryTypes = data[0];
        this.table.use(data[1]);
      this.isBusy = false;
    }, (error) => {
      this.isBusy = false;
    }, () => { });
  }
  
  loadPictures() {
    this.isBusy = true;
    this.picturesService.getPictures(this.galleryType).subscribe((data) => {
      let tableState = this.table.getTableState();
      let isActiveFilter: any = tableState.filter.isActive != null && tableState.filter.isActive.length > 0 ? tableState.filter.isActive[0].value : true;
      switch (this.galleryType) {
        case 1:
          tableState.filter = {};
          break;
        case 2:
          tableState.filter.isActive = [{ operator: <any>"equals", type: "string", value: isActiveFilter }];
          tableState.filter.isApproved = [];
          break;
        case 3:
          tableState.filter.isActive = [{ operator: <any>"equals", type: "string", value: isActiveFilter }];
          tableState.filter.isApproved = [{ operator: <any>"equals", type: "string", value: false }];
          break;
      }
      this.table.use(data, tableState);
      this.isBusy = false;
    }, (error) => {
      this.isBusy = false;
    }, () => { });
  }

    galleryType_changed() {
    this.loadPictures();
  }
  
  edit(index: number | undefined) {
    const initialState = {
      dtoOriginal: index != null ? (<any>this.table).get(index) : undefined
    };

    let subcription = this.modalService.onHide.subscribe((reason: string) => {
      if (modal.content.success) {
        let dto = modal.content.dto;
        if (index == null) {
          (<any>this.table).insert(dto);
        }
        else {
          (<any>this.table).update(index, dto);
        }
      }

      subcription.unsubscribe();
    });

    let modal: BsModalRef = this.modalService.show(PictureAddEditComponent, { initialState, class: 'modal-lg' });
  }

  showImage(imageUrl: string) {
    const initialState = {
      imageUrl: imageUrl
    };

    let subcription = this.modalService.onHide.subscribe((reason: string) => {
      subcription.unsubscribe();
    });

    let modal: BsModalRef = this.modalService.show(ImageViewerComponent, { initialState, class: 'modal-full-screen' });
  }
}
