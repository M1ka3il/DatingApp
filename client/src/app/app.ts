import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Nav } from "../layout/nav/nav";

@Component({
  selector: 'app-root',
  imports: [Nav, RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'Dating App';
}
