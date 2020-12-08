import { ThingService } from './../../core/services/thing-data.service';
import { Thing } from './../../models/thing';
import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'app-default-component',
    templateUrl: 'default.component.html'
})

export class DefaultComponent implements OnInit {

    public message: string;
    public things: Thing[] = [];
    public thing: Thing = new Thing();

    constructor(private dataService: ThingService) {
        this.message = 'Things from the ASP.NET Core API';
    }

    ngOnInit() {
        this.getAllThings();
    }

    public addThing() {
        this.dataService
            .add(this.thing)
            .subscribe(() => {
                this.getAllThings();
                this.thing = new Thing();
            }, (error) => {
                console.log(error);
            });
    }

    public deleteThing(thing: Thing) {
        this.dataService
            .delete(thing.id)
            .subscribe(() => {
                this.getAllThings();
            }, (error) => {
                console.log(error);
            });
    }

    private getAllThings() {
        this.dataService
            .getAll()
            .subscribe(
            data => this.things = data,
            error => console.log(error),
            () => console.log('Get all complete')
            );
    }
}
