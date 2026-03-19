import api from './api'
import type { LoginRequest, RegisterRequest, AuthResponse, ChangePasswordRequest } from '@/types/auth'

const TOKEN_KEY = 'winery_token'
const USER_KEY = 'winery_user'

class AuthService {
  async login(credentials: LoginRequest): Promise<AuthResponse> {
    const response = await api.post<AuthResponse>('/auth/login', credentials)
    this.saveToken(response.data.token)
    this.saveUser(response.data)
    return response.data
  }

  async register(data: RegisterRequest): Promise<AuthResponse> {
    const response = await api.post<AuthResponse>('/auth/register', data)
    return response.data
  }

  async changePassword(data: ChangePasswordRequest): Promise<void> {
    await api.post('/auth/change-password', data)
  }

  async getCurrentUser(): Promise<any> {
    const response = await api.get('/auth/me')
    return response.data
  }

  async getProfile(): Promise<any> {
    const response = await api.get('/auth/profile')
    return response.data
  }

  async getAllEmployees(kategorija?: string): Promise<any> {
    const params = kategorija ? { kategorija } : {}
    const response = await api.get('/auth/employees', { params })
    return response.data
  }

  logout(): void {
    this.removeToken()
    this.removeUser()
  }

  saveToken(token: string): void {
    localStorage.setItem(TOKEN_KEY, token)
  }

  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY)
  }

  removeToken(): void {
    localStorage.removeItem(TOKEN_KEY)
  }

  saveUser(user: AuthResponse): void {
    localStorage.setItem(USER_KEY, JSON.stringify(user))
  }

  getUser(): AuthResponse | null {
    const user = localStorage.getItem(USER_KEY)
    return user ? JSON.parse(user) : null
  }

  removeUser(): void {
    localStorage.removeItem(USER_KEY)
  }

  isLoggedIn(): boolean {
    return !!this.getToken()
  }
}

export default new AuthService()

