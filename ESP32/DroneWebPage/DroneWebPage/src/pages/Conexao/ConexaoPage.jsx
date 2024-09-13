import React, { createRef, useContext, useEffect, useRef, useState } from 'react'
import './ConexaoPage.css'
import GLPViewer from '../../components/GLPViewer/GLPViewer'
import {ApiContext} from '../../context/ApiContext'

const ConexaoPage = () => {
  const [orientation, setOrientation] = useState({
    Yaw: 0,
    Pitch: 0,
    Roll: 0,
  })
  const inputRef = createRef("192.168.1.46");
  const {setUrl, connectionStatus, sendMessage, ip} = useContext(ApiContext);
  const interval = useRef()

  useEffect(() => {
    inputRef.current.value = ip;
  }, [ip])

  useEffect(()=>{
    //Get orientation
    interval.current = setInterval(() => { 
      //console.log("http://" + ip + ":80/getValues");
      /*
      fetch("http://" + ip + ":80/getValues")
      .then(response => response.json())
      .then(data => {
        setOrientation(data);
      })
        */
    }, 1000);

  }, [])

  

  function IpChange(value){
    //console.log(value);
  }

  const btnConectar = () =>{
    setUrl(inputRef.current.value);
  };

  return (
    <div className='conexaopage-container'>
      <div className='title'>CONEXÃO</div>
      <div className='conexao-form-container'>
        <span>IP:</span>
        <input type="text" ref={inputRef} onChange={(e)=>IpChange(e.target.value)}></input>
        <button onClick={btnConectar}>Conectar</button>
      </div>
      <br/>
      <br/>
      <div className='glp-viewer'>
        <GLPViewer Pitch={orientation.Pitch} Yaw={orientation.Yaw} Roll={orientation.Roll}/>
      </div>
    </div>
  )
}

export default ConexaoPage