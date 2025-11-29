import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../enviroments/enviroment';
import { ShortenedUrl } from '../../models/shortened-url.model';

@Injectable({
  providedIn: 'root'
})
export class UrlService {
  private apiUrl = `${environment.apiBaseUrl}/shortener`;

  constructor(private http: HttpClient) { }

  getUrls(): Observable<ShortenedUrl[]> {
    return this.http.get<ShortenedUrl[]>(`${this.apiUrl}/urls`);
  }

  getUrlInfo(code: string): Observable<ShortenedUrl> {
    return this.http.get<ShortenedUrl>(`${this.apiUrl}/info/${code}`);
  }

  shortenUrl(url: string): Observable<ShortenedUrl> {
    return this.http.post<ShortenedUrl>(`${this.apiUrl}`, { url });
  }

  deleteUrl(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
