import { ServersService, JobsService, CallsService, StatsService, LoadingService } from './../services/index';
import { Component, OnInit } from '@angular/core';
import { Last20CallResponse, Server, Call, Job } from '../models/index';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {
  public servers: Observable<Server[]>;
  public jobs: Observable<Job[]>;
  public calls: Observable<Call[]>;
  public last20Calls: Observable<Last20CallResponse>;
  public chartPerHour: Array<any> = [];
  public chartType = 'line';
  public chartOptions: any = {
    responsive: true
  };
  public serversContext: string;
  public jobsContext: string;
  public callsContext: string;
  public statsContext: string;

  constructor(private statsService: StatsService, private serversService: ServersService,
    private jobsService: JobsService, private callsService: CallsService, private loadingService: LoadingService) {
      this.serversContext = serversService.context;
      this.jobsContext = jobsService.context;
      this.callsContext = callsService.context;
      this.statsContext = statsService.context;
    }

  ngOnInit(): void {
    this.servers = this.serversService.servers;
    this.jobs = this.jobsService.jobs;
    this.calls = this.callsService.calls;
    this.last20Calls = this.statsService.last20Call;
    this.statsService.callPerHour.subscribe(x => {
        if (x !== undefined) {
          this.chartPerHour.push({ data: x.data, label: 'Esecuzioni Per Ora'}) ;
        }
    });
    this.statsService.errorPerHour.subscribe(x => {
      if (x !== undefined) {
        this.chartPerHour.push({ data: x.data, label: 'Errori Per Ora'});
      }
    });

    this.serversService.getAll();
    this.jobsService.getAll();
    this.callsService.getAll();
    this.statsService.getLast20Calls();

    console.log(this.last20Calls);
    console.log(this.chartPerHour);
  }
}
