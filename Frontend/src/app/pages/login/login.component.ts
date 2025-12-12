import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { LoginDto } from '../../core/models/login.dto';
import { AuthService } from '../../core/services/auth.service';
import { Router, RouterModule } from '@angular/router';
import { BaseFormComponent } from '../../shared/components/base/base-form.component';
import { RouteConstants } from '../../core/constants/route.constant';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent extends BaseFormComponent {
  routes = RouteConstants;

  constructor(
    fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
  ) {
    super(fb);
  }

  protected initForm(): void {
    this.form = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  protected submit(): void {
    const dto = this.form.value as LoginDto;
    this.authService.login(dto).subscribe({
      next: () => {
        this.router.navigate(['/']);
      },
    });
  }
}
