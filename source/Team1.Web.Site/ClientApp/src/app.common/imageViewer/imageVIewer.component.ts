import { Component, OnInit, Input } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap';

@Component({
  selector: 'imageViewer',
  templateUrl: './imageViewer.component.html',
})
export class ImageViewerComponent implements OnInit {
  constructor(private activeModal: BsModalRef) {
  }

    @Input() imageUrl: string | undefined;
    isMiddleZoom: boolean = true;

  ngOnInit() {
  }

    close(isOverride: boolean) {
        if (!isOverride && this.isMiddleZoom) {
            this.isMiddleZoom = false;
            
            return false;
        }
        this.activeModal.hide();

        return false;
  }
}
