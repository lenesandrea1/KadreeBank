import { Component, OnInit, inject, signal } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AccountApiService } from '../../core/account-api.service';
import { I18nService } from '../../core/i18n.service';
import { TranslatePipe } from '../../core/translate.pipe';
import { CustomerTransactionCount, OffCityWithdrawal } from '../../core/models';

@Component({
  selector: 'app-reportes-page',
  imports: [FormsModule, CurrencyPipe, TranslatePipe],
  templateUrl: './reportes-page.component.html',
  styleUrl: './reportes-page.component.css'
})
export class ReportesPageComponent implements OnInit {
  private readonly api = inject(AccountApiService);
  readonly i18n = inject(I18nService);

  year = 2026;
  month = 8;
  readonly ranking = signal<CustomerTransactionCount[]>([]);
  readonly offCity = signal<OffCityWithdrawal[]>([]);
  readonly error = signal<string | null>(null);
  readonly loading = signal(false);

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.error.set(null);
    this.api.customersByTransactions(this.year, this.month).subscribe({
      next: (rows) => this.ranking.set(rows),
      error: (e) => this.fail(e)
    });
    this.api.offCityWithdrawals().subscribe({
      next: (rows) => {
        this.offCity.set(rows);
        this.loading.set(false);
      },
      error: (e) => this.fail(e)
    });
  }

  private fail(err: { status?: number; error?: { title?: string; detail?: string } }): void {
    this.loading.set(false);
    if (err.status === 0 || err.status === 504) {
      this.error.set(this.i18n.t('apiError'));
      return;
    }
    this.error.set(err.error?.title ?? err.error?.detail ?? this.i18n.t('fail'));
  }
}
