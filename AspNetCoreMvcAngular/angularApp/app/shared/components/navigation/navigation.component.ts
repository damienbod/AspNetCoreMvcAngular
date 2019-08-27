import { Component } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Configuration } from './../../../app.constants';

@Component({
    selector: 'app-navigation-component',
    templateUrl: 'navigation.component.html'
})

export class NavigationComponent {

    private actionUrl: string;
    private headers: HttpHeaders;

    constructor(private http: HttpClient, configuration: Configuration) {

        this.actionUrl = configuration.Server + 'api/logout';
    }

    logout() {
        console.log('logout clicked');
        const toAdd = ''; // JSON.stringify({ });
        this.setHeaders();
        this.http.post(this.actionUrl, toAdd, { headers: this.headers})
            .subscribe(() => {
                console.log('finished 200');
                window.location.href = '';
        }, (error) => {
            console.log(error);
        });
    }

    private setHeaders() {
        this.headers = new HttpHeaders();
        this.headers = this.headers.set('Content-Type', 'application/json');
        this.headers = this.headers.set('Accept', 'application/json');
        const token: any = document.getElementById('__RequestVerificationToken');
        this.headers = this.headers.set('X-XSRF-TOKEN', token.value);
        console.log(token.value);
    }
}
