import { Link } from 'react-router-dom'

export function LoginPage() {
  return (
    <main className="page">
      <h1>Login</h1>
      <p>Esta pagina vai receber o formulario de autenticacao.</p>
      <Link to="/">Ir para home</Link>
    </main>
  )
}
