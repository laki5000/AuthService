import { Component, Input } from '@angular/core';
import { AbstractControl } from '@angular/forms';
import { FormErrorConstants } from '../../../core/constants/form-error.constant';
import { MatFormFieldModule } from '@angular/material/form-field';

@Component({
  selector: 'app-validation-error-display',
  standalone: true,
  imports: [MatFormFieldModule],
  templateUrl: './validation-error-display.component.html',
  styleUrl: './validation-error-display.component.scss',
})
export class ValidationErrorDisplayComponent {
  @Input() control!: AbstractControl | null;
  @Input() fieldName!: string;

  get errorMessage(): string | null {
    if (!this.control) return null;
    const errors = this.control.errors;
    if (errors && Object.keys(errors).length > 0) {
      for (const key of Object.keys(errors)) {
        const typedKey = key.toUpperCase() as keyof typeof FormErrorConstants;
        if (FormErrorConstants[typedKey]) return FormErrorConstants[typedKey];
      }

      return FormErrorConstants.DEFAULT;
    }

    return null;
  }
}
