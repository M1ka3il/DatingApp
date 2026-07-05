import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Nav } from "../layout/nav/nav";
import { AccountService } from '../core/services/account-service.service';

@Component({
  selector: 'app-root',
  imports: [Nav, RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  private accountService = inject(AccountService);
  protected title = 'Dating App';

  ngOnInit() {
    this.accountService.loadCurrentUser();
  }
}
