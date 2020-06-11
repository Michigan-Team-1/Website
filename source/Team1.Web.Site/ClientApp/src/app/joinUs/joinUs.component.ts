import { Component, OnInit } from '@angular/core';
import { AuthService } from 'app.common/services/auth.service';
import { UsersService } from 'app/users/users.service';
import { IUser } from 'app.common/dtos/UserDto';

@Component({
  selector: 'joinUs',
  templateUrl: './joinUs.component.html'
})
export class JoinUsComponent implements OnInit {
  constructor(public authService: AuthService, private usersServer: UsersService) {
  }
  isBusy: number = 0;

  boardOfDirectors: Array<IUser> = [];

  ngOnInit() {
    this.usersServer.getBoardOfDirectors().subscribe((data) => {
      this.boardOfDirectors = data;
    });
  }
}
