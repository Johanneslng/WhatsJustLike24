import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { AuthService } from './auth.service';


@Injectable({
  providedIn: 'root'
})
export class UserService {

  private API_URL = environment.API_URL;

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) { }

  getUserProfile() {
    const reqHeader = new HttpHeaders({'Authorization': 'Bearer ' + this.authService.getToken()})
    return this.http.get(this.API_URL + '/UserProfile', {headers: reqHeader})
  }

  isLoggedIn() {
    const reqHeader = new HttpHeaders({ 'Authorization': 'Bearer ' + this.authService.getToken() })
    return this.http.get(this.API_URL + '/CheckAuth', { headers: reqHeader })
  }
}
