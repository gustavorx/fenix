import { Link } from 'react-router-dom'

export function HomePage() {
  return (
    <main className="page">
      <h1>Home</h1>
      <p>Esta sera a primeira area protegida do Fenix.</p>
      <Link to="/login">Ir para login</Link>
    </main>
  )
}
