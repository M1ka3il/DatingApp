import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { inject, Injectable } from '@angular/core';
import { AccountService } from '../../core/services/account-service.service';

@Component({
  selector: 'app-nav',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './nav.html',
  styleUrls: ['./nav.css']
})
export class Nav {
  private accountService = inject(AccountService);
  protected creds: any = {}
  
  login()
  {
    this.accountService.login(this.creds).subscribe({
      next: (result) => console.log(result),
      error: (error) => alert(error.message)
      })
  }
}
