import { Component } from '@angular/core';
import { UsersService } from 'app/users/users.service';
import { IUser } from 'app.common/dtos/UserDto';

@Component({
    selector: 'footerNav',
    templateUrl: './footerNav.component.html'
})
export class FooterNavComponent {
  constructor(private usersServer: UsersService) {
  }

  boardOfDirectors: Array<IUser> = [];

  ngOnInit() {
    this.usersServer.getBoardOfDirectors().subscribe((data) => {
      this.boardOfDirectors = data;
    });
  }
}
