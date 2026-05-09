import { Component, inject } from '@angular/core';
import { LoaderService } from '../../../core/services/loader.service';

@Component({
  selector: 'app-loader',
  standalone: true,
  template: `
    @if (loader.isLoading()) {
      <div class="sm-loader-overlay">
        <div class="spinner-border text-light" role="status">
          <span class="visually-hidden">Loading...</span>
        </div>
      </div>
    }
  `,
  styles: [`
    .sm-loader-overlay {
      position: fixed;
      inset: 0;
      background: rgba(15, 23, 42, 0.45);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 1080;
    }
    .spinner-border {
      width: 3rem;
      height: 3rem;
    }
  `]
})
export class LoaderComponent {
  protected readonly loader = inject(LoaderService);
}
