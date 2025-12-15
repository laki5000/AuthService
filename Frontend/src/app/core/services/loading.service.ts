import { Overlay, OverlayRef } from '@angular/cdk/overlay';
import { ComponentPortal } from '@angular/cdk/portal';
import { Injectable, Injector } from '@angular/core';
import { LoadingOverlayComponent } from '../../shared/components/loading-overlay/loading-overlay.component';

@Injectable({
  providedIn: 'root',
})
export class LoadingService {
  private overlayRef: OverlayRef | undefined;

  constructor(private overlay: Overlay, private injector: Injector) {}
  
  showLoading() {
    if (!this.overlayRef) {
      this.overlayRef = this.overlay.create({
        hasBackdrop: true,
        positionStrategy: this.overlay.position()
          .global()
          .centerHorizontally()
          .centerVertically(),
        scrollStrategy: this.overlay.scrollStrategies.block(),
      });
    }

    if (!this.overlayRef.hasAttached()) {
      this.overlayRef.attach(new ComponentPortal(LoadingOverlayComponent, null, this.injector));
    }
  }

  hideLoading() {
    if (this.overlayRef && this.overlayRef.hasAttached()) {
      this.overlayRef.detach();
    }
  }
}
