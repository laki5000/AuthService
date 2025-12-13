import { Component } from '@angular/core';
import { UserService } from '../../../core/services/user.service';
import { Router } from '@angular/router';
import { RouteConstants } from '../../../core/constants/route.constant';
import { Observable } from 'rxjs';
import { AsyncPipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [AsyncPipe, MatButtonModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss',
})
export class NavbarComponent {
  loggedIn$: Observable<boolean>;

  constructor(
    private userService: UserService,
    private router: Router,
  ) {
    this.loggedIn$ = this.userService.loggedIn$;
  }

  logout(): void {
    this.userService.logout().subscribe({
      next: () => {
        this.router.navigate([RouteConstants.LOGIN_PATH]);
      },
    });
  }
}
