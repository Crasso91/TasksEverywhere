import { MessagesComponent } from './messages/messages.component';
import { NgModule } from '@angular/core';
import { MDBBootstrapModule } from 'angular-bootstrap-md';
import { JobsService } from './services/jobs.service';
import { JwtInterceptor } from './helpers/jwt';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, NgModel } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { ServersComponent } from './servers/servers.component';
import { JobsComponent } from './jobs/jobs.component';
import { CallsComponent } from './calls/calls.component';
import { ServersService, AuthenticationService, AlertService, CallsService, SocketService } from './services';
import { AlertComponent } from './directives/index';
import { LoginComponent } from './login/index';
import { AuthGuard } from './guards/index';
import { routing } from './app.routing';

import { Location } from '@angular/common';
import { StatsService } from './services/stats.service';
import { LoadingService } from './services/loading.service';
import { LoadingComponent } from './loading/loading.component';


import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ToastrModule } from 'ngx-toastr';


@NgModule({
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    routing,
    BrowserAnimationsModule, // rquired animations module
    ToastrModule.forRoot(), // ToastrModule added
    MDBBootstrapModule.forRoot()
  ],
  providers: [
    LoadingService,
    ServersService,
    AuthGuard,
    AlertService,
    JobsService,
    AuthenticationService,
    CallsService,
    {
        provide: HTTP_INTERCEPTORS,
        useClass: JwtInterceptor,
        multi: true
    },
    Location,
    StatsService,
    SocketService
  ],
  declarations: [
    LoadingComponent,
    AppComponent,
    NavMenuComponent,
    AlertComponent,
    HomeComponent,
    LoginComponent,
    ServersComponent,
    JobsComponent,
    CallsComponent,
    MessagesComponent
  ],
  bootstrap: [
    AppComponent
  ]
})
export class AppModule { }
