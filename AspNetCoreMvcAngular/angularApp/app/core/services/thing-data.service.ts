import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { Configuration } from './../../app.constants';
import { Thing } from './../../models/thing';

@Injectable()
export class ThingService {

    private actionUrl: string;
    private headers: HttpHeaders;

    constructor(private http: HttpClient, configuration: Configuration) {

        this.actionUrl = configuration.Server + 'api/things/';
    }

    private setHeaders() {
        this.headers = new HttpHeaders();
        this.headers = this.headers.set('Content-Type', 'application/json');
        this.headers = this.headers.set('Accept', 'application/json');
        const token: any = document.getElementById('__RequestVerificationToken');
        this.headers = this.headers.set('X-XSRF-TOKEN', token.value);
    }

    getAll(): Observable<Thing[]> {
        this.setHeaders();
        return this.http.get<Thing[]>(this.actionUrl, { headers: this.headers });
    }

    getSingle(id: number): Observable<Thing> {
        this.setHeaders();
        return this.http.get<Thing>(this.actionUrl + id, { headers: this.headers });
    }

    add(thingToAdd: Thing): Observable<Thing> {
        this.setHeaders();
        const toAdd = JSON.stringify({ name: thingToAdd.name });

        return this.http.post<Thing>(this.actionUrl, toAdd, { headers: this.headers });
    }

    update(id: number, itemToUpdate: any): Observable<Thing> {
        this.setHeaders();
        return this.http
            .put<Thing>(this.actionUrl + id, JSON.stringify(itemToUpdate), { headers: this.headers });
    }

    delete(id: number): Observable<any> {
        this.setHeaders();
        return this.http.delete<any>(this.actionUrl + id, { headers: this.headers });
    }
}
