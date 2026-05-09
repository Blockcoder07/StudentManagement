import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import { AuthResponse, AuthSession, LoginRequest } from '../models/auth.model';

const STORAGE_KEY = 'sm.auth.session';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  private readonly sessionSignal = signal<AuthSession | null>(this.readFromStorage());

  readonly session = computed(() => this.sessionSignal());
  readonly isAuthenticated = computed(() => {
    const session = this.sessionSignal();
    if (!session) return false;
    return session.expiresAtDate.getTime() > Date.now();
  });
  readonly currentUser = computed(() => this.sessionSignal());

  login(payload: LoginRequest): Observable<ApiResponse<AuthResponse>> {
    return this.http.post<ApiResponse<AuthResponse>>(`${environment.apiBaseUrl}/auth/login`, payload).pipe(
      tap(response => {
        if (response.success && response.data) {
          this.persistSession(response.data);
        }
      })
    );
  }

  logout(redirect = true): void {
    localStorage.removeItem(STORAGE_KEY);
    this.sessionSignal.set(null);
    if (redirect) {
      this.router.navigate(['/login']);
    }
  }

  getToken(): string | null {
    return this.sessionSignal()?.accessToken ?? null;
  }

  private persistSession(payload: AuthResponse): void {
    const session: AuthSession = {
      ...payload,
      expiresAtDate: new Date(payload.expiresAt)
    };
    localStorage.setItem(STORAGE_KEY, JSON.stringify(payload));
    this.sessionSignal.set(session);
  }

  private readFromStorage(): AuthSession | null {
    const raw = typeof localStorage !== 'undefined' ? localStorage.getItem(STORAGE_KEY) : null;
    if (!raw) return null;

    try {
      const parsed = JSON.parse(raw) as AuthResponse;
      const expiresAtDate = new Date(parsed.expiresAt);

      // Defence in depth: if the token itself has expired, drop it.
      const decoded = jwtDecode<{ exp?: number }>(parsed.accessToken);
      if (decoded?.exp && decoded.exp * 1000 < Date.now()) {
        localStorage.removeItem(STORAGE_KEY);
        return null;
      }

      return { ...parsed, expiresAtDate };
    } catch {
      localStorage.removeItem(STORAGE_KEY);
      return null;
    }
  }
}
