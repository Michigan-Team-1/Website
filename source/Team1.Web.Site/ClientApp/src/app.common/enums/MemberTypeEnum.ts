


import {ISelectOption} from 'app.common/dtos/SelectOptionDto';
enum MemberTypeEnum {
        Prefect = 1,
        VicePrefect = 2,
        Secretary = 3,
        Treasurer = 4,
         
}

export class MemberTypeEnum_class {

public static enum = MemberTypeEnum;

  public static readonly enumByText : { [key : string ]: number} = {
    ['Prefect']: 1,
    ['VicePrefect']: 2,
    ['Secretary']: 3,
    ['Treasurer']: 4,
    
  };
  public static readonly enumByNumber : { [key : number ]: string} = {
    [1]: "Prefect",
    [2]: "Vice Prefect",
    [3]: "Secretary",
    [4]: "Treasurer",
    
  };

  public static readonly enumAsSelectOptions:Array<ISelectOption<number>> = [
  {value: 1, text: "Prefect"},
  {value: 2, text: "Vice Prefect"},
  {value: 3, text: "Secretary"},
  {value: 4, text: "Treasurer"},
  ]
}

