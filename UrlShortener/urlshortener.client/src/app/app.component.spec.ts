import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AppComponent } from './app.component';

describe('AppComponent', () => {
  let component: AppComponent;
  let fixture: ComponentFixture<AppComponent>;
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AppComponent],
      imports: [HttpClientTestingModule]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AppComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should create the app', () => {
    expect(component).toBeTruthy();
  });

  it('should retrieve shortened URLs from the server', () => {
    const mockUrls = [
      { id: '1', longUrl: 'https://example.com', shortUrl: 'https://localhost:7271/abc123', code: 'abc123', createdAt: '2024-01-01T00:00:00' },
      { id: '2', longUrl: 'https://another.com', shortUrl: 'https://localhost:7271/xyz789', code: 'xyz789', createdAt: '2024-01-02T00:00:00' }
    ];

    component.ngOnInit();

    const req = httpMock.expectOne('https://localhost:7271/api/urls');
    expect(req.request.method).toEqual('GET');
    req.flush(mockUrls);

    expect(component.urls).toEqual(mockUrls);
  });
});
