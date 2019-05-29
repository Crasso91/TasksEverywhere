import { CallsService } from './../services/calls.service';
import { AlertService } from './../services/alert.service';
import { JobsService } from './../services/jobs.service';
import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { Job } from '../models/index';
import { Location } from '@angular/common';
import { first, map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { CallsComponent } from '../calls/calls.component';
import { LoadingService } from '../services';

@Component({
  selector: 'app-jobs',
  templateUrl: './jobs.component.html'
})
export class JobsComponent implements OnInit {
  public jobs: Observable<Job[]>;
  public loading = true;
  private serverID = 0;
  public context = 'jobs';

  constructor(http: HttpClient, @Inject('API_URL') baseUrl: string, private route: ActivatedRoute,
      private jobsService: JobsService, private location: Location,
      private callsService: CallsService, private loadingService: LoadingService) {
        this.context = jobsService.context;
       }

  ngOnInit() {
    this.jobs = this.jobsService.jobs;
    this.serverID  = this.route.snapshot.params['serverId'] ;
    this.refresh();
  }
  goBack() {
    this.location.back();
  }

  refresh() {
    this.jobsService.getByFilter({ 'ServerID' : this.serverID});
  }

  loadCalls(jobId: number) {
    this.callsService.getByFilter({ 'JobID' : jobId });
  }
}
