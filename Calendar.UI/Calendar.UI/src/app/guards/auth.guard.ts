import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  if(inject(AuthService).isLoggedIN())
  {
    return true;
  }
  localStorage.setItem('returnUrl', state.url);
  inject(Router).navigate(['/login']);
  return false;
};
