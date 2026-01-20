import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from '../../services/auth.service';
import { GoogleService, TokenExpiredError } from '../../services/google.service';
import { User, GoogleEmail, GoogleCalendarEvent } from '../../models/models';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit {
  currentUser: User | null = null;
  emails: GoogleEmail[] = [];
  events: GoogleCalendarEvent[] = [];
  loadingEmails = false;
  loadingEvents = false;
  tokenExpired = false;

  constructor(
    private authService: AuthService,
    private googleService: GoogleService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
      if (user?.hasGoogleAccess) {
        this.loadEmails();
        this.loadCalendarEvents();
      }
    });

    // Load current user if not already loaded
    if (!this.currentUser) {
      this.authService.getCurrentUser().subscribe();
    }
  }

  connectGoogleAccount(): void {
    this.tokenExpired = false;
    this.googleService.redirectToGoogleAuth();
    this.snackBar.open(
      'Please complete the authorization in the popup window, then refresh this page.', 
      'Close', 
      { duration: 10000 }
    );
  }

  loadEmails(): void {
    this.loadingEmails = true;
    this.googleService.getEmails(10).subscribe({
      next: (emails) => {
        this.emails = emails;
        this.loadingEmails = false;
        this.tokenExpired = false;
      },
      error: (error) => {
        console.error('Failed to load emails:', error);
        this.loadingEmails = false;
        
        if (this.googleService.isTokenExpiredError(error)) {
          this.handleTokenExpired(error);
        } else {
          this.snackBar.open('Failed to load emails', 'Close', { duration: 3000 });
        }
      }
    });
  }

  loadCalendarEvents(): void {
    this.loadingEvents = true;
    const timeMin = new Date();
    const timeMax = new Date();
    timeMax.setDate(timeMax.getDate() + 30);

    this.googleService.getCalendarEvents(timeMin, timeMax).subscribe({
      next: (events) => {
        this.events = events;
        this.loadingEvents = false;
        this.tokenExpired = false;
      },
      error: (error) => {
        console.error('Failed to load calendar events:', error);
        this.loadingEvents = false;
        
        if (this.googleService.isTokenExpiredError(error)) {
          this.handleTokenExpired(error);
        } else {
          this.snackBar.open('Failed to load calendar events', 'Close', { duration: 3000 });
        }
      }
    });
  }

  private handleTokenExpired(error: TokenExpiredError): void {
    this.tokenExpired = true;
    this.emails = [];
    this.events = [];
    
    const snackBarRef = this.snackBar.open(
      'Your Google access has expired. Please reconnect your account.',
      'Reconnect',
      { duration: 10000 }
    );
    
    snackBarRef.onAction().subscribe(() => {
      this.connectGoogleAccount();
    });
  }
}