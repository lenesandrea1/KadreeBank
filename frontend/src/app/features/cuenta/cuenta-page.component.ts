import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AccountApiService } from '../../core/account-api.service';
import { I18nService } from '../../core/i18n.service';
import { TranslatePipe } from '../../core/translate.pipe';
import { AccountSummary, Balance, MonthlyStatement, Movement } from '../../core/models';

@Component({
  selector: 'app-cuenta-page',
  imports: [FormsModule, CurrencyPipe, DatePipe, TranslatePipe],
  templateUrl: './cuenta-page.component.html',
  styleUrl: './cuenta-page.component.css'
})
export class CuentaPageComponent implements OnInit {
  private readonly api = inject(AccountApiService);
  readonly i18n = inject(I18nService);

  readonly accounts = signal<AccountSummary[]>([]);
  readonly balance = signal<Balance | null>(null);
  readonly movements = signal<Movement[]>([]);
  readonly error = signal<string | null>(null);
  readonly loading = signal(false);
  readonly filter = signal<'all' | 'Deposit' | 'Withdrawal'>('all');
  readonly statement = signal<MonthlyStatement | null>(null);

  selectedId = '';
  amount = 0;
  city = '';
  statementYear = 2026;
  statementMonth = 8;
  operation: 'consignacion' | 'retiro' = 'consignacion';

  readonly visibleMovements = computed(() => {
    const f = this.filter();
    return this.movements().filter((m) => f === 'all' || m.type === f);
  });

  readonly totalBalance = computed(() =>
    this.accounts().reduce((sum, a) => sum + a.balance, 0)
  );

  readonly chart = computed(() => this.buildChart());

  ngOnInit(): void {
    this.api.listAccounts().subscribe({
      next: (rows) => {
        this.accounts.set(rows);
        if (rows.length > 0) {
          this.selectedId = rows[0].id;
          this.refresh();
        }
      },
      error: () => this.error.set(this.i18n.t('apiError'))
    });
  }

  selectAccount(id: string): void {
    this.selectedId = id;
    this.refresh();
  }

  refresh(): void {
    if (!this.selectedId) {
      return;
    }
    this.loading.set(true);
    this.error.set(null);
    this.api.getBalance(this.selectedId).subscribe({
      next: (b) => this.balance.set(b),
      error: (e) => this.fail(e)
    });
    this.api.getMovements(this.selectedId, 20).subscribe({
      next: (m) => {
        this.movements.set(m);
        this.loading.set(false);
        this.api.listAccounts().subscribe((rows) => this.accounts.set(rows));
      },
      error: (e) => this.fail(e)
    });
    this.loadStatement();
  }

  loadStatement(): void {
    if (!this.selectedId) {
      return;
    }
    this.api.getStatement(this.selectedId, this.statementYear, this.statementMonth).subscribe({
      next: (s) => this.statement.set(s),
      error: (e) => this.fail(e)
    });
  }

  submit(): void {
    if (this.loading()) {
      return;
    }
    if (!this.selectedId || this.amount <= 0 || !this.city.trim()) {
      this.error.set(this.i18n.t('opError'));
      return;
    }
    this.loading.set(true);
    this.error.set(null);
    const body = { amount: this.amount, city: this.city.trim() };
    const request =
      this.operation === 'consignacion'
        ? this.api.deposit(this.selectedId, body)
        : this.api.withdraw(this.selectedId, body);
    request.subscribe({
      next: () => {
        this.amount = 0;
        this.refresh();
      },
      error: (e) => this.fail(e)
    });
  }

  accountType(type: string): string {
    this.i18n.lang();
    return type === 'Checking' ? this.i18n.t('checking') : this.i18n.t('savings');
  }

  movementType(type: string): string {
    this.i18n.lang();
    return type === 'Deposit' ? this.i18n.t('depositType') : this.i18n.t('withdrawType');
  }

  private buildChart(): { line: string; area: string } {
    const current = this.balance()?.balance ?? 0;
    const ordered = [...this.movements()].sort(
      (a, b) => new Date(a.occurredAt).getTime() - new Date(b.occurredAt).getTime()
    );
    let running = current;
    for (const m of [...ordered].reverse()) {
      running += m.type === 'Deposit' ? -m.amount : m.amount;
    }
    const points = ordered.map((m) => {
      running += m.type === 'Deposit' ? m.amount : -m.amount;
      return running;
    });
    if (points.length === 0) {
      return { line: '', area: '' };
    }
    const min = Math.min(...points);
    const max = Math.max(...points);
    const span = Math.max(max - min, 1);
    const w = 640;
    const h = 180;
    const coords = points.map((v, i) => {
      const x = (i / Math.max(points.length - 1, 1)) * w;
      const y = h - ((v - min) / span) * (h - 16) - 8;
      return `${x},${y}`;
    });
    const line = `M ${coords.join(' L ')}`;
    const area = `${line} L ${w},${h} L 0,${h} Z`;
    return { line, area };
  }

  private fail(err: { status?: number; error?: { title?: string; detail?: string } }): void {
    this.loading.set(false);
    if (err.status === 409) {
      this.error.set(this.i18n.t('concurrencyError'));
      return;
    }
    this.error.set(err.error?.title ?? err.error?.detail ?? this.i18n.t('fail'));
  }
}
