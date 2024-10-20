import { Component } from '@angular/core';
import { AuthService } from '../../services/apiCalls/auth.service';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { Login } from '../../models/Login';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { RegisterComponent } from '../register/register.component';
import { ToastrService } from 'ngx-toastr';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {

  constructor(
    private dialog: MatDialog,
    private authService: AuthService,
    private dialogRef: MatDialogRef<LoginComponent>,
    private snackBar: MatSnackBar,
    private formBuilder: FormBuilder,
    private toastr: ToastrService
  ) { }

  isSubmitted: boolean = false;

  loginForm = this.formBuilder.group({
    email: ['', Validators.required],
    password: ['', Validators.required]
  })

  onSubmit(): void {
    this.isSubmitted = true;
    if (this.loginForm.valid) {
      this.authService.signIn(this.loginForm.value).subscribe({
        next: (res: any) => {
          localStorage.setItem('token', res.token);
          this.toastr.success('Success!', 'Logged in');
          this.dialogRef.close(this.loginForm.value);
        },
        error: err => {
          if (err.status == 400)
            this.toastr.error('Incorrect email or password', 'Login failed')
          else
            console.log('error during login:\n', err)
        }
      })
    }
  }

  hasDisplayableError(controlName: string): Boolean {
    const control = this.loginForm.get(controlName);
    return Boolean(control?.invalid) &&
      (this.isSubmitted || Boolean(control?.touched) || Boolean(control?.dirty))
  }


  onCancel(): void {
    this.dialogRef.close();
  }

  openRegister(): void {
    this.dialogRef.close();
    this.dialog.open(RegisterComponent);
  }
}
