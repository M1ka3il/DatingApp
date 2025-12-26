import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-nav',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './nav.html',
  styleUrls: ['./nav.css']
})
export class Nav {

  protected creds: any = {}
  
  login()
  {
    console.log(this.creds);
  }

}
