import React, {useContext, useState} from 'react'
import './MPU6050Page.css'
import {ApiContext} from '../../context/ApiContext'

const MPU6050Page = () => {
  const [statusCalibracao, setStatusCalibracao] = useState('')
  const [statusCalibracaoEsc, setStatusCalibracaoEsc] = useState('')
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

  const handleCalibrarEsc = ()=>{

  }

  return (
    <>
      <div style={{display: 'flex', flexDirection: 'column', alignItems: 'center', width: '90%'}}>
        <div style={{display: 'flex', flexDirection: 'column', width: '100%', height: '40px', fontSize: '25px', alignItems: 'center', justifyContent: 'center'}}>CALIBRAÇÕES</div>
        <div style={{display: 'flex', flexDirection: 'column', gap: '20px', height: 'auto', width: '100%'}}>
          {/* Giroscopiop */}
          <form style={{width: '400px', height: '100px'}}>
          <fieldset style={{height: '100%'}}>
            <legend>Calibrar o Giroscopio</legend>
            <div style={{display: "flex", gap: "10px"}}>
              <input type="button" value="Calibrar Sensor" onClick={handleCalibrarClick}/>
            </div>
            <p style={{borderStyle: 'solid', borderWidth: '1px'}}>Status:{statusCalibracao}</p>
          </fieldset>
          </form>

          {/* Esc */}
          <form style={{width: '400px', minHeight: '100px'}}>
          <fieldset style={{height: '100%'}}>
            <legend>Calibrar os ESCs</legend>
            <div style={{display: "flex", gap: "10px"}}>
              <input type="button" value="Iniciar calibração" onClick={handleCalibrarEsc}/>
            </div>
            <p style={{borderStyle: 'solid', borderWidth: '1px'}}>Status:{statusCalibracaoEsc}</p>
            <p>
              <h3>Procedimento:</h3>
              <ol>
                <li>Habilitar a calibração</li>
                <li>O ESP32 inicia automaticamente</li>
                <li>A sequência de calibração inicia</li>
                <li>Quando finalizado, o ESP32 reinicia</li>
                <li>O ESP32 envia uma resposta de finalizado</li>
              </ol>
            </p>
          </fieldset>
          </form>

        </div>
      </div>
    </>
  )
}

export default MPU6050Page