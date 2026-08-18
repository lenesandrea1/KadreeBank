import { Injectable, signal } from '@angular/core';

export type Theme = 'dark' | 'light';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  readonly theme = signal<Theme>(this.read());

  constructor() {
    this.apply(this.theme());
  }

  toggle(): void {
    this.set(this.theme() === 'dark' ? 'light' : 'dark');
  }

  set(theme: Theme): void {
    this.theme.set(theme);
    localStorage.setItem('kadree.theme', theme);
    this.apply(theme);
  }

  private apply(theme: Theme): void {
    document.documentElement.dataset['theme'] = theme;
  }

  private read(): Theme {
    const stored = localStorage.getItem('kadree.theme');
    return stored === 'light' || stored === 'dark' ? stored : 'dark';
  }
}
