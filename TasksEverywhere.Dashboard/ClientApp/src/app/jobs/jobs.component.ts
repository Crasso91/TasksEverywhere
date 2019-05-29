import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute, Params } from '@angular/router';

@Component({
  selector: 'app-jobs',
  templateUrl: './jobs.component.html'
})
export class JobsComponent {
  public jobs: Jobs[];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string, private route: ActivatedRoute) {
    const clientId = route.snapshot.params["clientId"];
    console.log(route,route.params,clientId);
    http.get<Jobs[]>(baseUrl + 'api/Jobs/List?filters={"ClientID":"' + clientId + '"}').subscribe(result => {
      this.jobs = result;
      console.log(result);
    }, error => console.error(error));
  }

}

interface Jobs {
  jobID: number,
  key: string,
  xecuting: boolean,
  startedAt: Date,
  isExecutingCorrectly: boolean,
  lastCall: Call
}

interface Call {
  callId: number,
  fireInstenceId: number,
  startedAt: Date,
  endedAt: Date,
  nextStart: Date 
}
