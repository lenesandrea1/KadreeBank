import { Injectable, computed, signal } from '@angular/core';

export type Lang = 'es' | 'en';

const DICT: Record<Lang, Record<string, string>> = {
  es: {
    brand: 'Kadree Bank',
    welcome: 'Bienvenido',
    welcomeSub: 'Gestione cuentas, movimientos y reportes.',
    menuMain: 'Menú principal',
    menuSupport: 'Soporte',
    navDashboard: 'Tablero',
    navAccounts: 'Cuentas',
    navReports: 'Reportes',
    navHelp: 'Ayuda',
    search: 'Buscar cuenta, cliente o movimiento',
    totalHolding: 'Saldo total',
    returnLabel: 'Variación',
    myAccounts: 'Mis cuentas',
    seeAll: 'Ver todas',
    performance: 'Rendimiento de la cuenta',
    overview: 'Movimientos recientes',
    watchlist: 'Operar',
    deposit: 'Consignación',
    withdraw: 'Retiro',
    amount: 'Monto',
    city: 'Ciudad de la operación',
    apply: 'Aplicar',
    account: 'Cuenta',
    origin: 'Ciudad de origen',
    date: 'Fecha',
    type: 'Tipo',
    all: 'Todos',
    deposits: 'Consignaciones',
    withdrawals: 'Retiros',
    noMovements: 'No hay movimientos.',
    reportsTitle: 'Reportes en tiempo real',
    ranking: 'Clientes por transacciones',
    rankingHint: 'Orden descendente para el mes indicado.',
    offCity: 'Retiros fuera de la ciudad de origen',
    offCityHint: 'Total retirado superior a $1.000.000.',
    year: 'Año',
    month: 'Mes',
    consult: 'Consultar',
    customer: 'Cliente',
    transactions: 'Transacciones',
    totalWithdrawn: 'Total retirado',
    savings: 'Ahorros',
    checking: 'Corriente',
    depositType: 'Consignación',
    withdrawType: 'Retiro',
    apiError: 'No se pudo conectar con la API. ¿Está en el puerto 5203?',
    opError: 'Indique un monto mayor a 0 y la ciudad.',
    fail: 'La operación no se pudo completar.',
    themeLight: 'Claro',
    themeDark: 'Oscuro',
    langEs: 'ES',
    langEn: 'EN',
    profileName: 'Oficial Kadree',
    profileMail: 'ops@kadree.bank',
    range6m: '6M',
    statementTitle: 'Extracto mensual',
    statementHint: 'Saldo inicial, movimientos y saldo final del mes.',
    openingBalance: 'Saldo inicial',
    closingBalance: 'Saldo final',
    generateStatement: 'Generar extracto',
    noStatement: 'No hay movimientos en este mes.',
    concurrencyError: 'Otra operación modificó la cuenta. Intente de nuevo.'
  },
  en: {
    brand: 'Kadree Bank',
    welcome: 'Welcome',
    welcomeSub: 'Manage accounts, movements and reports.',
    menuMain: 'Main menu',
    menuSupport: 'Support',
    navDashboard: 'Dashboard',
    navAccounts: 'Accounts',
    navReports: 'Reports',
    navHelp: 'Help',
    search: 'Search account, customer or movement',
    totalHolding: 'Total holding',
    returnLabel: 'Return',
    myAccounts: 'My accounts',
    seeAll: 'See all',
    performance: 'Account performance',
    overview: 'Recent movements',
    watchlist: 'New operation',
    deposit: 'Deposit',
    withdraw: 'Withdrawal',
    amount: 'Amount',
    city: 'Operation city',
    apply: 'Apply',
    account: 'Account',
    origin: 'Origin city',
    date: 'Date',
    type: 'Type',
    all: 'All',
    deposits: 'Deposits',
    withdrawals: 'Withdrawals',
    noMovements: 'No movements yet.',
    reportsTitle: 'Real-time reports',
    ranking: 'Customers by transactions',
    rankingHint: 'Descending order for the selected month.',
    offCity: 'Withdrawals outside origin city',
    offCityHint: 'Total withdrawn greater than $1,000,000.',
    year: 'Year',
    month: 'Month',
    consult: 'Run report',
    customer: 'Customer',
    transactions: 'Transactions',
    totalWithdrawn: 'Total withdrawn',
    savings: 'Savings',
    checking: 'Checking',
    depositType: 'Deposit',
    withdrawType: 'Withdrawal',
    apiError: 'Could not reach the API. Is it running on port 5203?',
    opError: 'Enter an amount greater than 0 and a city.',
    fail: 'The operation could not be completed.',
    themeLight: 'Light',
    themeDark: 'Dark',
    langEs: 'ES',
    langEn: 'EN',
    profileName: 'Kadree Officer',
    profileMail: 'ops@kadree.bank',
    range6m: '6M',
    statementTitle: 'Monthly statement',
    statementHint: 'Opening balance, movements and closing balance for the month.',
    openingBalance: 'Opening balance',
    closingBalance: 'Closing balance',
    generateStatement: 'Generate statement',
    noStatement: 'No movements in this month.',
    concurrencyError: 'Another operation modified the account. Please try again.'
  }
};

@Injectable({ providedIn: 'root' })
export class I18nService {
  readonly lang = signal<Lang>(this.readLang());
  readonly dict = computed(() => DICT[this.lang()]);

  constructor() {
    document.documentElement.lang = this.lang();
  }

  t(key: string): string {
    return this.dict()[key] ?? key;
  }

  setLang(lang: Lang): void {
    this.lang.set(lang);
    localStorage.setItem('kadree.lang', lang);
    document.documentElement.lang = lang;
  }

  toggle(): void {
    this.setLang(this.lang() === 'es' ? 'en' : 'es');
  }

  private readLang(): Lang {
    const stored = localStorage.getItem('kadree.lang');
    return stored === 'en' || stored === 'es' ? stored : 'es';
  }
}
