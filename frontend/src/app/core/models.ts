export interface AccountSummary {
  id: string;
  number: string;
  type: string;
  originCity: string;
  balance: number;
  customerId: string;
}

export interface Balance {
  accountId: string;
  number: string;
  balance: number;
  originCity: string;
  type: string;
}

export interface Movement {
  id: string;
  type: string;
  amount: number;
  city: string;
  occurredAt: string;
}

export interface MoneyOperation {
  amount: number;
  city: string;
}

export interface CustomerTransactionCount {
  customerId: string;
  customerName: string;
  transactionCount: number;
}

export interface OffCityWithdrawal {
  customerId: string;
  customerName: string;
  accountNumber: string;
  originCity: string;
  totalWithdrawn: number;
}

export interface MonthlyStatement {
  accountId: string;
  number: string;
  year: number;
  month: number;
  openingBalance: number;
  closingBalance: number;
  movements: Movement[];
}
