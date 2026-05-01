import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { getCurrentUser, login } from './api'

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

export function useLogin() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: login,
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: authQueryKeys.currentUser })
    }
  })
}