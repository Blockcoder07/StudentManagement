import { Component, computed, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './shell.html',
  styleUrl: './shell.scss'
})
export class ShellComponent {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  protected readonly username = computed(() => this.auth.currentUser()?.username ?? 'User');
  protected readonly role = computed(() => this.auth.currentUser()?.role ?? '');

  logout(): void {
    this.auth.logout();
  }
}
