import { Component, OnInit, Input } from '@angular/core';
import { NgForm } from '@angular/forms';

import { BsModalRef } from 'ngx-bootstrap';

import { getErrorMessageFromServerResponse } from "app.common/helpers/general";
import { setIsUpdatedIfChanged, deepClone } from "app.common/helpers/object";
import { CommonService } from 'app.common/services/common.service';

import { ITask_PropertyAttributes, ITask } from 'app.common/dtos/TaskDto';
import { TasksService } from './tasks.service';
import { TaskCategoryEnum_class } from 'app.common/enums/TaskCategoryEnum';
import { MemberTypeEnum_class } from 'app.common/enums/MemberTypeEnum';
import { ISelectOption } from 'app.common/dtos/SelectOptionDto';
import * as moment from 'moment';
import { ITaskMemberType } from 'app.common/dtos/TaskMemberTypeDto';

@Component({
  selector: 'taskAddEdit',
  templateUrl: './taskAddEdit.component.html',
})
export class TaskAddEditComponent implements OnInit {
  constructor(private tasksService: TasksService,
    private activeModal: BsModalRef,
    private commonService: CommonService) { }

  isBusy: number = 0;
  @Input() dtoOriginal: ITask | undefined;
  defaultDto: ITask = { isActive: true, isUpdated: true };
  dto: ITask = deepClone(this.defaultDto);
  dtoPropertyAttributes = ITask_PropertyAttributes;
  memberType: ISelectOption<number> | undefined;
  memberTypes: Array<ISelectOption<number>> = MemberTypeEnum_class.enumAsSelectOptions;
  message: string | undefined;
  success: boolean = false;
  dtoUpdated: boolean = false;
  dataUpdated: boolean = false;
  taskCategories = TaskCategoryEnum_class.enumAsSelectOptions;
  months: Array<ISelectOption<number>> = moment.months().map((item, index) => { return { value: index + 1, text: item }; });

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

  getMemberTypes() {
    if (this.dto.taskMemberTypes == null || this.dto.taskMemberTypes.length == 0) return this.memberTypes;
    let self = this;
    return this.memberTypes.filter((value) => {
      return !self.dto.taskMemberTypes!.some((item) => { return item.memberTypeId == value.value; });
    });
  }

  addMemberType() {
    if (this.memberType == null) return;
    if (this.dto.taskMemberTypes == null) this.dto.taskMemberTypes = [];
    var memberType: ITaskMemberType = { isAdded: true, memberTypeId: this.memberType.value, memberTypeString: this.memberType.text };
    this.dto.taskMemberTypes.push(memberType);

    this.memberType = undefined;
  }

  removeMemberType(index: number) {
    if (this.dto.taskMemberTypes == null) return;

    var taskMemberType = this.dto.taskMemberTypes[index];
    if (taskMemberType.isAdded) {
      this.dto.taskMemberTypes.splice(index, 1);
      return;
    }

    taskMemberType.isDeleted = true;
  }

  save(form: NgForm) {
    if (!form.valid) {
      return;
    }
    this.message = undefined;
    if (this.dto.taskMemberTypes == null || this.dto.taskMemberTypes.every((value, index) => { return value.isDeleted!; })) {
      this.message = "Must have at least  one responsible member type chosen.";
      return;
    }

    setIsUpdatedIfChanged(this.dto, this.dtoOriginal);

    this.isBusy++;
    this.tasksService.saveTask(this.dto).subscribe((data) => {
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
