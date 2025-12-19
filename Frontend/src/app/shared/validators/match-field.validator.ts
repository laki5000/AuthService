import { AbstractControl, FormGroup, ValidationErrors, ValidatorFn } from '@angular/forms';

export function matchFieldsValidator(controlName: string, matchingControlName: string, errorKey: string): ValidatorFn {
  return (group: AbstractControl): ValidationErrors | null => {
    const formGroup = group as FormGroup;
    const control = formGroup.controls[controlName];
    const matchingControl = formGroup.controls[matchingControlName];

    if (!control || !matchingControl) {
      return null;
    }

    const controls = [control, matchingControl];

    if (control.value !== matchingControl.value) {
      controls.forEach(ctrl => {
        const errors = ctrl.errors || {};
        ctrl.setErrors({ ...errors, [errorKey]: true });
      });
    } else {
      controls.forEach(ctrl => {
        if (ctrl.errors) {
          const { [errorKey]: removed, ...otherErrors } = ctrl.errors;
          ctrl.setErrors(Object.keys(otherErrors).length ? otherErrors : null);
        }
      });
    }

    return null;
  };
}