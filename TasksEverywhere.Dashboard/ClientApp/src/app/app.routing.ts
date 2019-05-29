import { Routes, RouterModule } from '@angular/router';

import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/index';
import { AuthGuard } from './guards/index';
import { ServersComponent } from './servers/servers.component'
import { JobsComponent } from './jobs/jobs.component';
import { CallsComponent } from './calls/calls.component';

const appRoutes: Routes = [
    { path: '', component: HomeComponent, canActivate: [] },
    { path: 'login', component: LoginComponent },
    { path: 'servers', component: ServersComponent },
    { path: 'jobs', component: JobsComponent },
    { path: 'jobs/:clientId', component: JobsComponent },
    { path: 'calls', component: CallsComponent },
    { path: 'calls/:jobId', component: CallsComponent },
    { path: 'jobs/:clientId/calls/:jobId', component: CallsComponent, pathMatch: 'full' },

    // otherwise redirect to home
    { path: '**', redirectTo: '' }
];

export const routing = RouterModule.forRoot(appRoutes);
