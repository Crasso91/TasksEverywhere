import { Injectable, Output, EventEmitter } from '@angular/core';

@Injectable()
export class LoadingService {
  constructor() { }
  @Output() loadingStart: EventEmitter<any> = new EventEmitter();
  @Output() loadingEnd: EventEmitter<any> = new EventEmitter();
}
