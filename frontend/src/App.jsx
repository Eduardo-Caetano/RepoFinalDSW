import { BrowserRouter, Route, Routes } from 'react-router-dom'
import Home from './pages/Home.jsx'
import DeputadoDetalhe from './pages/DeputadoDetalhe.jsx'
import LandingPage from './pages/LandingPage.jsx'

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<LandingPage />} />
        <Route path="/deputados" element={<Home />} />
        <Route path="/deputado/:id" element={<DeputadoDetalhe />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App
