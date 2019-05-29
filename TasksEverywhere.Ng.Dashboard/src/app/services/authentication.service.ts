import { map } from 'rxjs/operators';
import { Injectable, Inject, EventEmitter, Output } from '@angular/core';
import { HttpClient } from '@angular/common/http';
// tslint:disable-next-line:import-blacklist
import { BehaviorSubject, Observable } from 'rxjs';


import { LoginInfo } from '../models/index';

@Injectable()
export class AuthenticationService {
  @Output() loginEvent: EventEmitter<any> = new EventEmitter();
  @Output() logoutEvent: EventEmitter<any> = new EventEmitter();
  private currentUserSubject: BehaviorSubject<LoginInfo>;
  public currentUser: Observable<LoginInfo>;

  constructor(private http: HttpClient, @Inject('API_URL') private baseUrl: string) {
    this.currentUserSubject = new BehaviorSubject<LoginInfo>(JSON.parse(localStorage.getItem('currentUser')));
    this.currentUser = this.currentUserSubject.asObservable();
  }

  public currentUserValue(): LoginInfo {
    return this.currentUserSubject.value;
  }

  login(username: string, password: string) {
    const account = {
      username: username,
      password: password
    };
    console.log(account, this.baseUrl + '/Authentication/Login');
    return this.http.post<any>(this.baseUrl + '/Authentication/Login', account).pipe(map(response => {
      console.log(response, response.operationResult.Code === 0);
      if (response.operationResult.Code === 0) {
        localStorage.setItem('currentUser', JSON.stringify(response.data));
        this.currentUserSubject.next(response.data);
        this.loginEvent.emit();
      } else {
        console.log('shit');
      }
      return this.currentUserValue();
    }, error => console.error(error)));
  }

  logout() {
      const request = {
        sessionKey: this.currentUserValue() == null ? '' :  this.currentUserValue().SessionKey,
        accountID:  this.currentUserValue() == null ? '' : this.currentUserValue().Account.AccountID
      };
      return this.http.post<any>(this.baseUrl + '/Authentication/Logout', request).pipe(map(response => {
      if (response.data) {
        localStorage.removeItem('currentUser');
        this.currentUserSubject.next(null);
      }
      return this.currentUser;
    }));
  }

  isValid() {
    const sessionKey = this.currentUserValue() == null ? '' :  this.currentUserValue().SessionKey;
    const accountID = this.currentUserValue() == null ? '' : this.currentUserValue().Account.AccountID;
    console.log('/Authentication/IsValid?sessionKey=' + sessionKey + '&accountId=' + accountID);
    return this.http.get<boolean>(this.baseUrl + '/Authentication/IsValid?sessionKey=' + sessionKey + '&accountId=' + accountID)
    .pipe(map(response => {
      if (!response) {
        localStorage.removeItem('currentUser');
        this.currentUserSubject.next(null);
        this.logoutEvent.emit();
      }
      return response;
    }));
  }
}
