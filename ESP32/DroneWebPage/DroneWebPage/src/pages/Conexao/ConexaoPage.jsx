import React, { createRef } from 'react'
import './ConexaoPage.css'
import GLPViewer from '../../components/GLPViewer/GLPViewer'

const ConexaoPage = () => {
  const inputRef = createRef("192.168.1.45");

  function IpChange(value){
    console.log(value);
  }
  return (
    <div className='conexaopage-container'>
      <div className='title'>CONEXÃO</div>
      <div className='conexao-form-container'>
        <span>IP:</span>
        <input type="text" ref={inputRef} onChange={(e)=>IpChange(e.target.value)}></input>
        <button>Conectar</button>
      </div>
      <div className='conexao-joystick-container'>
        <span>Status Joystick: </span>
        <span className='conexao-status-joystick'>Desconectado</span>
      </div>
      <br/>
      <br/>
      <div className='glp-viewer'>
        <GLPViewer Pitch={0} Yaw={0} Roll={0}/>
      </div>
    </div>
  )
}

export default ConexaoPage