import React, {useState, useContext, useEffect, useRef} from 'react'
import '@vaadin/progress-bar';
import './ConfigPidPage.css'
import {ApiContext} from '../../context/ApiContext'
import PIDData from '../../components/PIDData/PIDData';
import { saveAs } from 'file-saver';
import { useFilePicker } from 'use-file-picker';

const ConfiPidPage = () => {
  const enableChartDataPitch = useRef(false)
  const enableChartDataRoll = useRef(false)
  const pitchLoadData = useRef(false)
  const rollLoadData = useRef(false)
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
  const [pitchChartData, setPitchChartData] = useState([])
  const [rollChartData, setRollChartData] = useState([])

  const { openFilePicker, filesContent, loading } = useFilePicker({
    accept: '.json',
    onFilesSuccessfullySelected: ({ plainFiles, filesContent }) => {
      if (pitchLoadData.current){
        setPitchPidData(JSON.parse(filesContent[0].content));
      }
      if (rollLoadData.current){
        setRollPidData(JSON.parse(filesContent[0].content));
      }
      pitchLoadData.current = false;
      rollLoadData.current = false;
    },
  });

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
  const formPitchSubmit = (data)=>{ 
      
    try{
      fetch("http://" + ip + ":80/postPID", {
        method: "POST",
        body: JSON.stringify({
          Pitch_kP: data.kP,
          Pitch_kI: data.kI,
          Pitch_kD: data.kD
        }),
        headers: {
          "Content-type": "text/plain;"
        }
      })
      .then((response) => response.json())
      
    }catch(erro){
      console.log(erro)
    }
  }
  const formRollSubmit = (data)=>{    
    try{
      fetch("http://" + ip + ":80/postPID", {
        method: "POST",
        body: JSON.stringify({
          Roll_kP: data.kP,
          Roll_kI: data.kI,
          Roll_kD: data.kD
        }),
        headers: {
          "Content-type": "text/plain;"
        }
      })
      .then((response) => response.json())
      
    }catch(erro){
      console.log(erro)
    }
  }
  const pitchChartSaveData = ()=>{
    saveJson("BackupPitchPIDData", pitchChartData);
  }
  const rollChartSaveData = ()=>{
    saveJson("BackupRollPIDData", rollChartData);
  }
  const pitchChartLoadData = ()=>{
    pitchLoadData.current = true;
    openFilePicker()
  }
  const rollChartLoadData = ()=>{
    rollLoadData.current = true;
    openFilePicker()
  }
  const saveJson = (fileName, jsonData)=>{
    if (jsonData !== null && jsonData !== ''){
      var blob = new Blob([JSON.stringify(jsonData)], {type: "application/json"});
      const date = new Date();
      let dateTime = String(date.getFullYear());
      dateTime += String(date.getMonth() + 1).padStart(2, '0');
      dateTime += String(date.getDate()).padStart(2, '0');
      dateTime += '_';
      dateTime += String(date.getHours()).padStart(2, '0');
      dateTime += String(date.getMinutes()).padStart(2, '0');
  
      saveAs(blob, fileName + "_"+ dateTime +".json");
    }
  }
  const enableFetchDataPitch = (value)=>{
    enableChartDataPitch.current = value;
  }
  const enableFetchDataRoll = (value)=>{
    enableChartDataRoll.current = value;
  }
  useEffect(() => {
    const interval = setInterval(()=>{
      if (connectionStatus === 'Open'){
        if (enableChartDataPitch.current || enableChartDataRoll.current){
          try{
            fetch("http://" + ip + ":80/getPIDTrace")
            .then((response) => response.json())
            .then((json) => {
              if (enableChartDataPitch.current){
                setPitchChartData([{
                  SP: json.Pitch_Setpoint,
                  Output: json.Pitch_Output,
                  Actual: json.Pitch_Actual,
                  PidTimeSinceLastCall_Millis: json.PidTimeSinceLastCall_Millis,
                  MinMax: [json.Pitch_PIDMin, json.Pitch_PIDMax],
                  kP: json.Pitch_PIDkP,
                  kI: json.Pitch_PIDkI,
                  kD: json.Pitch_PIDkD
                }])
              }
              
              if (enableChartDataRoll.current){
                setRollChartData([{
                  SP: json.Roll_Setpoint,
                  Output: json.Roll_Output,
                  Actual: json.Roll_Actual,
                  PidTimeSinceLastCall_Millis: json.PidTimeSinceLastCall_Millis,
                  Min: json.Roll_PIDMin,
                  Max: json.Roll_PIDMax
                }])
              }
            })
            .catch((erro)=>{
              console.log(erro)
            })
          }catch{
          }finally{
          }
        }
      }
    }, 50)

    return ()=>{
      clearInterval(interval)
    }
  }, [connectionStatus])
  
  return (
    <div style={{display: 'flex', flexDirection: 'column', gap: '20px', borderBottom: 'solid 1px', padding: '5px', width: '100%'}}>
      <div className='title'>
        CONFIGURAÇÃO DOS PIDs
      </div>
      <PIDData 
        title={"Pitch PID"} 
        pidData={pitchPidData} 
        pidParamDataUpdate={PIDDataUpdate} 
        pidParamDataSave={formPitchSubmit} 
        enableFetchData={enableFetchDataPitch} 
        chartData={pitchChartData} 
        save={pitchChartSaveData} 
        load={pitchChartLoadData}
      />
      <PIDData 
        title={"Roll PID"} 
        pidData={rollPidData} 
        pidParamDataUpdate={PIDDataUpdate} 
        pidParamDataSave={formRollSubmit} 
        enableFetchData={enableFetchDataRoll} 
        chartData={rollChartData} 
        save={rollChartSaveData} 
        load={rollChartLoadData}
      />
    </div>
  )
}

export default ConfiPidPage