import { Menu } from './../model/loginuser';
import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../service/auth.service';
import { JwtHelperService } from '@auth0/angular-jwt';
@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
    constructor(
        private router: Router,
        private authenticationService: AuthService,
        private jwtHelper: JwtHelperService
    ) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        const currentUser = this.authenticationService.currentUserValue;
        if (currentUser) {
            if (!this.jwtHelper.isTokenExpired(currentUser.Token)) { // Token是否過期
                let checkUrl = false;
                currentUser.Menu.forEach(element => {
                    element.items.forEach(element2 => {
                        if (element2.routerLink[0] === state.url || state.url === '/') {
                            checkUrl = true;
                        }
                    });
                });
                if (checkUrl) {
                    return true;
                }
            } else {
                this.authenticationService.logout(); // 過期清掉登入資料
            }
        }
        this.router.navigate(['/login']);
        return false;
    }
}
