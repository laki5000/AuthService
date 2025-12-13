import { Component } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserService } from '../../core/services/user.service';
import { RegisterDto } from '../../core/models/register.dto';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { BaseFormComponent } from '../../shared/components/base/base-form.component';
import { RouteConstants } from '../../core/constants/route.constant';
import { matchFieldsValidator } from '../../shared/validators/match-fields.validator';
import { FormFieldConstants } from '../../core/constants/form-field.constant';
import { FormGroupErrorKeyConstants } from '../../core/constants/form-error-key.constant';
import { ValidationErrorDisplayComponent } from '../../shared/components/validation-error-display/validation-error-display.component';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, ValidationErrorDisplayComponent],
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
    this.form = this.fb.group(
      {
        [this.formFields.USERNAME]: ['', Validators.required],
        [this.formFields.EMAIL]: ['', [Validators.required, Validators.email]],
        [this.formFields.EMAIL_AGAIN]: ['', [Validators.required, Validators.email]],
        [this.formFields.PASSWORD]: ['', Validators.required],
        [this.formFields.PASSWORD_AGAIN]: ['', Validators.required],
        [this.formFields.FIRST_NAME]: [null],
        [this.formFields.LAST_NAME]: [null],
      },
      {
        validators: [
          matchFieldsValidator(
            FormFieldConstants.EMAIL,
            FormFieldConstants.EMAIL_AGAIN,
            FormGroupErrorKeyConstants.EMAILMISMATCH,
            FormFieldConstants.EMAIL_AGAIN,
          ),
          matchFieldsValidator(
            FormFieldConstants.PASSWORD,
            FormFieldConstants.PASSWORD_AGAIN,
            FormGroupErrorKeyConstants.PASSWORDMISMATCH,
            FormFieldConstants.PASSWORD_AGAIN,
          ),
        ],
      },
    );
  }

  protected submit(): void {
    const dto = this.form.value as RegisterDto;
    this.userService.register(dto).subscribe();
  }
}
