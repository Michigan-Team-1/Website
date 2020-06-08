/* SystemJS module definition */
declare var module: NodeModule;
interface NodeModule {
  id: string;
}

//declare module "inputmask";
//declare module "Inputmask.Phone.Extensions";

declare module 'smart-table-crud' {
  import { SmartTable, TableState } from 'smart-table-ng';
  export var crud: any;
  //export default function <T>(input: { table: SmartTableCore<T>, data: T[], tableState: TableState }):void;
  //export default function <U>(input: { table: SmartTableCore<INotification>; data: INotification[]; tableState: TableState; }): U;
  interface Crud<T> {
    get(index: number): T;
    insert(newVal: T): void;
    patch(index: number, newVal: T): void;
    remove(index: number): void;
    update(index: number, newVal: T): void;
  }
}

