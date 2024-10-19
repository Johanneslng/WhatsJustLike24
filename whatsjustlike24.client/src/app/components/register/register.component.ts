import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogRef } from '@angular/material/dialog';
import { AuthService } from '../../services/apiCalls/auth.service';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  
  constructor(
    private dialogRef: MatDialogRef<RegisterComponent>,
    private fb: FormBuilder,
    private toastr: ToastrService,
    private authService: AuthService,

  ) { }
  isSubmitted: boolean = false;

  passwordMatchValidator: ValidatorFn = (control: AbstractControl): null => {
    const password = control.get('password')
    const confirmPassword = control.get('confirmPassword')

    if (password && confirmPassword && password.value != confirmPassword.value)
      confirmPassword?.setErrors({ passwordMismatch: true })
    else
      confirmPassword?.setErrors(null)

    return null;
  }

  registrationForm = this.fb.group({
    fullName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [
      Validators.required,
      Validators.minLength(10),
      Validators.pattern(/(?=.*[^a-zA-Z0-9 ])/)]],
    confirmPassword: [''],
  }, { validators: this.passwordMatchValidator })

  onSubmit() {
    this.isSubmitted = true;
    if (this.registrationForm.valid) {
      this.authService.signup(this.registrationForm.value)
        .subscribe({
          next: (res: any) => {
            if (res.succeeded) {
              this.registrationForm.reset();
              this.isSubmitted = false;
              this.toastr.success('New user created!', 'Registration Successful');
              this.dialogRef.close(this.registrationForm.value);
            }
          },
          error: err => {
            if (err.error?.errors) {
              // Handle specific error codes
              err.error.errors.forEach((x: any) => {
                switch (x.code) {
                  case "DuplicateUserName":
                    this.toastr.error('Username is already taken.', 'Registration Failed');
                    break;

                  case "DuplicateEmail":
                    this.toastr.error('Email is already taken.', 'Registration Failed');
                    break;

                  default:
                    this.toastr.error('An unknown error occurred.', 'Registration Failed');
                    console.log(x); // Log for developer debugging
                    break;
                }
              });
            } else if (err.error?.message) {
              // Handle general error messages
              this.toastr.error(err.error.message, 'Registration Failed');
            } else {
              // Fallback for unexpected errors
              this.toastr.error('An unexpected error occurred.', 'Registration Failed');
              console.log('Unexpected error:', err);
            }
          }
        });
    }
  }

  hasDisplayableError(controlName: string): Boolean {
    const control = this.registrationForm.get(controlName);
    return Boolean(control?.invalid) &&
      (this.isSubmitted || Boolean(control?.touched) || Boolean(control?.dirty))
  }
  onCancel(): void {
    this.dialogRef.close();
  }
}
