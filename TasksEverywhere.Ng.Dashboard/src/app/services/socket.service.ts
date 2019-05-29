import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { webSocket } from 'rxjs/webSocket';
import { SocketMessage } from '../models/SocketMessage';


@Injectable()
export class SocketService {
  private socket;
  private subscriptions: PushSubscription[];

  public initSocket(): void {
      this.socket = webSocket('ws://185.58.192.19:9992');
  }

  public onMessage(): Observable<SocketMessage> {
    return new Observable<SocketMessage>(observer => {
      this.socket.subscribe((data: SocketMessage) => observer.next(data));
    });
  }
}
