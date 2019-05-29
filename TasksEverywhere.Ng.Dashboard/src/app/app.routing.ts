import { Routes, RouterModule } from '@angular/router';

import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/index';
import { AuthGuard } from './guards/index';
import { ServersComponent } from './servers/servers.component';
import { JobsComponent } from './jobs/jobs.component';
import { CallsComponent } from './calls/calls.component';

const appRoutes: Routes = [
    { path: '', component: HomeComponent, canActivate: [AuthGuard] },
    { path: 'login', component: LoginComponent },
    { path: 'servers', component: ServersComponent, canActivate: [AuthGuard] },
    { path: 'jobs', component: JobsComponent, canActivate: [AuthGuard] },
    { path: 'jobs/:serverId', component: JobsComponent, canActivate: [AuthGuard] },
    { path: 'calls', component: CallsComponent, canActivate: [AuthGuard] },
    { path: 'calls/:jobId', component: CallsComponent, canActivate: [AuthGuard] },

    // otherwise redirect to home
     { path: '**', redirectTo: '' }
];

export const routing = RouterModule.forRoot(appRoutes);
