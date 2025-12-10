import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { RegisterDto } from '../../core/models/register.dto';
import { ResultDto } from '../../core/models/result.dto';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class Register implements OnInit {
  form!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
  ) {}

  ngOnInit(): void {
    this.initForm();
  }

  private initForm(): void {
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

  submit(): void {
    const dto: RegisterDto = this.form.value as RegisterDto;

    this.authService.register(dto).subscribe({
      next: (response: ResultDto<string>) => {},
    });
  }
}
