import { LoadingService } from './loading.service';
import { AuthenticationService } from './authentication.service';
import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import { Last20CallResponse, CallPerHourResponse } from '../models';
import { JobsService } from './jobs.service';

@Injectable()
export class StatsService {
  public context = 'StatsService';
  private _last20Call: BehaviorSubject<Last20CallResponse>;
  private _last20CallDataStore: {
    last20Call: Last20CallResponse;
  };
  private _callPerHour: BehaviorSubject<CallPerHourResponse>;
  private _callPerHourDataStore: {
    callPerHour: CallPerHourResponse;
  };
  private _errorPerHour: BehaviorSubject<CallPerHourResponse>;
  private _errorPerHourDataStore: {
    errorPerHour: CallPerHourResponse;
  };
  constructor(private http: HttpClient, @Inject('API_URL') private baseUrl: string, private authService: AuthenticationService,
              private loadingService: LoadingService, private jobsService: JobsService) {
    this._last20CallDataStore = { last20Call: <Last20CallResponse> {} };
    this._last20Call = <BehaviorSubject<Last20CallResponse>>new BehaviorSubject(<Last20CallResponse>{});
    this._callPerHourDataStore = { callPerHour: <CallPerHourResponse> {} };
    this._callPerHour  = <BehaviorSubject<CallPerHourResponse>>new BehaviorSubject(<CallPerHourResponse>{});
    this._errorPerHourDataStore = { errorPerHour: <CallPerHourResponse> {} };
    this._errorPerHour  = <BehaviorSubject<CallPerHourResponse>>new BehaviorSubject(<CallPerHourResponse>{});
   }

  get last20Call() {
    return this._last20Call.asObservable();
  }

  get callPerHour() {
    return this._callPerHour.asObservable();
  }

  get errorPerHour() {
    return this._errorPerHour.asObservable();
  }

  getLast20Calls() {
    this.loadingService.loadingStart.emit(this.context);
    const sessionKey = this.authService.currentUserValue() == null ? '' :  this.authService.currentUserValue().SessionKey;
    const accountID = this.authService.currentUserValue() == null ? '' : this.authService.currentUserValue().Account.AccountID;
    this.http.get<Last20CallResponse>(`${this.baseUrl}/stats/Last20Call?sessionKey=` + sessionKey + '&accountID=' + accountID )
      .subscribe(data => {
        console.log(data);
        this._last20CallDataStore.last20Call = data;
        this._last20Call.next(this._last20CallDataStore.last20Call);
        this.loadingService.loadingEnd.emit(this.context);
      }, error => console.log('Could not load Last20Calls.'));
  }

  getCallPerHour() {
    this.loadingService.loadingStart.emit(this.context);
    const sessionKey = this.authService.currentUserValue() == null ? '' :  this.authService.currentUserValue().SessionKey;
    const accountID = this.authService.currentUserValue() == null ? '' : this.authService.currentUserValue().Account.AccountID;
    this.http.get<CallPerHourResponse>(`${this.baseUrl}/stats/CallPerHour?sessionKey=` + sessionKey + '&accountID=' + accountID )
    .subscribe(data => {
      this._callPerHourDataStore.callPerHour = data;
      this._callPerHour.next(this._callPerHourDataStore.callPerHour);
      this.loadingService.loadingEnd.emit(this.context);
    }, error => console.log('Could not load CallPerHour.'));
  }

  getErrorPerHour() {
    this.loadingService.loadingStart.emit(this.context);
    const sessionKey = this.authService.currentUserValue() == null ? '' :  this.authService.currentUserValue().SessionKey;
    const accountID = this.authService.currentUserValue() == null ? '' : this.authService.currentUserValue().Account.AccountID;
    this.http.get<CallPerHourResponse>(`${this.baseUrl}/stats/ErrorPerHour?sessionKey=` + sessionKey + '&accountID=' + accountID )
    .subscribe(data => {
      this._errorPerHourDataStore.errorPerHour = data;
      this._errorPerHour.next(this._errorPerHourDataStore.errorPerHour);
      this.loadingService.loadingEnd.emit(this.context);
    }, error => console.log('Could not load ErrorPerHour.'));
  }
}

