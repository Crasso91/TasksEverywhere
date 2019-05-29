import { LoadingService } from './../services/loading.service';
import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from '../services';
import { first } from 'rxjs/operators';
import { ActivatedRouteSnapshot, Router } from '@angular/router';
import { Location } from '@angular/common';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit {
  isExpanded = false;
  isAuthenticated = false;
  username = '';
  constructor(
    private authenticationService: AuthenticationService,
    private location: Location,
    private router: Router,
    private loadingService: LoadingService) {
      this.authenticationService.loginEvent.subscribe(x => this.ngOnInit());
      this.authenticationService.logoutEvent.subscribe(x => this.logOut());
    }

  ngOnInit() {
    const currentUser = this.authenticationService.currentUserValue();
    if (currentUser != null) {
      this.isAuthenticated = true;
      this.username = this.authenticationService.currentUserValue().Account.Username;
    } else {
      this.isAuthenticated = false;
    }
    this.loadingService.loadingEnd.emit();
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  logOut() {
    this.authenticationService.logout().pipe(first()).subscribe(x => {
      this.router.navigate(['/login'], { queryParams: { returnUrl: this.router.url }});
      this.ngOnInit();
    });
  }
}
