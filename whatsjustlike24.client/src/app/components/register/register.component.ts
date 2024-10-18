import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogRef } from '@angular/material/dialog';
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
    private toastr: ToastrService

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
      Validators.minLength(6),
      Validators.pattern(/(?=.*[^a-zA-Z0-9 ])/)]],
    confirmPassword: [''],
  }, { validators: this.passwordMatchValidator })

  onSubmit(): void {
    this.isSubmitted = true;
    if (this.registrationForm.valid) {
      console.log('User registered:', this.registrationForm.value);
      this.dialogRef.close(this.registrationForm.value);
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  password1MatchValidator(form: FormGroup) {
    return form.get('password')?.value === form.get('confirmPassword')?.value
      ? null
      : { mismatch: true };
  }
}
