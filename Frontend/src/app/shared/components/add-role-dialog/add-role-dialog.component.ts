import { Component, EventEmitter, Output } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { BaseFormComponent } from '../base/base-form.component';
import { ValidationErrorDisplayComponent } from '../validation-error-display/validation-error-display.component';
import { RoleService } from '../../../core/services/role.service';
import { ResultDto } from '../../../core/models/result.dto';
import { RoleDto } from '../../../core/models/role.dto';

@Component({
  selector: 'app-add-role-dialog',
  standalone: true,
  imports: [
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    ReactiveFormsModule,
    ValidationErrorDisplayComponent,
  ],
  templateUrl: './add-role-dialog.component.html',
  styleUrl: './add-role-dialog.component.scss',
})
export class AddRoleDialogComponent extends BaseFormComponent {
  constructor(
    fb: FormBuilder,
    private roleService: RoleService,
    private dialogRef: MatDialogRef<AddRoleDialogComponent>,
  ) {
    super(fb);
  }

  protected override initForm(): void {
    this.form = this.fb.group({
      [this.formFields.ROLE_NAME]: ['', Validators.required],
    });
  }

  protected override submit(): void {
    const dto = this.form.value as RoleDto;
    this.roleService.create(dto).subscribe({
      next: (result: ResultDto<string>) => {
        const roleName = result.Result;
        if (roleName) {
          this.dialogRef.close(roleName);
        }
      },
    });
  }
}
