import { Component, OnInit } from '@angular/core';

import { BsModalService, BsModalRef } from 'ngx-bootstrap';

import { TaskAddEditComponent } from './taskAddEdit.component'
import { TasksService } from './tasks.service';

import { ITask, ITask_PropertyAttributes } from 'app.common/dtos/TaskDto';
import { AuthService } from 'app.common/services/auth.service';
import { defaultPageSize } from 'app.common/constants';
import { SmartTable, from } from 'smart-table-ng';
import * as crud from 'smart-table-crud';
import { TaskCategoryEnum_class } from 'app.common/enums/TaskCategoryEnum';
import { MemberTypeEnum_class } from 'app.common/enums/MemberTypeEnum';

@Component({
  selector: 'tasks',
  templateUrl: './tasks.component.html',
  providers: [{
    provide: SmartTable,
    useFactory: (tasksService: TasksService) => from(tasksService.getTasks(), {
      search: {},
      sort: { pointer: "dueDate", direction: "asc" },
      filter: {
        isActive: [{ operator: "equals", type: "string", value: "true" }]
      },
      slice: { page: 1, size: defaultPageSize }
    }, <any>crud),
    deps: [TasksService]
  }]
})
export class TasksComponent implements OnInit {
  constructor(private tasksService: TasksService, private modalService: BsModalService,
    private authService: AuthService, private table: SmartTable<ITask>
  ) { }

  columnCount: number = 5;
  isCollapsedFilters: boolean = true;
  isBusy:boolean = false;
  dtoPropertyAttributes = ITask_PropertyAttributes;
  taskCategories = TaskCategoryEnum_class.enumAsSelectOptions;
  memberTypes = MemberTypeEnum_class.enumAsSelectOptions;

  ngOnInit() {
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

    let modal: BsModalRef = this.modalService.show(TaskAddEditComponent, { initialState, class: 'modal-lg' });
  }
}
