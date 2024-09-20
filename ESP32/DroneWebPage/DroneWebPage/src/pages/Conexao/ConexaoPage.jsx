import React, { createRef, useContext, useEffect, useRef, useState } from 'react'
import './ConexaoPage.css'
import GLPViewer from '../../components/GLPViewer/GLPViewer'
import {ApiContext} from '../../context/ApiContext'

const ConexaoPage = () => {
  const [orientation, setOrientation] = useState({
    Yaw: 0,
    Pitch: 0,
    Roll: 0,
    AccX: 1,
    AccY: 1,
    AccZ: 1,
    GyroX: 1,
    GyroY: 1,
    GyroZ: 1
  })
  const inputRef = createRef("192.168.1.46");
  const {setUrl, connectionStatus, sendMessage, ip} = useContext(ApiContext);
  const connectionStatusRef = useRef();

  useEffect(() => {
    inputRef.current.value = ip;
  }, [ip])

  const btnConectar = () =>{
    setUrl('0.0.0.0')
    setUrl(inputRef.current.value);
  };

  useEffect(() => {
    connectionStatusRef.current = connectionStatus;
    let interval = null
    if(connectionStatus === 'Open'){
      interval = setInterval(()=>{
        if (connectionStatusRef.current === 'Open'){
          fetch("http://" + ip + ":80/getValues")
          .then(response => response.json())
          .then(data => {
            setOrientation(data)
          })
        }
      }, 100)
    }
  
    return () => {
      if (interval !== null){
        clearInterval(interval)
      }
    }
  }, [connectionStatus])
  

  return (
    <div className='conexaopage-container'>
      <div className='title'>CONEXÃO</div>
      <div className='conexao-form-container'>
        <span>IP:</span>
        <input type="text" ref={inputRef}></input>
        <button onClick={btnConectar}>Conectar</button>
      </div>
      <br/>
      <br/>
      <div className='glp-viewer'>
        <GLPViewer Pitch={orientation.Pitch} Yaw={orientation.Yaw} Roll={orientation.Roll} step={0.1}/>
        <div style={{width: "20%", height: "100%", padding: "5px", backgroundColor: "rgb(101, 132, 158)"}}>
          <h3>Orientação:</h3>
          <p>Pitch: {orientation.Pitch}</p>
          <p>Roll: {orientation.Roll}</p>
          <p>Yaw: {orientation.Yaw}</p>
          <h3>Aceleração:</h3>
          <p>X: {orientation.AccX}</p>
          <p>Y: {orientation.AccY}</p>
          <p>Z: {orientation.AccZ}</p>
          <h3>Giroscopio:</h3>
          <p>X: {orientation.GyroX}</p>
          <p>Y: {orientation.GyroY}</p>
          <p>Z: {orientation.GyroZ}</p>
        </div>
      </div>
    </div>
  )
}

export default ConexaoPage