import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { UserService } from './services/apiCalls/user.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  constructor(
    private http: HttpClient,
    private userService: UserService
  ) { }

  fullName: string = '';

  ngOnInit() {
    this.userService.getUserProfile().subscribe({
      next: (res: any) => { this.fullName = res.fullName },
      error: (err: any) => console.log('error while retrieving user profile:\n', err)
    })
  }


  title = 'Whats Just Like';


}
