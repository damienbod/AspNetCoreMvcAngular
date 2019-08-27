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

        this.headers = new HttpHeaders();
        this.headers = this.headers.set('Content-Type', 'application/json');
        this.headers = this.headers.set('Accept', 'application/json');
    }

    logout() {
        console.log('logout clicked');
        const toAdd = ''; // JSON.stringify({ });

        this.http.post(this.actionUrl, toAdd)
            .subscribe(() => {
                console.log('finished 200');
                window.location.href = '';
        }, (error) => {
            console.log(error);
        });
    }
}
