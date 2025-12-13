import { Component } from '@angular/core';
import { UserService } from '../../core/services/user.service';

@Component({
  selector: 'app-dashboard.component',
  standalone: true,
  imports: [],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent {
  constructor(private userService: UserService) {}

  onRolesClick(): void {}

  onAmIAdminClick(): void {
    this.userService.amIAdmin().subscribe();
  }
}
