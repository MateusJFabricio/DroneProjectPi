import React, {useContext, useState} from 'react'
import './MPU6050Page.css'
import {ApiContext} from '../../context/ApiContext'

const MPU6050Page = () => {
  const [statusCalibracao, setStatusCalibracao] = useState('')
  const {setUrl, connectionStatus, sendMessage, ip} = useContext(ApiContext);
  const handleCalibrarClick = ()=>{
    try {
      setStatusCalibracao('Iniciando a calibração... Aguarde!')
      fetch("http://" + ip + ":80/postAnglesCalibration", {
        method: "POST",
        body: "",
        headers: {
          "Content-type": "text/plain;"
        }
      })
      .then(response => setStatusCalibracao("Finalizado"))
        .catch((error)=>{
          setStatusCalibracao("Erro: " + error.message)
        })
    } catch (error) {
      setStatusCalibracao("Erro: " + error.message)
    }
  }

  return (
    <div>
      <button onClick={handleCalibrarClick}>Calibrar Sensor</button>
      <p>{statusCalibracao}</p>
    </div>
  )
}

export default MPU6050Page