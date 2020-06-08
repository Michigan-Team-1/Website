import { Component } from '@angular/core';
import { AuthService } from 'app.common/services/auth.service';

@Component({
  selector: '[navLinks]',
    templateUrl: './navLinks.component.html'
})
export class NavLinksComponent {
  constructor(public authService: AuthService) {
  }
}
