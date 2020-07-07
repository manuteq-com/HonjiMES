import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../service/auth.service';
@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
    constructor(
        private router: Router,
        private authenticationService: AuthService
    ) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        debugger;
        const currentUser = this.authenticationService.currentUserValue;
        if (currentUser) {
            // 檢查是不是 Timeout 了
            const dateTime = new Date();
            const Timeout = new Date(currentUser.Timeout);
            if (Timeout > dateTime) {
                return true;
            } else {
                this.authenticationService.logout();
            }
        }
        // not logged in so redirect to login page with the return url
        // this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
        this.router.navigate(['/login']);
        return false;
    }
}
