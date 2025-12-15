import { Component, Inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { BaseFormComponent } from '../base/base-form.component';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserService } from '../../../core/services/user.service';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { ValidationErrorDisplayComponent } from '../validation-error-display/validation-error-display.component';
import { UpdateUserRoleDto } from '../../../core/models/update-user-role.dto';

@Component({
  selector: 'app-update-user-role-dialog.component',
  standalone: true,
  imports: [
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    ReactiveFormsModule,
    ValidationErrorDisplayComponent,
    MatSelectModule,
    MatSlideToggleModule
  ],
  templateUrl: './update-user-role-dialog.component.html',
  styleUrl: './update-user-role-dialog.component.scss',
})
export class UpdateUserRoleDialogComponent extends BaseFormComponent {
  roles: string[] = [];

  constructor(
    fb: FormBuilder,
    private userService: UserService,
    private dialogRef: MatDialogRef<UpdateUserRoleDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { roles: string[] },
  ) {
    super(fb);
  }

  protected override initForm(): void {
    this.form = this.fb.group({
      [this.formFields.USERNAME]: ['', Validators.required],
      [this.formFields.ROLE_NAME]: ['', Validators.required],
      [this.formFields.ADD]: [false],
    });
  }

  localNgOnInit(): void {
    if (this.data?.roles) {
      this.roles = this.data.roles;
    }
  }

  protected override submit(): void {
    const dto = this.form.value as UpdateUserRoleDto;
  }
}
