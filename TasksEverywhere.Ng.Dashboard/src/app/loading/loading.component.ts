import { LoadingService } from './../services/loading.service';
import { Component, Inject, OnInit, Input } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute, Params } from '@angular/router';

@Component({
  selector: 'app-loading',
  templateUrl: './loading.component.html'
})
export class LoadingComponent implements OnInit {
  loading = true;
  @Input() public context: string;
  constructor(http: HttpClient, @Inject('API_URL') baseUrl: string, private route: ActivatedRoute,
      private loadingService: LoadingService) { }

  ngOnInit() {
    this.loadingService.loadingStart.subscribe(x => { console.log('Start', x); if (this.context === x) { this.loading = true; } });
    this.loadingService.loadingEnd.subscribe(x => { console.log('End', x); if (this.context === x) { this.loading = false; }});
  }
}
