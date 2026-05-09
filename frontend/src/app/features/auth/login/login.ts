import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly toastr = inject(ToastrService);

  protected readonly submitting = signal(false);
  protected readonly showPassword = signal(false);

  protected readonly form = this.fb.nonNullable.group({
    username: ['admin', [Validators.required, Validators.minLength(3)]],
    password: ['Admin@123', [Validators.required, Validators.minLength(6)]]
  });

  togglePassword(): void {
    this.showPassword.update(value => !value);
  }

  submit(): void {
    if (this.form.invalid || this.submitting()) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    this.auth.login(this.form.getRawValue()).subscribe({
      next: response => {
        this.submitting.set(false);
        if (response.success) {
          this.toastr.success(response.message);
          const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') ?? '/dashboard';
          this.router.navigateByUrl(returnUrl);
        }
      },
      error: () => this.submitting.set(false)
    });
  }
}
