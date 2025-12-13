import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function matchFieldsValidator(
  field: string,
  fieldAgain: string,
  errorKey: string
): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const first = control.get(field)?.value;
    const second = control.get(fieldAgain)?.value;

    if (!first || !second) {
      return null;
    }

    return first === second
      ? null
      : { [errorKey]: true };
  };
}