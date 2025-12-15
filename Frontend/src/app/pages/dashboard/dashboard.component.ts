import { Component } from '@angular/core';
import { UserService } from '../../core/services/user.service';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { RolesDialogComponent } from '../../shared/components/roles-dialog/roles-dialog.component';

@Component({
  selector: 'app-dashboard.component',
  standalone: true,
  imports: [MatButtonModule, MatDialogModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent {
  constructor(
    private userService: UserService,
    private dialog: MatDialog,
  ) {}

  onRolesClick(): void {
    const dialogRef: MatDialogRef<RolesDialogComponent> = this.openRolesDialog();

    dialogRef.afterOpened().subscribe(() => {
      dialogRef.componentInstance.getRoles();
    });
  }

  onAmIAdminClick(): void {
    this.userService.amIAdmin().subscribe();
  }

  private openRolesDialog(): MatDialogRef<RolesDialogComponent> {
    return this.dialog.open(RolesDialogComponent, {
      width: '400px',
    });
  }
}
