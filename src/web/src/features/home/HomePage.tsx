import { Link } from 'react-router-dom'
import { useCurrentUser } from '../auth/queries'

export function HomePage() {
  const currentUserQuery = useCurrentUser()

  return (
    <main className="page">
      <h1>Home</h1>
      {currentUserQuery.isPending && <p>Verificando sessao...</p>}
      {currentUserQuery.isSuccess && (
        <p>Usuario autenticado: {currentUserQuery.data.email}</p>
      )}
      {currentUserQuery.isError && <p>Nenhuma sessao ativa encontrada.</p>}
      <Link to="/login">Ir para login</Link>
    </main>
  )
}
