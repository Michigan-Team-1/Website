import { Component, OnInit, Input } from '@angular/core';
import { NgForm, FormControl, ControlContainer } from '@angular/forms';
import { Observable, forkJoin } from "rxjs";

import { CommonService } from 'app.common/services/common.service';

import { IAddressObj, IAddressObj_PropertyAttributes } from 'app.common/dtos/AddressObjDto';
import { ICountry } from 'app.common/dtos/CountryDto';
import { IGoverningDistrict } from 'app.common/dtos/GoverningDistrictDto';
import { getRandomNameSuffix } from 'app.common/helpers/general';

@Component({
  selector: 'addressEdit',
  templateUrl: './addressEdit.component.html',
  viewProviders: [{ provide: ControlContainer, useExisting: NgForm }]
})
export class AddressEditComponent implements OnInit {
  constructor(private commonService: CommonService) { }

  @Input("form") f: NgForm = new NgForm([],[]);
  @Input() addressObj: IAddressObj = {};
  @Input() isRequired: boolean = false;
  @Input() nameSuffix: string = getRandomNameSuffix();

  dtoPropertyAttributes = IAddressObj_PropertyAttributes;
  countries: Array<ICountry> = [];
  governingDistricts: Array<IGoverningDistrict> = [];
  isBusy: boolean = false;

    ngOnInit() {
        this.isBusy = true;

        forkJoin(this.commonService.getCountries(), this.commonService.getGoverningDistricts())
        .subscribe(
            (data: any) => {
                this.countries = data[0];
                this.governingDistricts = data[1];
                
                this.ngOnChanges();
                this.isBusy = false;
            },
            (error:any) => { },
            () => { }
        );
    }

ngOnChanges() {
    if (this.countries.length > 0 && this.addressObj.country == null) {
      // default to USA
      this.addressObj.country = this.countries.find((item, index) => { return item.countryId == 840; });
      this.addressObj.countryId = 840;
      this.addressObj.governingDistrictId = undefined;
    }
  }

    countryChanged() {
      if (this.addressObj.country) {
        this.addressObj.countryId = this.addressObj.country.countryId;
      }
      else {
        this.addressObj.countryId = undefined;
      }
    }

    countryCompareWith(o1: ICountry, o2: ICountry):boolean {
        return o1 != null && o2 != null && o1.countryId == o2.countryId;
    }
}
