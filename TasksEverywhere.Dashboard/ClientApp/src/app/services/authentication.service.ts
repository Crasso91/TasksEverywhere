import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { Account, LoginInfo } from '../models/index';

@Injectable()
export class AuthenticationService {
  private currentUserSubject: BehaviorSubject<LoginInfo>;
  public currentUser: Observable<LoginInfo>;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    this.currentUserSubject = new BehaviorSubject<LoginInfo>(JSON.parse(localStorage.getItem('currentUser')));
    this.currentUser = this.currentUserSubject.asObservable();
  }

  public get currentUserValue(): LoginInfo {
    return this.currentUserSubject.value;
  }

  login(username: string, password: string) {
    let account = {
      username: username,
      password: password
    }
    console.log(account, this.baseUrl)
    console.log(account, this.baseUrl + 'api/Authentication/Login')
    this.http.post<LoginInfo>(this.baseUrl + 'api/Authentication/Login', account).subscribe(loginInfo => {
      console.log(loginInfo);
      if (loginInfo.status == 'OK')
        localStorage.setItem('currentUser', JSON.stringify(loginInfo));
      this.currentUserSubject.next(loginInfo);
    }, error => console.error(error));
    return this.currentUser;
  }

  logout() {
    this.http.post<boolean>(this.baseUrl + 'api/Authentication/Logout', this.currentUserValue).subscribe(isLoggedOut => {
      if (isLoggedOut) {
        localStorage.removeItem('currentUser');
        this.currentUserSubject.next(null);
      }
    });
  }
}
