import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { LoginDto } from '../../core/models/login.dto';
import { UserService } from '../../core/services/user.service';
import { Router, RouterModule } from '@angular/router';
import { BaseFormComponent } from '../../shared/components/base/base-form.component';
import { RouteConstants } from '../../core/constants/route.constant';
import { ValidationErrorDisplayComponent } from '../../shared/components/validation-error-display/validation-error-display.component';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, ValidationErrorDisplayComponent],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent extends BaseFormComponent {
  routes = RouteConstants;

  constructor(
    fb: FormBuilder,
    private userService: UserService,
    private router: Router,
  ) {
    super(fb);
  }

  protected initForm(): void {
    this.form = this.fb.group({
      [this.formFields.USERNAME]: ['', Validators.required],
      [this.formFields.PASSWORD]: ['', Validators.required],
    });
  }

  protected submit(): void {
    const dto = this.form.value as LoginDto;
    this.userService.login(dto).subscribe({
      next: () => {
        this.router.navigate([RouteConstants.DASHBOARD_PATH]);
      },
    });
  }
}
