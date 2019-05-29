import { LoadingService } from './loading.service';
import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthenticationService } from './authentication.service';
import { Server } from '../models/index';
import { BehaviorSubject } from 'rxjs';

@Injectable()
export class ServersService {
  public context = 'ServersService';
  private _servers: BehaviorSubject<Server[]>;
  private dataStore: {
    servers: Server[];
  };
  constructor(private http: HttpClient, @Inject('API_URL') private baseUrl: string, private authService: AuthenticationService,
              private loadingService: LoadingService) {
    this.dataStore = { servers: [] };
    this._servers = <BehaviorSubject<Server[]>>new BehaviorSubject([]);
   }

   get servers() {
    return this._servers.asObservable();
  }

  getAll() {
    this.loadingService.loadingStart.emit(this.context);
    const request = {
      sessionKey: this.authService.currentUserValue().SessionKey,
      accountID: this.authService.currentUserValue().Account.AccountID
    };
    this.http.post<Server[]>(`${this.baseUrl}/servers/list`, request).subscribe(data => {
      this.dataStore.servers = data;
      this.loadingService.loadingEnd.emit(this.context);
      this._servers.next(this.dataStore.servers);
    }, error => console.log('Could not load servers.'));
  }

  getByFilter(filter: object) {
    this.loadingService.loadingStart.emit(this.context);
    const request = {
      sessionKey: this.authService.currentUserValue().SessionKey,
      accountID: this.authService.currentUserValue().Account.AccountID,
      data: filter
    };
    this.http.post<Server[]>(`${this.baseUrl}/servers/list`, request).subscribe(data => {
      this.dataStore.servers = data;
      this._servers.next(this.dataStore.servers);
      this.loadingService.loadingEnd.emit(this.context);
    }, error => console.log('Could not load servers.'));
  }
}
