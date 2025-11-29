import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { TOKEN_KEY } from '../constants';
import { environment } from '../../../enviroments/enviroment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  constructor(private http: HttpClient) { }

  signin(formData: any) {
    return this.http.post(environment.apiBaseUrl + '/identity/signin', formData);
  }

  isLoggedIn() {
    return this.getToken() != null ? true : false;
  }

  saveToken(token: string) {
    localStorage.setItem(TOKEN_KEY, token)
  }

  getToken() {
    return localStorage.getItem(TOKEN_KEY);
  }

  deleteToken() {
    localStorage.removeItem(TOKEN_KEY);
  }

  getClaims(){
   return JSON.parse(window.atob(this.getToken()!.split('.')[1]))
  }

  getUserId(): string | null {
    debugger;
    if (!this.isLoggedIn()) {
      return null;
    }
    const claims = this.getClaims();
    return claims.userID;
  }

  isAdmin(): boolean {
    if (!this.isLoggedIn()) {
      return false;
    }
    const claims = this.getClaims();
    if (claims.role) {
      return claims.role === 'Admin';
    }
    return false;
  }

}
