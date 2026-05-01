import { useNavigate } from 'react-router-dom'
import { useCurrentUser, useLogout } from '../auth/queries'

export function HomePage() {
  const currentUserQuery = useCurrentUser()
  const logoutMutation = useLogout()
  const navigate = useNavigate()

  function handleLogout() {
    logoutMutation.mutate(undefined, {
      onSuccess: () => {
        navigate('/login')
      },
    })
  }

  return (
    <main className="page">
      <h1>Home</h1>
      {currentUserQuery.isPending && <p>Verificando sessao...</p>}
      {currentUserQuery.isSuccess && (
        <p>Usuario autenticado: {currentUserQuery.data.email}</p>
      )}
      {currentUserQuery.isError && <p>Nenhuma sessao ativa encontrada.</p>}
      <button
        disabled={logoutMutation.isPending}
        onClick={handleLogout}
        type="button"
      >
        {logoutMutation.isPending ? 'Saindo...' : 'Sair'}
      </button>
      {logoutMutation.isError && (
        <p role="alert">Nao foi possivel encerrar a sessao.</p>
      )}
    </main>
  )
}
