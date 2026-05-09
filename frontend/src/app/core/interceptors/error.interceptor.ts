import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const toastr = inject(ToastrService);
  const auth = inject(AuthService);
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const fallback = 'Something went wrong. Please try again.';
      const apiBody = error.error as { message?: string; errors?: string[] } | null;

      const apiMessage = apiBody?.message;
      const apiErrors = apiBody?.errors ?? [];

      switch (error.status) {
        case 0:
          toastr.error('Cannot reach the server. Check your connection.', 'Network');
          break;
        case 400:
          toastr.error([apiMessage, ...apiErrors].filter(Boolean).join('\n') || fallback, 'Validation');
          break;
        case 401:
          if (!req.url.endsWith('/auth/login')) {
            auth.logout(false);
            toastr.warning('Your session has expired. Please log in again.');
            router.navigate(['/login']);
          } else {
            toastr.error(apiMessage || 'Invalid credentials.', 'Login failed');
          }
          break;
        case 403:
          toastr.error('You are not authorised to perform this action.', 'Forbidden');
          break;
        case 404:
          toastr.error(apiMessage || 'Resource not found.', 'Not found');
          break;
        case 409:
          toastr.error(apiMessage || 'Conflict detected.', 'Conflict');
          break;
        default:
          toastr.error(apiMessage || fallback, 'Error');
      }

      return throwError(() => error);
    })
  );
};
