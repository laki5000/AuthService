import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class LoadingService {
  private loadingSubject = new BehaviorSubject<boolean>(false);
  loading$ = this.loadingSubject.asObservable();

  showLoading() {
    console.log('Show loading');
    this.loadingSubject.next(true);
  }

  hideLoading() {
    console.log('Hide loading');
    this.loadingSubject.next(false);
  }
}
