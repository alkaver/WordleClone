import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';
import { adminGuard } from './guards/admin.guard';

export const routes: Routes = [
    {
        path:'',
        pathMatch:'full',
        loadComponent: () =>
        {
            return import('./components/home/home.component').then(
                m => m.HomeComponent
            )
        }
    },
    {
        path: 'login',
        loadComponent: () =>
          import('./components/auth/auth.component').then((m) => m.AuthComponent)
      },
    {
        path: 'instruction',
        loadComponent: () =>
          import('./components/instruction/instruction.component').then((m) => m.InstructionComponent)
      },
    {
        path:'ranking',
        loadComponent: () =>
        {
            return import('./components/ranking/ranking.component').then(
                m => m.RankingComponent
            )
        },
        canActivate:[authGuard]
    },
      {
    path: 'admin',
    loadComponent: () =>
      import('./components/admin/admin.component').then(m => m.AdminComponent),
    canActivate: [adminGuard]
  }
];
