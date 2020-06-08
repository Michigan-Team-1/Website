


import {ISelectOption} from 'app.common/dtos/SelectOptionDto';
enum TaskCategoryEnum {
        Confirmation = 1,
        SendEmail = 2,
        SendLetter = 3,
        Print = 4,
        PhoneCall = 5,
        Other = 100,
         
}

export class TaskCategoryEnum_class {

public static enum = TaskCategoryEnum;

  public static readonly enumByText : { [key : string ]: number} = {
    ['Confirmation']: 1,
    ['SendEmail']: 2,
    ['SendLetter']: 3,
    ['Print']: 4,
    ['PhoneCall']: 5,
    ['Other']: 100,
    
  };
  public static readonly enumByNumber : { [key : number ]: string} = {
    [1]: "Confirmation",
    [2]: "Send Email",
    [3]: "Send Letter",
    [4]: "Print",
    [5]: "Phone Call",
    [100]: "Other",
    
  };

  public static readonly enumAsSelectOptions:Array<ISelectOption<number>> = [
  {value: 1, text: "Confirmation"},
  {value: 2, text: "Send Email"},
  {value: 3, text: "Send Letter"},
  {value: 4, text: "Print"},
  {value: 5, text: "Phone Call"},
  {value: 100, text: "Other"},
  ]
}

