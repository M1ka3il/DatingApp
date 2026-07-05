import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { inject } from '@angular/core';
import { AccountService } from '../../core/services/account-service.service';
import { ToastService } from '../../core/services/toast-service';

@Component({
  selector: 'app-nav',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './nav.html',
  styleUrls: ['./nav.css']
})
export class Nav {
  private accountService = inject(AccountService);
  private toast = inject(ToastService);
  protected creds: any = {}
  protected loggedIn = signal(false);


  login()
  {
    this.accountService.login(this.creds).subscribe({
      next: () => {
        this.loggedIn.set(true);
        this.creds = {};
        this.toast.success('Logged in successfully');
      },
      error: () => this.toast.error('Failed to log in')
      })
  }

  logout()
  {
    this.loggedIn.set(false);
    this.toast.info('Logged out');
  }
}
