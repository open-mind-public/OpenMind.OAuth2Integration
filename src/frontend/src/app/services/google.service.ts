import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { GoogleEmail, GoogleCalendarEvent } from '../models/models';

export interface TokenExpiredError {
  error: string;
  message: string;
  provider: string;
  requiresReauthorization: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class GoogleService {
  private readonly API_URL = 'http://localhost:8001/api';

  constructor(private http: HttpClient) {}

  getAuthorizationUrl(): Observable<{ authorizationUrl: string; state: string }> {
    return this.http.get<{ authorizationUrl: string; state: string }>(`${this.API_URL}/google/authorize`);
  }

  getEmails(maxResults: number = 10): Observable<GoogleEmail[]> {
    return this.http.get<GoogleEmail[]>(`${this.API_URL}/google/emails?maxResults=${maxResults}`)
      .pipe(catchError(this.handleError.bind(this)));
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

    return this.http.get<GoogleCalendarEvent[]>(url)
      .pipe(catchError(this.handleError.bind(this)));
  }

  getConnectionStatus(): Observable<{ provider: string; isConnected: boolean }> {
    return this.http.get<{ provider: string; isConnected: boolean }>(`${this.API_URL}/google/status`);
  }

  redirectToGoogleAuth(): void {
    this.getAuthorizationUrl().subscribe(response => {
      window.location.href = response.authorizationUrl;
    });
  }

  isTokenExpiredError(error: any): error is TokenExpiredError {
    return error?.error === 'token_expired' && error?.requiresReauthorization === true;
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    if (error.status === 403 && error.error?.error === 'token_expired') {
      // Return the error body so it can be handled by the component
      return throwError(() => error.error as TokenExpiredError);
    }
    return throwError(() => error);
  }
}