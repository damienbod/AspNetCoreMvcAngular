import { inject, async, TestBed, ComponentFixture } from '@angular/core/testing';
import { MockBackend, MockConnection } from '@angular/http/testing';
import {
    Http,
    ConnectionBackend,
    BaseRequestOptions,
    Response,
    ResponseOptions
} from '@angular/http';
import { FormsModule } from '@angular/forms';

import { Configuration } from '../../app/app.constants';
import { ThingService } from '../../app/core/services/thing-data.service';
import { DefaultComponent } from '../../app/default/components/default.component';

describe('DefaultComponent', () => {

    let fixture: ComponentFixture<DefaultComponent>;
    let comp: DefaultComponent;

    let configuration = new Configuration();
    let actionUrl: string = configuration.Server + 'api/things/';

    // Multiple requests with different URL.
    let responses: any = {};
    let data: any = JSON.stringify([{ id: 1, name: "NetCore" }]);
    responses[actionUrl + 'all/'] = new Response(new ResponseOptions({ body: data }));
    
    function expectURL(backend: MockBackend, responses: any) {
        backend.connections.subscribe((c: MockConnection) => {
            let response: any = responses[c.request.url];
            c.mockRespond(response);
        });
    }

    beforeEach(async () => {
        TestBed.configureTestingModule({
            imports: [
                FormsModule
            ],
            providers: [
                BaseRequestOptions,
                MockBackend,
                ThingService,
                Configuration,
                {
                    provide: Http, useFactory: (backend: ConnectionBackend, defaultOptions: BaseRequestOptions) => {
                        return new Http(backend, defaultOptions);
                    }, deps: [MockBackend, BaseRequestOptions]
                }
            ],
            declarations: [DefaultComponent]
        }).compileComponents();
    });

    beforeEach(() => {
        fixture = TestBed.createComponent(DefaultComponent);
        comp = fixture.componentInstance;
    });

    it('on init should get all things', async(
        inject([ThingService, MockBackend],
            ( backend: MockBackend) => {
                // Mock backend for testing the Http service.
                expectURL(backend, responses);

                fixture.detectChanges();
                // Waits for async response.
                fixture.whenStable().then(() => {
                    // Updates view with data.
                    fixture.detectChanges();

                    expect(JSON.stringify(comp.things)).toEqual(data);
                });
            })
    ));

});
