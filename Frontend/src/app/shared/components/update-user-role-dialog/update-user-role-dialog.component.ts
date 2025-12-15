import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';

@Component({
  selector: 'app-update-user-role-dialog.component',
  standalone: true,
  imports: [MatDialogModule, MatButtonModule],
  templateUrl: './update-user-role-dialog.component.html',
  styleUrl: './update-user-role-dialog.component.scss',
})
export class UpdateUserRoleDialogComponent {}
