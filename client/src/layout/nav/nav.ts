import { Component, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { NavigationEnd, Router, RouterLink, RouterLinkActive } from '@angular/router';
import { filter } from 'rxjs';
import { AccountService } from '../../core/services/account-service.service';
import { ToastService } from '../../core/services/toast-service';

@Component({
  selector: 'app-nav',
  standalone: true,
  imports: [FormsModule, RouterLink, RouterLinkActive],
  templateUrl: './nav.html',
  styleUrls: ['./nav.css']
})
export class Nav {
  protected accountService = inject(AccountService);
  private toast = inject(ToastService);
  protected creds: any = {}
  protected loggedIn = computed(() => !!this.accountService.currentUser());
  // Mobile menu (below the lg breakpoint).
  protected menuOpen = signal(false);

  constructor() {
    // Close the mobile menu whenever the user navigates.
    inject(Router).events
      .pipe(filter((e) => e instanceof NavigationEnd), takeUntilDestroyed())
      .subscribe(() => this.menuOpen.set(false));
  }

  toggleMenu() {
    this.menuOpen.update((open) => !open);
  }

  login()
  {
    this.accountService.login(this.creds).subscribe({
      next: () => {
        this.creds = {};
        this.menuOpen.set(false);
        this.toast.success('Logged in successfully');
      },
      error: () => this.toast.error('Failed to log in')
      })
  }

  logout()
  {
    this.accountService.logout();
    this.menuOpen.set(false);
    this.toast.info('Logged out');
  }
}
