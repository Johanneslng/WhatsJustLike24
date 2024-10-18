import { Component } from '@angular/core';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { LoginComponent } from '../login/login.component';
import { RegisterComponent } from '../register/register.component';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent {
  constructor(
    public dialog: MatDialog
  ) { }

  openLogin() {
    this.dialog.open(LoginComponent, {
      width: '50%'
    });
  }

  openRegister() {
    this.dialog.open(RegisterComponent, {
      width: '50%'
    });
  }
}
