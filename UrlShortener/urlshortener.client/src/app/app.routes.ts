import { Routes } from '@angular/router';
import { LoginComponent } from './user/login/login.component';
import { MainLayoutComponent } from './layouts/main-layout.component/main-layout.component';
import { AboutComponent } from './about.component/about.component';
import { UrlTableComponent } from './url-table/url-table.component';
import { UrlInfoComponent } from './url-info/url-info.component';

export const routes: Routes = [
  { path: '', redirectTo: '/urls', pathMatch: 'full' },
  {
    path: 'signin', component: LoginComponent,
    children: [
      { path: '', component: LoginComponent },
    ]
  },
  {
    path: '', component: MainLayoutComponent,
    children: [
      { path: 'urls', component: UrlTableComponent },
      { path: 'info/:code', component: UrlInfoComponent },
      { path: 'about', component: AboutComponent },
    ]
  },
];
