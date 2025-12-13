import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { globalErrorInterceptor } from './core/interceptors/global-error.interceptor';
import { loadingInterceptor } from './core/interceptors/loading.interceptor';
import { globalResponseInterceptor } from './core/interceptors/global-response.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideClientHydration(withEventReplay()),
    provideHttpClient(withInterceptors([
      globalErrorInterceptor, 
      loadingInterceptor, 
      globalResponseInterceptor
    ]), withFetch()),
  ],
};
