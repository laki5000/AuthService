import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function matchFieldValidator(controlToMatchName: string, errorKey: string): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.parent) {
      return null;
    }

    const controlToMatch = control.parent.get(controlToMatchName);
    if (!controlToMatch) {
      return null;
    }

    if (!(control as any)._matchFieldSubscribed) {
      (control as any)._matchFieldSubscribed = true;

      controlToMatch.valueChanges.subscribe(() => {
        control.updateValueAndValidity({ onlySelf: true });
      });
    }

    return control.value === controlToMatch.value ? null : { [errorKey]: true };
  };
}
