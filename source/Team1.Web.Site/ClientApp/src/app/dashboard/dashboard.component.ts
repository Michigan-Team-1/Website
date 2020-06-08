import { Component, OnInit } from '@angular/core';
import { AnnouncementsService } from 'app/announcements/announcements.service';
import { EventsService } from 'app/events/events.service';
import { forkJoin } from 'rxjs';
import { IAnnouncement } from 'app.common/dtos/AnnouncementDto';
import { IEvent } from 'app.common/dtos/EventDto';
import { PicturesService } from 'app/pictures/pictures.service';
import { IPicture } from 'app.common/dtos/PictureDto';
import { PicturesControllerAPI } from 'app.common/apis/PicturesController';
import { ILocation } from 'app.common/dtos/LocationDto';
import { ImageViewerComponent } from 'app.common/imageViewer/imageVIewer.component';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';

@Component({
    selector: 'dashboard',
    templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit {
  constructor(private announcementsService: AnnouncementsService,
    private eventsService: EventsService,
    private picturesService: PicturesService,
    private modalService: BsModalService
  ) {
  }

  announcements: Array<IAnnouncement> = [];
  events: Array<IEvent> = [];
  pictures: Array<IPicture> = [];
  pictureUrlFunction = PicturesControllerAPI.GetPictureForViewing;
  isBusy: number = 0;

  ngOnInit() {
    this.isBusy++;
    forkJoin(this.announcementsService.getAnnouncementsForDashboard(), this.eventsService.getEventsForDashboard(), this.picturesService.getRandomPictures()).subscribe((data) => {
      this.isBusy--;
      this.announcements = data[0];
      this.events = data[1];
      this.pictures = data[2];
    });
  }

  getAddress(location: ILocation) : string {
    var address = `${location!.addressObj!.address1}`;
    if (location!.addressObj!.address2 != null)
      address += ` ${location!.addressObj!.address2}`;
    if (location!.addressObj!.address3 != null)
      address += ` ${location!.addressObj!.address3}`;

    if (location!.addressObj!.city != null)
      address += `, ${location!.addressObj!.city}`;

    if (location!.addressObj!.governingDistrictName != null)
      address += `, ${location!.addressObj!.governingDistrictName}`;

    if (location!.addressObj!.postalCode != null)
      address += `, ${location!.addressObj!.postalCode}`;

    return encodeURIComponent(address);
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
