import { Component, Input } from '@angular/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-loading-overlay',
  standalone: true,
  imports: [MatProgressSpinnerModule],
  templateUrl: './loading-overlay.component.html',
  styleUrl: './loading-overlay.component.scss',
})
export class LoadingOverlayComponent {
  @Input() loading?: boolean | null = false;
}
