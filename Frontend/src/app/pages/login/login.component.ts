import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { LoginDto } from '../../core/models/login.dto';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class Login {
  form: FormGroup<{
    username: FormControl<string>;
    password: FormControl<string>;
  }>;

  constructor(private fb: FormBuilder) {
    this.form = this.fb.group({
      username: new FormControl<string>('', { validators: Validators.required, nonNullable: true  }),
      password: new FormControl<string>('', { validators: Validators.required, nonNullable: true  }),
    });
  }

  submit() {
    const dto: LoginDto = this.form.value as LoginDto;

    
  }
}