import { Routes, Route } from 'react-router-dom'
import Layout from './layouts/Layout'
import Catalogo from './pages/Catalogo'
import DetalheJogo from './pages/DetalheJogo'
import Login from './pages/Login'
import NaoEncontrada from './pages/NaoEncontrada'
import PainelAdmin from './pages/PainelAdmin'
import GerenciarPlataformas from './pages/GerenciarPlataformas'
import GerenciarJogos from './pages/GerenciarJogos'
import GerenciarAdministradores from './pages/GerenciarAdministradores'
import AlterarMinhaSenha from './pages/AlterarMinhaSenha'
import RotaProtegida from './components/RotaProtegida'
import './App.css'

function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        {/* Paginas publicas */}
        <Route path="/" element={<Catalogo />} />
        <Route path="/login" element={<Login />} />
        <Route path="/jogos/:id" element={<DetalheJogo />} />

        {/* Painel administrativo */}
        <Route
          path="/admin"
          element={
            <RotaProtegida apenasAdministrador>
              <PainelAdmin />
            </RotaProtegida>
          }
        />

        {/* Gerenciamento de plataformas */}
        <Route
          path="/admin/plataformas"
          element={
            <RotaProtegida apenasAdministrador>
              <GerenciarPlataformas />
            </RotaProtegida>
          }
        />

        {/* Gerenciamento de jogos */}
        <Route
          path="/admin/jogos"
          element={
            <RotaProtegida apenasAdministrador>
              <GerenciarJogos />
            </RotaProtegida>
          }
        />

        {/* Gerenciamento de administradores */}
        <Route
          path="/admin/administradores"
          element={
            <RotaProtegida apenasAdministrador>
              <GerenciarAdministradores />
            </RotaProtegida>
          }
        />

        {/* Alteracao da senha do proprio administrador */}
        <Route
          path="/admin/minha-senha"
          element={
            <RotaProtegida apenasAdministrador>
              <AlterarMinhaSenha />
            </RotaProtegida>
          }
        />

        <Route path="*" element={<NaoEncontrada />} />
      </Route>
    </Routes>
  )
}

export default App
