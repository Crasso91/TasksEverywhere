import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthenticationService } from './authentication.service';
import { Account } from '../models/index';

@Injectable()
export class AccountService {
  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string, private authService: AuthenticationService) { }

  getAll() {
    return this.http.post<Account[]>(`${this.baseUrl}/accounts`, this.authService.currentUserValue);
  }

  getById(id: number) {
    return this.http.post(`${this.baseUrl}/accounts/${id}`, this.authService.currentUserValue);
  }

  update(account: Account) {
    const request = {
      sessionKey: this.authService.currentUserValue().SessionKey,
      data: {
        account: account
      }
    };
    return this.http.post(`${this.baseUrl}/accounts/update`, request);
  }

  delete(id: number) {
    const request = {
      sessionKey: this.authService.currentUserValue().SessionKey,
      data: {
        accountId: id
      }
    };
    return this.http.post(`${this.baseUrl}/accounts/delete`, request);
  }
}
