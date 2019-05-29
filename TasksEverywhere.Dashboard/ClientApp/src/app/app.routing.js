"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var router_1 = require("@angular/router");
var home_component_1 = require("./home/home.component");
var index_1 = require("./login/index");
var servers_component_1 = require("./servers/servers.component");
var jobs_component_1 = require("./jobs/jobs.component");
var calls_component_1 = require("./calls/calls.component");
var appRoutes = [
    { path: '', component: home_component_1.HomeComponent, canActivate: [] },
    { path: 'login', component: index_1.LoginComponent },
    { path: 'servers', component: servers_component_1.ServersComponent },
    { path: 'jobs', component: jobs_component_1.JobsComponent },
    { path: 'jobs/:clientId', component: jobs_component_1.JobsComponent },
    { path: 'calls', component: calls_component_1.CallsComponent },
    { path: 'calls/:jobId', component: calls_component_1.CallsComponent },
    { path: 'jobs/:clientId/calls/:jobId', component: calls_component_1.CallsComponent, pathMatch: 'full' },
    // otherwise redirect to home
    { path: '**', redirectTo: '' }
];
exports.routing = router_1.RouterModule.forRoot(appRoutes);
//# sourceMappingURL=app.routing.js.map