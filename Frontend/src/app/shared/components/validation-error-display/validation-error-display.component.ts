import { Component, Input } from '@angular/core';
import { AbstractControl } from '@angular/forms';
import { FormErrorConstants } from '../../../core/constants/form-error.constant';

@Component({
  selector: 'app-validation-error-display',
  standalone: true,
  imports: [],
  templateUrl: './validation-error-display.component.html',
  styleUrl: './validation-error-display.component.scss',
})
export class ValidationErrorDisplayComponent {
  @Input() control!: AbstractControl | null;
  @Input() fieldName!: string;

  get errorMessage(): string | null {
    if (!this.control) return null;

    if (this.control.errors) {
      for (const key of Object.keys(this.control.errors)) {
        const typedKey = key.toUpperCase() as keyof typeof FormErrorConstants;
        if (FormErrorConstants[typedKey]) return FormErrorConstants[typedKey];
      }

      return FormErrorConstants.DEFAULT;
    }

    if (this.control.parent?.errors) {
      for (const [key, value] of Object.entries(this.control.parent.errors)) {
        if (value && typeof value === 'object') {
          if (value.displayField === this.fieldName) {
            const typedKey = key.toUpperCase() as keyof typeof FormErrorConstants;
            return FormErrorConstants[typedKey] ?? FormErrorConstants.DEFAULT;
          }
        }
      }
    }

    return null;
  }
}
