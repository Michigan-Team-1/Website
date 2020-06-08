


import {ISelectOption} from 'app.common/dtos/SelectOptionDto';
enum GalleryTypeEnum {
        Public = 1,
        MyGallery = 2,
        AdminMode = 3,
         
}

export class GalleryTypeEnum_class {

public static enum = GalleryTypeEnum;

  public static readonly enumByText : { [key : string ]: number} = {
    ['Public']: 1,
    ['MyGallery']: 2,
    ['AdminMode']: 3,
    
  };
  public static readonly enumByNumber : { [key : number ]: string} = {
    [1]: "Public",
    [2]: "My Gallery",
    [3]: "Admin Mode",
    
  };

  public static readonly enumAsSelectOptions:Array<ISelectOption<number>> = [
  {value: 1, text: "Public"},
  {value: 2, text: "My Gallery"},
  {value: 3, text: "Admin Mode"},
  ]
}

