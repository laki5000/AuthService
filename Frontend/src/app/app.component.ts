import { Component, OnDestroy, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './shared/components/navbar/navbar.component';
import { CommonModule } from '@angular/common';
import { AuthService } from './core/services/auth.service';

@Component({
  selector: 'app-root',
  imports: [CommonModule, RouterOutlet, NavbarComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class App implements OnInit, OnDestroy {
  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.authService.monitorLoginStatus();
  }

  ngOnDestroy(): void {
    this.unsubscribe();
  }

  private unsubscribe(): void {
    this.authService.stopMonitorLoginStatus();
  }
}
