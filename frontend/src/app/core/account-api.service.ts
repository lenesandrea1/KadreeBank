import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  AccountSummary,
  Balance,
  CustomerTransactionCount,
  MoneyOperation,
  MonthlyStatement,
  Movement,
  OffCityWithdrawal
} from './models';

@Injectable({ providedIn: 'root' })
export class AccountApiService {
  private readonly baseUrl = 'http://localhost:5203/api';

  constructor(private readonly http: HttpClient) {}

  listAccounts(): Observable<AccountSummary[]> {
    return this.http.get<AccountSummary[]>(`${this.baseUrl}/cuentas`);
  }

  getBalance(accountId: string): Observable<Balance> {
    return this.http.get<Balance>(`${this.baseUrl}/cuentas/${accountId}/saldo`);
  }

  getMovements(accountId: string, take = 10): Observable<Movement[]> {
    const params = new HttpParams().set('take', take);
    return this.http.get<Movement[]>(`${this.baseUrl}/cuentas/${accountId}/movimientos`, { params });
  }

  deposit(accountId: string, body: MoneyOperation): Observable<Balance> {
    return this.http.post<Balance>(`${this.baseUrl}/cuentas/${accountId}/consignaciones`, body);
  }

  withdraw(accountId: string, body: MoneyOperation): Observable<Balance> {
    return this.http.post<Balance>(`${this.baseUrl}/cuentas/${accountId}/retiros`, body);
  }

  getStatement(accountId: string, year: number, month: number): Observable<MonthlyStatement> {
    return this.http.get<MonthlyStatement>(
      `${this.baseUrl}/cuentas/${accountId}/extractos/${year}/${month}`
    );
  }

  customersByTransactions(year: number, month: number): Observable<CustomerTransactionCount[]> {
    const params = new HttpParams().set('year', year).set('month', month);
    return this.http.get<CustomerTransactionCount[]>(
      `${this.baseUrl}/reportes/clientes-transacciones`,
      { params }
    );
  }

  offCityWithdrawals(): Observable<OffCityWithdrawal[]> {
    return this.http.get<OffCityWithdrawal[]>(`${this.baseUrl}/reportes/retiros-fuera-ciudad`);
  }
}
