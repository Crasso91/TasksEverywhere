import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { ServersComponent } from './servers/servers.component';
import { JobsComponent } from './jobs/jobs.component';
import { CallsComponent } from './calls/calls.component';
import { ServersService, AuthenticationService, AlertService } from './services/index';
import { AlertComponent } from './directives/index';
import { LoginComponent } from './login/index';
import { AuthGuard } from './guards/index';
import { routing } from './app.routing'

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    AlertComponent,
    HomeComponent,
    LoginComponent,
    ServersComponent,
    JobsComponent,
    CallsComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    routing
  ],
  providers: [
    ServersService,
    AlertService,
    AuthenticationService],
  bootstrap: [
    AppComponent]
})
export class AppModule { }
