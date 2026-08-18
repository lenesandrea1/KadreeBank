import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { I18nService } from './core/i18n.service';
import { ThemeService } from './core/theme.service';
import { TranslatePipe } from './core/translate.pipe';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, TranslatePipe],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  readonly i18n = inject(I18nService);
  readonly theme = inject(ThemeService);
}
