export interface User {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  hasGoogleAccess: boolean;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface GoogleLoginRequest {
  idToken: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
}

export interface LoginResponse {
  token: string;
  expiresAt: string;
  user: User;
}

export interface GoogleEmail {
  id: string;
  threadId: string;
  subject: string;
  from: string;
  to: string;
  body: string;
  date: string;
  isRead: boolean;
}

export interface GoogleCalendarEvent {
  id: string;
  summary: string;
  description: string;
  start?: string;
  end?: string;
  location: string;
  attendees: string[];
}