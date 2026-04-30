import { apiRequest } from '../../shared/api/http'

export type AuthUser = {
  id: string
  name: string
  email: string
}

export function getCurrentUser(): Promise<AuthUser> {
  return apiRequest<AuthUser>('/api/auth/me')
}
