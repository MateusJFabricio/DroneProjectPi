import React, {useState, useContext, useEffect, useRef} from 'react'
import '@vaadin/progress-bar';
import './ConfigPidPage.css'
import {ApiContext} from '../../context/ApiContext'
import PIDData from '../../components/PIDData/PIDData';

const ConfiPidPage = () => {
  const enableChartDataPitch = useRef(false)
  const enableChartDataRoll = useRef(false)
  const [pitchPidData, setPitchPidData] = useState({
    P: 0,
    I: 0,
    D: 0
  })
  const [rollPidData, setRollPidData] = useState({
    P: 0,
    I: 0,
    D: 0
  })

  const {setUrl, connectionStatus, sendMessage, ip} = useContext(ApiContext);

  const PIDDataUpdate = ()=>{
    
    try{
      fetch("http://" + ip + ":80/getPID")
      .then((response) => response.json())
      .then((json) => {
        setPitchPidData({
          P: json.Pitch_kP,
          I: json.Pitch_kI,
          D: json.Pitch_kD
        })

        setRollPidData({
          P: json.Roll_kP,
          I: json.Roll_kI,
          D: json.Roll_kD
        })
      })
      .catch((erro)=>{
        console.log(erro)
      })
    }catch{
    }finally{
    }
  }

  useEffect(() => {
    if (connectionStatus === 'Open'){
      PIDDataUpdate();
    }
  }, [connectionStatus])

  const formSubmit = (data)=>{    
    try{
      fetch("http://" + ip + ":80/postPID", {
        method: "POST",
        body: JSON.stringify(data),
        headers: {
          "Content-type": "text/plain;"
        }
      })
      .then((response) => response.json())
      
    }catch(erro){
      console.log(erro)
    }
  }
  const pitchChartData = ()=>{
    let valor = []
    for (let index = 0; index < 80; index++) {
      valor.push({ name: index, Output: index * 2, Actual: index * Math.PI, SP: 80 })
    }

    return valor
  }
  const rollChartData = ()=>{
    let valor = []
    for (let index = 0; index < 42; index++) {
      valor.push({ name: index, Output: index * 2, Actual: index * Math.PI, SP: 80 })
    }

    return valor
  }

  const enableFetchDataPitch = (value)=>{
    enableChartDataPitch.current = value;
  }

  const enableFetchDataRoll = (value)=>{
    enableChartDataRoll.current = value;
  }

  return (
    <div style={{display: 'flex', flexDirection: 'column', gap: '20px', borderBottom: 'solid 1px', padding: '5px', width: '100%'}}>
      <div className='title'>
        CONFIGURAÇÃO DOS PIDs
      </div>
      <PIDData title={"Pitch PID"} pidData={pitchPidData} pidDataUpdate={PIDDataUpdate} pidDataSave={formSubmit} enableFetchData={enableFetchDataPitch} chartData={pitchChartData()}/>
      <PIDData title={"Roll PID"} pidData={rollPidData} pidDataUpdate={PIDDataUpdate} pidDataSave={formSubmit} enableFetchData={enableFetchDataRoll} chartData={rollChartData()}/>
    </div>
  )
}

export default ConfiPidPage