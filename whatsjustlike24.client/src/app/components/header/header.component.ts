import { Component } from '@angular/core';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { LoginComponent } from '../login/login.component';
import { RegisterComponent } from '../register/register.component';
import { AuthService } from '../../services/apiCalls/auth.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent {
  constructor(
    public dialog: MatDialog,
    private authService: AuthService

  ) { }

  public auth: string = ''; 

  openLogin() {
    this.dialog.open(LoginComponent, {
      width: '30%'
    });
  }

  openRegister() {
    this.dialog.open(RegisterComponent, {
      width: '30%'
    });
  }

  logout() {
    this.authService.logout();
  }

  isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }

  checkAuth() {
    this.authService.getAuthCheck().subscribe({
      next: (response: string) => {
        this.auth = response;
        console.log('Auth string:', this.auth);  // Use the authString here
      },
      error: (error) => {
        console.error('Error occurred:', error);
      },
    });
  }
}
