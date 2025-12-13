import { Component } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { UserService } from '../../core/services/user.service';
import { RegisterDto } from '../../core/models/register.dto';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { BaseFormComponent } from '../../shared/components/base/base-form.component';
import { RouteConstants } from '../../core/constants/route.constant';
import { ValidationErrorDisplayComponent } from '../../shared/components/validation-error-display/validation-error-display.component';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { matchFieldValidator } from '../../shared/validators/match-field.validator';
import { FormGroupErrorKeyConstants } from '../../core/constants/form-error-key.constant';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    ValidationErrorDisplayComponent,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
  ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent extends BaseFormComponent {
  routes = RouteConstants;

  constructor(
    fb: FormBuilder,
    private userService: UserService,
  ) {
    super(fb);
  }

  protected initForm(): void {
    this.form = this.fb.group({
      [this.formFields.USERNAME]: ['', Validators.required],
      [this.formFields.EMAIL]: ['', [Validators.required, Validators.email]],
      [this.formFields.EMAIL_AGAIN]: [
        '',
        [
          Validators.required,
          Validators.email,
          matchFieldValidator(this.formFields.EMAIL, FormGroupErrorKeyConstants.EMAILMISMATCH),
        ],
      ],
      [this.formFields.PASSWORD]: ['', Validators.required],
      [this.formFields.PASSWORD_AGAIN]: [
        '',
        [
          Validators.required,
          matchFieldValidator(
            this.formFields.PASSWORD,
            FormGroupErrorKeyConstants.PASSWORDMISMATCH,
          ),
        ],
      ],
      [this.formFields.FIRST_NAME]: [null],
      [this.formFields.LAST_NAME]: [null],
    });
  }

  protected submit(): void {
    const dto = this.form.value as RegisterDto;
    this.userService.register(dto).subscribe();
  }
}
