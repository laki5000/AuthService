import { Component } from '@angular/core';
import { UserService } from '../../core/services/user.service';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-dashboard.component',
  standalone: true,
  imports: [MatButtonModule],
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
