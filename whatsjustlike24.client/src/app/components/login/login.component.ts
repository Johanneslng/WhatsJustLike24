import { Component } from '@angular/core';
import { AuthService } from '../../services/apiCalls/auth.service';
import { MatDialogRef } from '@angular/material/dialog';
import { Login } from '../../models/Login';
import { MatSnackBar } from '@angular/material/snack-bar';



@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {

  authenticated: string = "Not Authenticated";

  loginDTO: Login = {
    email: '',
    password: ''
  };

  errorMessage: string = '';
  constructor(
    private authService: AuthService,
    private dialogRef: MatDialogRef<LoginComponent>,
    private snackBar: MatSnackBar
  ) { }

  login(loginDTO: Login) {
    this.authService.login(loginDTO).subscribe({
      next: () => {
        this.dialogRef.close();
        this.snackBar.open('Login successful', 'Close', { duration: 3000 });
      },
      error: (error) => {
        this.errorMessage = 'Login failed: ' + (error.error?.message || error.message);
      }
    });
  }

  public checkAuth() {
    this.authService.getAuthCheck()
      .subscribe((response: string) => {
        this.authenticated = response;
      }, error => {
        console.error(error)
      });
  }
}
