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
    localStorage.removeItem('token');
  }

  isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }
}
