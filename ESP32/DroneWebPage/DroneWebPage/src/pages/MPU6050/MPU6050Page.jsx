import React, {useContext, useState, useEffect} from 'react'
import '@vaadin/progress-bar';
import './MPU6050Page.css'
import {ApiContext} from '../../context/ApiContext'

const MPU6050Page = () => {
  const [statusCalibracao, setStatusCalibracao] = useState('')
  const [statusCalibracaoEsc, setStatusCalibracaoEsc] = useState('')
  const [filtroComplementarGyro, setFiltroComplementarGyro] = useState(0.0)
  const [filtroComplementarAcc, setFiltroComplementarAcc] = useState(1.0)
  const [calibracaoEmProgresso, setCalibracaoEmProgresso] = useState(false)
  const [salvarFiltroComplementarProgresso, setSalvarFiltroComplementarProgresso] = useState(false)
  const {setUrl, connectionStatus, sendMessage, ip} = useContext(ApiContext);
  const [kalmanQAngle_Pitch, setKalmanQAngle_Pitch] = useState(0)
  const [kalmanQBias_Pitch, setKalmanQBias_Pitch] = useState(0)
  const [kalmanRMeasure_Pitch, setKalmanRMeasure_Picth] = useState(0)
  const [kalmanQAngle_Roll, setKalmanQAngle_Roll] = useState(0)
  const [kalmanQBias_Roll, setKalmanQBias_Roll] = useState(0)
  const [kalmanRMeasure_Roll, setKalmanRMeasure_Roll] = useState(0)

  const handleCalibrarClick = ()=>{
    try {
      setStatusCalibracao('Iniciando a calibração... Aguarde!')
      setCalibracaoEmProgresso(true)
      fetch("http://" + ip + ":80/postAnglesCalibration", {
        method: "POST",
        body: "",
        headers: {
          "Content-type": "text/plain;"
        }
      })
      .then(response => {
        setStatusCalibracao("Finalizado")
        setCalibracaoEmProgresso(false)
      })
      .catch((error)=>{
        setStatusCalibracao("Erro: " + error.message)
        setCalibracaoEmProgresso(false)
      })
    } catch (error) {
      setStatusCalibracao("Erro: " + error.message)
      setCalibracaoEmProgresso(false)
    }finally{
      
    }
  }

  useEffect(() => {
    if (filtroComplementarAcc !== 1 - filtroComplementarGyro){
      setFiltroComplementarAcc(1 - filtroComplementarGyro);
    }
  
  }, [filtroComplementarGyro])

  useEffect(() => {
    if (filtroComplementarGyro !== 1 - filtroComplementarAcc){
      setFiltroComplementarGyro(1 - filtroComplementarAcc);
    }
  
  }, [filtroComplementarAcc])
  

  const handleCalibrarEsc = ()=>{

  }

  const handleSalvarFiltroComplementar = ()=>{
    try {
      setSalvarFiltroComplementarProgresso(true)

      fetch("http://" + ip + ":80/postGyroParam", {
        method: "POST",
        body: JSON.stringify({
          ComplementaryAlpha: filtroComplementarGyro,
          Pitch_Q_angle: kalmanQAngle_Pitch,
          Pitch_Q_bias: kalmanQBias_Pitch,
          Pitch_R_measure: kalmanRMeasure_Pitch,
          Roll_Q_angle: kalmanQAngle_Roll,
          Roll_Q_bias: kalmanQBias_Roll,
          Roll_R_measure: kalmanRMeasure_Roll
        }),
        headers: {
          "Content-type": "text/plain;"
        }
      })
      .then(response => {
        console.log(response)
        setSalvarFiltroComplementarProgresso(false)
      })
      .catch((error)=>{
        setSalvarFiltroComplementarProgresso(false)
      })
    } catch (error) {
      setSalvarFiltroComplementarProgresso(false)
    }
  }

  const handleAtualizarFiltroComplementar = ()=>{
    try {
      setSalvarFiltroComplementarProgresso(true)

      fetch("http://" + ip + ":80/getBackup", )
      .then(response => response.json())
      .then(json =>{
        setFiltroComplementarGyro(json.ComplementaryAlpha);
        setKalmanQAngle_Pitch(json.Pitch_Q_angle)
        setKalmanQBias_Pitch(json.Pitch_Q_bias)
        setKalmanRMeasure_Picth(json.Pitch_R_measure)
        setKalmanQAngle_Roll(json.Roll_Q_angle)
        setKalmanQBias_Roll(json.Roll_Q_bias)
        setKalmanRMeasure_Roll(json.Roll_R_measure)
        setSalvarFiltroComplementarProgresso(false)
      })
      .catch((error)=>{
        setSalvarFiltroComplementarProgresso(false)
      })
    } catch (error) {
      setSalvarFiltroComplementarProgresso(false)
    }
  }

  return (
    <>
      <div style={{display: 'flex', flexDirection: 'column', alignItems: 'center', width: '90%'}}>
        <div style={{display: 'flex', flexDirection: 'column', width: '100%', height: '40px', fontSize: '25px', alignItems: 'center', justifyContent: 'center'}}>CALIBRAÇÕES</div>
        <div style={{display: 'flex', flexDirection: 'row', gap: '20px', height: 'auto', width: '100%'}}>
          {/* Giroscopiop */}
          <form style={{width: '400px', minHeightheight: '100px'}}>
          <fieldset style={{height: '100%'}}>
            <legend>Parametros do Sensor de Angulo</legend>
            <div style={{display: "flex", flexDirection: 'column', border: 'solid 1px', padding: '5px', alignItems: 'center'}}>
              <div style={{display: "flex", flexDirection: 'row', width: '90%', gap: "10px", padding: '5px', alignItems: 'center', justifyContent: 'left'}}>
                <p>Auto Calibrar:</p>
                <input type="button" value="Iniciar Calibração" onClick={handleCalibrarClick}/>
              </div>
              <p style={{width: '90%'}}>Status:{statusCalibracao}</p>
              {
                calibracaoEmProgresso && <vaadin-progress-bar indeterminate></vaadin-progress-bar>
              }
            </div>
            <br />
            <div style={{display: "flex", flexDirection: 'column', gap: "10px", border: 'solid 1px', padding: '5px', alignItems: 'center'}}>
              Filtro Complementar:
              <div style={{display: "flex", flexDirection: 'row',width: '90%', gap: "10px", border: 'solid 1px', padding: '5px', alignItems: 'center'}}>
                <p>Alpha do Gyro:</p>
                  <input type="range" onChange={(e)=>{setFiltroComplementarGyro(parseFloat(e.target.value))}} value={filtroComplementarGyro} min={0} max={1} step={0.001}/>
                <p>{filtroComplementarGyro.toFixed(3)}%</p>
              </div>
              <div style={{display: "flex", flexDirection: 'row',width: '90%', gap: "10px", border: 'solid 1px', padding: '5px', alignItems: 'center'}}>
                <p>Alpha do Acc:</p>
                  <input type="range" onChange={(e)=>{setFiltroComplementarAcc(parseFloat(e.target.value))}} value={filtroComplementarAcc} min={0} max={1} step={0.001}/>
                <p>{filtroComplementarAcc.toFixed(3)}%</p>
              </div>
              Filtro de Kalman:
              <div style={{display: "flex", flexDirection: 'row',width: '90%', gap: "10px", border: 'solid 1px', padding: '5px', alignItems: 'center'}}>
                <p>Pitch Erro Processo:</p>
                <input type="range" onChange={(e)=>{setKalmanQAngle_Pitch(parseFloat(e.target.value))}} value={kalmanQAngle_Pitch}  min={0} max={2} step={0.001}/>
                <p>{kalmanQAngle_Pitch.toFixed(3)}%</p>
              </div>
              <div style={{display: "flex", flexDirection: 'row',width: '90%', gap: "10px", border: 'solid 1px', padding: '5px', alignItems: 'center'}}>
                <p>Pitch Erro Gyro:</p>
                <input type="range" onChange={(e)=>{setKalmanQBias_Pitch(parseFloat(e.target.value))}} value={kalmanQBias_Pitch} min={0} max={2} step={0.001}/>
                <p>{kalmanQBias_Pitch.toFixed(3)}%</p>
              </div>
              <div style={{display: "flex", flexDirection: 'row',width: '90%', gap: "10px", border: 'solid 1px', padding: '5px', alignItems: 'center'}}>
                <p>Pitch Ruido:</p>
                <input type="range" onChange={(e)=>{setKalmanRMeasure_Picth(parseFloat(e.target.value))}} value={kalmanRMeasure_Pitch} min={0} max={2} step={0.001}/>
                <p>{kalmanRMeasure_Pitch.toFixed(3)}%</p>
              </div>

              <div style={{display: "flex", flexDirection: 'row',width: '90%', gap: "10px", border: 'solid 1px', padding: '5px', alignItems: 'center'}}>
                <p>Roll Erro Processo:</p>
                <input type="range" onChange={(e)=>{setKalmanQAngle_Roll(parseFloat(e.target.value))}} value={kalmanQAngle_Roll}  min={0} max={2} step={0.001}/>
                <p>{kalmanQAngle_Roll.toFixed(3)}%</p>
              </div>
              <div style={{display: "flex", flexDirection: 'row',width: '90%', gap: "10px", border: 'solid 1px', padding: '5px', alignItems: 'center'}}>
                <p>Roll Erro Gyro:</p>
                <input type="range" onChange={(e)=>{setKalmanQBias_Roll(parseFloat(e.target.value))}} value={kalmanQBias_Roll} min={0} max={2} step={0.001}/>
                <p>{kalmanQBias_Roll.toFixed(3)}%</p>
              </div>
              <div style={{display: "flex", flexDirection: 'row',width: '90%', gap: "10px", border: 'solid 1px', padding: '5px', alignItems: 'center'}}>
                <p>Roll Ruido:</p>
                <input type="range" onChange={(e)=>{setKalmanRMeasure_Roll(parseFloat(e.target.value))}} value={kalmanRMeasure_Roll} min={0} max={2} step={0.001}/>
                <p>{kalmanRMeasure_Roll.toFixed(3)}%</p>
              </div>
              {
                salvarFiltroComplementarProgresso && <vaadin-progress-bar indeterminate></vaadin-progress-bar>
              }
              <input type="button" value="Salvar" onClick={handleSalvarFiltroComplementar} style={{width: '50%'}}/>
              <input type="button" value="Atualizar" onClick={handleAtualizarFiltroComplementar} style={{width: '50%'}}/>
            </div>
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