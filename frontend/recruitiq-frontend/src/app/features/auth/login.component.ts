import { Component, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NgClass } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, NgClass],
  templateUrl: './login.component.html',
})
export class LoginComponent {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);

  form = this.fb.group({
    email:    ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
  });

  loading = false;
  error = '';

  get email()    { return this.form.controls.email; }
  get password() { return this.form.controls.password; }

  submit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.error = '';
    this.loading = true;
    const { email, password } = this.form.getRawValue();
    this.auth.login({ email: email!, password: password! }).subscribe({
      next: () => this.router.navigate(['/dashboard']),
      error: () => {
        this.error = 'Invalid email or password.';
        this.loading = false;
      },
    });
  }
}
