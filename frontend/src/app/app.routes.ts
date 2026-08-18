import { Routes } from '@angular/router';
import { CuentaPageComponent } from './features/cuenta/cuenta-page.component';
import { ReportesPageComponent } from './features/reportes/reportes-page.component';

export const routes: Routes = [
  { path: '', component: CuentaPageComponent },
  { path: 'reportes', component: ReportesPageComponent },
  { path: '**', redirectTo: '' }
];
