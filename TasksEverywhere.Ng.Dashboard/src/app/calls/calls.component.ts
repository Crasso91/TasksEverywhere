import { LoadingService } from './../services/loading.service';
import { Job } from './../models/Job';
import { JobsService } from './../services/jobs.service';
import { AlertService } from './../services/alert.service';
import { CallsService } from './../services/calls.service';
import { Component, Inject, OnInit, HostListener } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { Call } from '../models/index';
import { Location } from '@angular/common';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';


@Component({
  selector: 'app-calls',
  templateUrl: './calls.component.html'
})
export class CallsComponent implements OnInit {
  calls: Observable<Call[]>;
  job: Job;
  private jobId: number;
  public context: string;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string, private route: ActivatedRoute,
      private callsService: CallsService, private alertService: AlertService,
      private location: Location, private jobsService: JobsService,
      private loadingService: LoadingService) {
          this.context = callsService.context;
       }

  ngOnInit() {
    this.jobId = +this.route.snapshot.params['jobId'];
    this.jobsService.jobs.subscribe(x => this.job = x.find(item => item.JobID === this.jobId));
    this.calls = this.callsService.calls;

    console.log(this.job);
    this.refresh();
  }
  goBack() {
    this.location.back();
  }

  refresh() {
    const datenow = new Date(Date.now());
    const datefilter = new Date(datenow.getFullYear(), datenow.getMonth() - 1 , datenow.getDay() - 3, 0, 0, 0);

    this.jobsService.getByFilter({ 'JobID' : this.jobId});
    this.callsService.getByFilter({ 'JobID' : this.jobId, 'EndedAt#Greater' : datefilter.toDateString() });
  }

}
