import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { Login } from '../../models/Login';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private API_URL = environment.API_URL;
  constructor(private http: HttpClient) { }


  login(loginData: Login): Observable<any> {
    const body = {
      email: loginData.email,
      password: loginData.password
    };
    return this.http.post<any>(
      this.API_URL + `/login?useCookies=true`,
      body
    );
  }

  signup(formData: any) {
    return this.http.post(this.API_URL+'/auth/signup', formData)
  }

  getAuthCheck(): Observable<string> {
    return this.http.get<string>(
      this.API_URL + `/WeatherForecast/testAuth`,
      { withCredentials: true }
    );
  }

}
