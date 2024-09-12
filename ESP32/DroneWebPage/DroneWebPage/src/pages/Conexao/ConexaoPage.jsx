import React from 'react'
import './ConexaoPage.css'

const ConexaoPage = () => {
  return (
    <div className='conexaopage-container'>
      <div className='title'>CONEXÃO</div>
      <div className='conexao-form-container'>
        <span>IP:</span>
        <input type="text" value="192.168.1.45"></input>
        <button>Conectar</button>
      </div>
      <div className='conexao-joystick-container'>
        <span>Status Joystick: </span>
        <span className='conexao-status-joystick'>Desconectado</span>
      </div>
    </div>
  )
}

export default ConexaoPage