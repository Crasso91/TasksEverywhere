import { CallsService } from './../services/calls.service';
import { JobsComponent } from './../jobs/jobs.component';
import { JobsService } from './../services/jobs.service';
import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ServersService, LoadingService } from '../services/index';
import { Server } from '../models/index';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-servers',
  templateUrl: './servers.component.html'
})
export class ServersComponent implements OnInit {
  public servers: Observable<Server[]>;
  private serverID: number;
  public context = 'servers';

  constructor(http: HttpClient, private serversService: ServersService,
    private jobsService: JobsService, private callsService: CallsService,
    private loadingService: LoadingService) {
      this.context = serversService.context;
     }

  ngOnInit() {
    this.servers = this.serversService.servers;
    this.serversService.getAll();
    this.callsService.empty();
  }

  refresh() {
    this.serversService.getAll();
    this.jobsService.getByFilter({ 'ServerID' : this.serverID });
    this.callsService.empty();
  }

  loadJobs(serverID: number) {
    this.serverID = serverID;
    this.callsService.empty();
    this.jobsService.getByFilter({ 'ServerID' : serverID});
  }
}
