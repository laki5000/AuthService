import { Component } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { RegisterDto } from '../../core/models/register.dto';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { BaseFormComponent } from '../../shared/components/base/base-form.component';
import { RouteConstants } from '../../core/constants/route.constant';
import { matchFieldsValidator } from '../../shared/validators/match-fields.validator';
import { FormFieldConstants } from '../../core/constants/form-field.constant';
import { FormErrorConstants } from '../../core/constants/form-error.constant';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent extends BaseFormComponent {
  routes = RouteConstants;

  constructor(
    fb: FormBuilder,
    private authService: AuthService,
  ) {
    super(fb);
  }

  protected initForm(): void {
    this.form = this.fb.group({
      username: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      emailAgain: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
      passwordAgain: ['', Validators.required],
      firstName: [null],
      lastName: [null],
    },
    {
      validators: [
        matchFieldsValidator(FormFieldConstants.EMAIL, FormFieldConstants.EMAIL_AGAIN, FormErrorConstants.EMAIL_MISMATCH),
        matchFieldsValidator(FormFieldConstants.PASSWORD, FormFieldConstants.PASSWORD_AGAIN, FormErrorConstants.PASSWORD_MISMATCH)
      ]
    });
  }

  protected submit(): void {
    const dto = this.form.value as RegisterDto;
    this.authService.register(dto).subscribe();
  }
}
