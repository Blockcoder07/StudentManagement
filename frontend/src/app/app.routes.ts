import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { guestGuard } from './core/guards/guest.guard';
import { ShellComponent } from './shared/layout/shell/shell';

export const routes: Routes = [
  {
    path: 'login',
    canActivate: [guestGuard],
    loadComponent: () => import('./features/auth/login/login').then(m => m.LoginComponent),
    title: 'Sign in · Student Management'
  },
  {
    path: '',
    component: ShellComponent,
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./features/dashboard/dashboard').then(m => m.DashboardComponent),
        title: 'Dashboard · Student Management'
      },
      {
        path: 'students',
        loadComponent: () => import('./features/students/student-list/student-list').then(m => m.StudentListComponent),
        title: 'Students · Student Management'
      },
      {
        path: 'students/new',
        loadComponent: () => import('./features/students/student-form/student-form').then(m => m.StudentFormComponent),
        title: 'Add Student · Student Management'
      },
      {
        path: 'students/:id/edit',
        loadComponent: () => import('./features/students/student-form/student-form').then(m => m.StudentFormComponent),
        title: 'Edit Student · Student Management'
      },
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' }
    ]
  },
  { path: '**', redirectTo: 'dashboard' }
];
