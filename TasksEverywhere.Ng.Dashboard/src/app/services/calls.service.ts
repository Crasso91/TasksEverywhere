import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthenticationService } from './authentication.service';
import { Call } from '../models/index';
import { BehaviorSubject } from 'rxjs';
import { LoadingService } from './loading.service';

@Injectable()
export class CallsService {
  public context = 'CallsService';
  private _calls: BehaviorSubject<Call[]>;
  private dataStore: {
    calls: Call[];
  };
  get calls() {
    return this._calls.asObservable();
  }

  constructor(private http: HttpClient, @Inject('API_URL') private baseUrl: string, private authService: AuthenticationService,
              private loadingService: LoadingService) {
    this.dataStore = { calls: [] };
    this._calls = <BehaviorSubject<Call[]>>new BehaviorSubject([]);
   }

  getAll() {
    this.loadingService.loadingStart.emit(this.context);
    const request = {
      sessionKey: this.authService.currentUserValue().SessionKey,
      accountID: this.authService.currentUserValue().Account.AccountID
    };
    this.http.post<Call[]>(`${this.baseUrl}/calls/list`, request).subscribe(data => {
      this.dataStore.calls = data;
      this._calls.next(this.dataStore.calls);
      this.loadingService.loadingEnd.emit(this.context);
    }, error => console.log('Could not load jobs.'));
  }

  getByFilter(filter: object) {
    console.log(filter)
    this.loadingService.loadingStart.emit(this.context);
    const request = {
      sessionKey: this.authService.currentUserValue().SessionKey,
      accountID: this.authService.currentUserValue().Account.AccountID,
      data: filter
    };
    this.http.post<Call[]>(`${this.baseUrl}/calls/list`, request).subscribe(data => {
      this.dataStore.calls = data;
      this._calls.next(this.dataStore.calls);
      this.loadingService.loadingEnd.emit(this.context);
    }, error => console.log('Could not load jobs.'));
  }

  empty() {
    this.loadingService.loadingStart.emit(this.context);
    this._calls.next(null);
    this.loadingService.loadingEnd.emit(this.context);
  }
}
