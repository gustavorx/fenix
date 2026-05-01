import { Link, useNavigate } from 'react-router-dom'
import { useLogin } from './queries'
import { useState, type FormEvent } from 'react'
import { ApiRequestError } from '../../shared/api/errors'

export function LoginPage() {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const navigate = useNavigate()
  const loginMutation = useLogin()


  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()

    loginMutation.mutate(
      { email, password },
      {
        onSuccess: () => {
          navigate('/')
        },
      },
    )

  }

  const errorMessage =
    loginMutation.error instanceof ApiRequestError
      ? loginMutation.error.errors[0]?.message
      : null

  return (
    <main className="page">
      <h1>Login</h1>

      <form onSubmit={handleSubmit}>
        <label>
          Email:
          <input
            autoComplete="email"
            name="email"
            onChange={(e) => setEmail(e.target.value)}
            type="email"
            value={email}
          />
        </label>

        <label>
          Senha:
          <input
            autoComplete="current-password"
            name="password"
            onChange={(e) => setPassword(e.target.value)}
            type="password"
            value={password}
          />
        </label>

        {errorMessage && <p role="alert" className="error">{errorMessage}</p>}

        <button disabled={loginMutation.isPending} type="submit">
          {loginMutation.isPending ? 'Entrando...' : 'Entrar'}
        </button>

        <Link to="/">Ir para home</Link>
      </form>
    </main>
  )
}
