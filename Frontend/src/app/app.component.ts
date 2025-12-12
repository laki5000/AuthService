import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './shared/components/navbar/navbar.component';
import { LoadingService } from './core/services/loading.service';
import { Observable } from 'rxjs';
import { AsyncPipe, CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  imports: [CommonModule, RouterOutlet, NavbarComponent, AsyncPipe],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class App {
  loading$: Observable<boolean>;

  constructor(loadingService: LoadingService) {
    this.loading$ = loadingService.loading$;
  }
}
