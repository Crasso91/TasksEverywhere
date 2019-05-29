import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { Call } from '../models/index';

@Component({
  selector: 'app-calls',
  templateUrl: './calls.component.html'
})
export class CallsComponent {
  public calls: Call[];
  public clientId: number;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string, private route: ActivatedRoute) {
    const jobId = route.snapshot.params["jobId"];
    let clientId = route.snapshot.params["clientId"];
    console.log(route, route.params, jobId);
    http.get<Call[]>(baseUrl + 'api/Calls/List?filters={"JobID":"' + jobId + '"}').subscribe(result => {
      this.calls = result;
      console.log(result);
    }, error => console.error(error));
  }

}
