import { Directive, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { FormFieldConstants } from '../../../core/constants/form-field.constant';

@Directive({
  standalone: true,
})
export abstract class BaseFormComponent implements OnInit {
  form!: FormGroup;
  formFields = FormFieldConstants;

  constructor(protected fb: FormBuilder) {}

  ngOnInit(): void {
    this.initForm();
    this.localNgOnInit();
  }

  protected abstract initForm(): void;
  protected abstract submit(): void;
  protected abstract localNgOnInit(): void;
}
