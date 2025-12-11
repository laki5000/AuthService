import { Directive, OnInit } from "@angular/core";
import { FormBuilder, FormGroup } from "@angular/forms";

@Directive({
  standalone: true
})
export abstract class BaseFormComponent implements OnInit {
  form!: FormGroup;

  constructor(protected fb: FormBuilder) {}

  ngOnInit(): void {
    this.initForm();
  }

  protected abstract initForm(): void;
  protected abstract submit(): void;
}