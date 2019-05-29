import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthenticationService } from './index';
import { Account, Server } from '../models/index';

@Injectable()
export class ServersService {
  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string, private authService: AuthenticationService) { }

  getAll() {
    let request = {
      sessionKey: this.authService.currentUserValue.sessionKey
    }
    return this.http.post<Server[]>(`${this.baseUrl}/servers/list`, request);
  }

  getById(id: number) {
    let request = {
      sessionKey: this.authService.currentUserValue.sessionKey,
      data: { "ClientID" : id}
    }
    return this.http.post(`${this.baseUrl}/servers/list`, request);
  }

  add(server: Server) {
    let request = {
      sessionKey : this.authService.currentUserValue.sessionKey,
      data: {
        client: server
      }
    }
    return this.http.post(`${this.baseUrl}/servers/add`, request);
  }

  update(server: Server) {
    let request = {
      sessionKey: this.authService.currentUserValue.sessionKey,
      data: {
        client: server
      }
    }
    return this.http.post(`${this.baseUrl}/servers/update`, request);
  }

  delete(id: number) {
    let request = {
      sessionKey: this.authService.currentUserValue.sessionKey,
      data: {
        accountId: id
      }
    }
    return this.http.post(`${this.baseUrl}/servers/delete`, request);
  }

  generateAppId(id: number) {
    let request = {
      sessionKey: this.authService.currentUserValue.sessionKey,
      data: { "ServerID": id}
    }
    return this.http.post(`${this.baseUrl}/servers/appId`, request);
  }

  generateAppToken(id: number) {
    let request = {
      sessionKey: this.authService.currentUserValue.sessionKey,
      data: { "ServerID": id }
    }
    return this.http.post(`${this.baseUrl}/servers/appToken`, request);
  }
}
