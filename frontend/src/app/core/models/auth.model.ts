export interface LoginRequest {
  username: string;
  password: string;
}

export interface AuthResponse {
  accessToken: string;
  expiresAt: string;
  username: string;
  email: string;
  role: string;
}

export interface AuthSession extends AuthResponse {
  expiresAtDate: Date;
}
