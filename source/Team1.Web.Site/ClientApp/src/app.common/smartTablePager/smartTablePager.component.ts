import { Component, OnInit, Input } from '@angular/core';
import { pageSizes } from 'app.common/constants';

@Component({
  selector: 'smartTablePager',
  templateUrl: './smartTablePager.component.html',
})
export class SmartTablePagerComponent implements OnInit {
  constructor() {
  }

  @Input() pager: any;
  @Input() pageSizes: number[] = pageSizes

  ngOnInit() {
  } 

  numberPages(): number {
    return Math.ceil(this.pager.length / this.pager.size);
  }

  pageChange() {
    // clean up user input
    if (isNaN(this.pager.page)) {
      this.pager.page = 1;
    }
    else {
      this.pager.page = Math.round(this.pager.page);
    }
    if (this.pager.page < 1) {
      this.pager.page = 1;
    }
    else if (this.pager.page > this.numberPages()) {
      this.pager.page = this.numberPages();
    }

    this.pager.selectPage(this.pager.page);
  }
}
