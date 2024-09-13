import React from 'react'
import { useParams, useNavigate } from 'react-router-dom'

import './MenuLateral.css'

const MenuLateral = () => {
  const navigate = useNavigate()

  const handleFlyClick = ()=>{
    navigate("/Fly")
  }

  const handleConexaoClick = ()=>{
    navigate("/Conexao")
  }

  const handleGeralClick = ()=>{
    navigate("/ConfigGeral")
  }

  const handlePIDClick = ()=>{
    navigate("/ConfigPid")
  }

  const handleControleClick = ()=>{
    navigate("/Controle")
  }

  return (
    <div id='menu'>
      <div className='menu-item' onClick={handleConexaoClick}>Conexão</div>
      <div className='menu-item' onClick={handleFlyClick}>Navegação</div>
      <div className='menu-item' onClick={handleGeralClick}>Configurações</div>
      <div className='menu-subitem' onClick={handleGeralClick}>Geral</div>
      <div className='menu-subitem' onClick={handlePIDClick}>PID</div>
      <div className='menu-item' onClick={handleControleClick}>Controle</div>
    </div>
  )
}

export default MenuLateral