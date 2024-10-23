import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { Login } from '../../models/Login';
import { TOKEN_KEY } from '../constants';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private API_URL = environment.API_URL;
  constructor(private http: HttpClient) { }


  signIn(formData:any) {
    return this.http.post( this.API_URL + `/auth/signin`, formData);
  }

  signUp(formData: any) {
    return this.http.post( this.API_URL + '/auth/signup', formData)
  }

  setToken(token: string) {
    localStorage.setItem(TOKEN_KEY, token);
  }

  isLoggedIn(): boolean {
    return this.getToken() != null ? true : false;
  }

  getToken() {
    return localStorage.getItem(TOKEN_KEY);
  }

  logout() {
    return localStorage.removeItem(TOKEN_KEY);
  }

  getAuthCheck(): Observable<string> {
    return this.http.get<string>(
      this.API_URL + `/auth/CheckAuth`
    );
  }
}
