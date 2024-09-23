import React, { createRef, useContext, useEffect, useRef, useState } from 'react'
import './ConexaoPage.css'
import {ApiContext} from '../../context/ApiContext'

const ConexaoPage = () => {
  const inputRef = createRef("192.168.1.46");
  const {setUrl, connectionStatus, sendMessage, ip} = useContext(ApiContext);
  const connectionStatusRef = useRef();

  useEffect(() => {
    inputRef.current.value = ip;
  }, [ip])

  const btnConectar = () =>{
    setUrl(inputRef.current.value);
  };
  

  return (
    <div className='conexaopage-container'>
      <div className='title'>CONEXÃO</div>
      <div className='conexao-form-container'>
        <span>IP:</span>
        <input type="text" ref={inputRef}></input>
        <button onClick={btnConectar}>Conectar</button>
      </div>
    </div>
  )
}

export default ConexaoPage