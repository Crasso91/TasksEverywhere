import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ServersService } from '../services/index';
import { Server } from '../models/index'
@Component({
  selector: 'app-servers',
  templateUrl: './servers.component.html'
})
export class ServersComponent {
  public clients: Server[];

  constructor(http: HttpClient, private serversService: ServersService) {
    serversService.getAll().subscribe(result => {
      this.clients = result;
      console.log(result);
    }, error => console.error(error)); 
  }
}
