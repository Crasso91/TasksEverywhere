import { LoadingService } from './loading.service';
import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthenticationService } from './authentication.service';
import { Job } from '../models/index';
import { BehaviorSubject } from 'rxjs';

@Injectable()
export class JobsService {
  public context = 'JobsService';
  private _jobs: BehaviorSubject<Job[]>;
  private dataStore: {
    jobs: Job[];
  };

  constructor(private http: HttpClient, @Inject('API_URL') private baseUrl: string, private authService: AuthenticationService,
              private loadingService: LoadingService) {
    this.dataStore = { jobs: [] };
    this._jobs = <BehaviorSubject<Job[]>>new BehaviorSubject([]);
   }

   get jobs() {
    return this._jobs.asObservable();
  }

  getAll() {
    this.loadingService.loadingStart.emit(this.context);
    const request = {
      sessionKey: this.authService.currentUserValue().SessionKey,
      accountID: this.authService.currentUserValue().Account.AccountID
    };
    this.http.post<Job[]>(`${this.baseUrl}/jobs/list`, request).subscribe(data => {
      console.log(data);
      this.dataStore.jobs = data;
      this._jobs.next(this.dataStore.jobs);
      this.loadingService.loadingEnd.emit(this.context);
    }, error => console.log('Could not load jobs.'));
  }

  getByFilter(filter: object) {
    this.loadingService.loadingStart.emit(this.context);
    const request = {
      sessionKey: this.authService.currentUserValue().SessionKey,
      accountID: this.authService.currentUserValue().Account.AccountID,
      data: filter
    };
    this.http.post<Job[]>(`${this.baseUrl}/jobs/list`, request).subscribe(data => {
      console.log(data);
      this.dataStore.jobs = data;
      this._jobs.next(this.dataStore.jobs);
      this.loadingService.loadingEnd.emit(this.context);
    }, error => console.log('Could not load jobs.'));
  }
}
