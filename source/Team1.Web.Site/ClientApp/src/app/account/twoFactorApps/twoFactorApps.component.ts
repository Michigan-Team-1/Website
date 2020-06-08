import { Component } from '@angular/core';

@Component({
  selector: 'twoFactorApps',
  templateUrl: './twoFactorApps.component.html',
})
export class TwoFactorAppsComponent {
  constructor() { }

  googlePlay = require("assets/Google_Play.png");
  appStore = require("assets/App_Store.png");

}
