import { useQuery } from '@tanstack/react-query'
import { getCurrentUser } from './api'

export const authQueryKeys = {
  currentUser: ['auth', 'current-user'],
} as const

export function useCurrentUser() {
  return useQuery({
    queryKey: authQueryKeys.currentUser,
    queryFn: getCurrentUser,
    retry: false,
  })
}
