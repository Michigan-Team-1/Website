import { DefaultUrlSerializer, UrlTree } from '@angular/router';
import { Injectable } from "@angular/core";

@Injectable()
export class LowerCaseUrlSerializer extends DefaultUrlSerializer {
  parse(url: string): UrlTree {
    // If you lower it in the optional step 
    // you don't need to use "toLowerCase" 
    // when you pass it down to the next function
    var urlParts = url.split('?');
    url = urlParts[0].toLowerCase();
    if (urlParts.length > 1) {
      url += `?${urlParts[1]}`;
    }
    return super.parse(url);
  }
}
