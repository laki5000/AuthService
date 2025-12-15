import { ChangeDetectorRef, Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatListModule } from '@angular/material/list';
import { RoleService } from '../../../core/services/role.service';
import { ResultDto } from '../../../core/models/result.dto';
import { AddRoleDialogComponent } from '../add-role-dialog/add-role-dialog.component';
import { UpdateUserRoleDialogComponent } from '../update-user-role-dialog/update-user-role-dialog.component';

@Component({
  selector: 'app-roles-dialog',
  standalone: true,
  imports: [MatDialogModule, MatButtonModule, MatListModule],
  templateUrl: './roles-dialog.component.html',
  styleUrl: './roles-dialog.component.scss',
})
export class RolesDialogComponent {
  roles: string[] = [];

  constructor(
    private roleService: RoleService,
    private dialog: MatDialog,
    private cdr: ChangeDetectorRef,
  ) {}

  getRoles(): void {
    this.roleService.getAll().subscribe({
      next: (result: ResultDto<string[]>) => {
        this.roles = result.Result ?? [];
        this.cdr.detectChanges();
      },
    });
  }

  onAddRoleClick(): void {
    const dialogRef: MatDialogRef<AddRoleDialogComponent> = this.openAddRoleDialog();
    dialogRef.afterClosed().subscribe((roleName: string) => {
      if (roleName) {
        this.roles.push(roleName);
        this.cdr.detectChanges();
      }
    });
  }

  onUpdateUserRoleClick(): void {
    this.openUpdateUserRoleDialog();
  }

  private openAddRoleDialog(): MatDialogRef<AddRoleDialogComponent> {
    return this.dialog.open(AddRoleDialogComponent, {
      width: '400px',
    });
  }

  private openUpdateUserRoleDialog(): void {
    this.dialog.open(UpdateUserRoleDialogComponent, {
      width: '400px',
      data: { roles: this.roles },
    });
  }
}
