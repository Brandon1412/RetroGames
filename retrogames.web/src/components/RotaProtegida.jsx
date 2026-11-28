import { Navigate, useLocation } from 'react-router-dom'
import useAutenticacao from '../hooks/useAutenticacao'

function RotaProtegida({ children, apenasAdministrador = false }) {
  const {
    carregando,
    estaAutenticado,
    ehAdministrador,
  } = useAutenticacao()

  const location = useLocation()

  if (carregando) {
    return (
      <div className="estado" role="status">
        <p>VERIFICANDO ACESSO...</p>
      </div>
    )
  }

  if (!estaAutenticado) {
    return (
      <Navigate
        to="/login"
        replace
        state={{ from: location }}
      />
    )
  }

  if (apenasAdministrador && !ehAdministrador) {
    return <Navigate to="/" replace />
  }

  return children
}

export default RotaProtegida