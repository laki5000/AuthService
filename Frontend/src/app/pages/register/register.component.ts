import { Component, } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { RegisterDto } from '../../core/models/register.dto';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { BaseFormComponent } from '../../shared/components/base/base-form.component';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent extends BaseFormComponent {
  constructor(
    fb: FormBuilder,
    private authService: AuthService,
  ) {
    super(fb);
  }

  protected initForm(): void {
    this.form = this.fb.group({
      username: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      emailAgain: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
      passwordAgain: ['', Validators.required],
      firstName: [null],
      lastName: [null],
    });
  }

  protected submit(): void {
    const dto = this.form.value as RegisterDto;
    this.authService.register(dto).subscribe();
  }
}