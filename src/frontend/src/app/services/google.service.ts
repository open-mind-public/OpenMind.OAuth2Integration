import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GoogleEmail, GoogleCalendarEvent } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class GoogleService {
  private readonly API_URL = 'http://localhost:8001/api';

  constructor(private http: HttpClient) {}

  getAuthorizationUrl(): Observable<{ authorizationUrl: string; state: string }> {
    return this.http.get<{ authorizationUrl: string; state: string }>(`${this.API_URL}/google/auth-url`);
  }

  getEmails(maxResults: number = 10): Observable<GoogleEmail[]> {
    return this.http.get<GoogleEmail[]>(`${this.API_URL}/google/emails?maxResults=${maxResults}`);
  }

  getCalendarEvents(timeMin?: Date, timeMax?: Date): Observable<GoogleCalendarEvent[]> {
    let url = `${this.API_URL}/google/calendar/events`;
    const params: string[] = [];

    if (timeMin) {
      params.push(`timeMin=${timeMin.toISOString()}`);
    }
    if (timeMax) {
      params.push(`timeMax=${timeMax.toISOString()}`);
    }

    if (params.length > 0) {
      url += `?${params.join('&')}`;
    }

    return this.http.get<GoogleCalendarEvent[]>(url);
  }

  redirectToGoogleAuth(): void {
    this.getAuthorizationUrl().subscribe(response => {
      window.open(response.authorizationUrl, '_blank');
    });
  }
}