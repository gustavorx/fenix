import type { ReactNode } from 'react'
import { Navigate } from 'react-router-dom'
import { useCurrentUser } from './queries'

type RequireAuthProps = {
  children: ReactNode
}

export function RequireAuth({ children }: RequireAuthProps) {
  const currentUserQuery = useCurrentUser()

  if (currentUserQuery.isPending) {
    return (
      <main className="page">
        <p>Verificando sessao...</p>
      </main>
    )
  }

  if (currentUserQuery.isError) {
    return <Navigate replace to="/login" />
  }

  return children
}
