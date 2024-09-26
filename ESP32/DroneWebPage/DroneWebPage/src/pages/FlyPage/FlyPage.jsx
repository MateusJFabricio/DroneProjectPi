import React, { createRef, useContext, useEffect, useRef, useState }  from 'react'
import '@vaadin/progress-bar';
import './FlyPage.css'
import Map from '../../components/Maps/Map'
import GLPViewer from '../../components/GLPViewer/GLPViewer'
import {ApiContext} from '../../context/ApiContext'
import GraficoPequeno from '../../components/GraficoPequeno/GraficoPequeno'

const FlyPage = () => {
  const inputRef = createRef("192.168.1.46");
  const {setUrl, connectionStatus, sendMessage, ip} = useContext(ApiContext);
  const connectionStatusRef = useRef();
  const chartData = useRef([])
  const [data, setData] = useState([])
  const [mode, setMode] = useState('0'); 
  const [rebootProgress, setRebootProgress] = useState(1)
  const [currentFlightMode, setCurrentFlightMode] = useState({
    Angle: false,
    Acro: false,
    EscCalibration: false,
    GyroCalibration: false,
    GyroAnalisys: false,
    ReturnHome: false,
  })

  useEffect(() => {
    if (currentFlightMode.Angle){
      setMode('1')
    }else if (currentFlightMode.Acro){
      setMode('2')
    }else if (currentFlightMode.EscCalibration){
      setMode('3')
    }else if (currentFlightMode.GyroCalibration){
      setMode('4')
    }else if (currentFlightMode.GyroAnalisys){
      setMode('5')
    }else if (currentFlightMode.ReturnHome){
      setMode('6')
    }else{
      setMode('0')
    }
    
  }, [currentFlightMode])
  

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

  useEffect(() => {
    connectionStatusRef.current = connectionStatus;
    let interval = null
    if(connectionStatus === 'Open'){
      interval = setInterval(()=>{
        if (connectionStatusRef.current === 'Open'){
          fetch("http://" + ip + ":80/getValues")
          .then(response => response.json())
          .then(dados => {
            chartData.current = dados;
            setOrientation(dados)
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

  useEffect(()=>{
    let dataTemp = chartData.current;
    while (dataTemp.length > 5){
      dataTemp.shift();
    }
    setData(dataTemp)
  }, [chartData.current])

  const handleModeClick = () => {
    let jsonState = {
      Angle: mode === '1',
      Acro: mode === '2',
      EscCalibration: mode === '3',
      GyroCalibration: mode === '4',
      GyroAnalisys: mode === '5',
      ReturnHome: mode === '6',
    }

    setCurrentFlightMode(jsonState)
    console.log(currentFlightMode)
    console.log(mode)
    if (connectionStatus === 'Open'){
      try {
        fetch("http://" + ip + ":80/postFlightMode", {
          method: "POST",
          body: JSON.stringify(jsonState),
          headers: {
            "Content-type": "text/plain;"
          }
        })
        .then((response) => response.json())
        .catch(()=>{
          setFormMesssage('Houve um erro ao salvar os dados')
        })
      } catch (error) {
        setFormMesssage('Houve um erro ao salvar os dados')
      }
    }
  }
  const handleRebootClick = ()=>{
    if (connectionStatus === 'Open'){
      try {
        setRebootProgress(0);
        fetch("http://" + ip + ":80/postReboot", {
          method: "POST",
          body: "",
          headers: {
            "Content-type": "text/plain;"
          }
        })
      } catch (error) {
        console.log('Houve um erro ao salvar os dados')
      }
    }
  }

  useEffect(() => {
    let timerout = null
    if (rebootProgress < 1){
      timerout = setTimeout(() => {
        if (rebootProgress < 1){
          setRebootProgress((e)=>e+=0.1)
        }
        console.log('to aqui')
      }, 1000);
    }
    return () =>{
      if (timerout !==null){
        clearTimeout(timerout)
      }
    }
    
  }, [rebootProgress])  
  
  const timeReboot = ()=>{
    setTimeout(() => {
      setRebootProgress((e)=>e+=0.2)
      if (rebootProgress < 1){
        timeReboot();
      }
      console.log('to aqui')
    }, 1000);
  }

  return (
    <div className='flypage-container'>
      <div className='title'>
        NAVEGAÇÃO
      </div>
      <div className='flypage-map-container'>
        <div style={{display: 'flex', flexDirection: 'row', height: '50px', width: '100%', border: 'solid 1px', alignItems: 'center', gap: '10px', padding: '2px 5px 2px 5px'}}>
          <div style={{display: 'flex', flexDirection: 'row' ,gap: '10px', width: '90%'}}>
            Flight Mode - {mode}
            <select onChange={e=>setMode(e.target.value)} name="mode" id="mode">
              <option value="0">Nenhum</option>
              <option value="1">Angle</option>
              <option value="2">Acro</option>
              <option value="3">Esc Calibration</option>
              <option value="4">Gyro Calibration</option>
              <option value="5">Gyro Analise</option>
            </select>
            <button onClick={handleModeClick}>Confirmar</button>
          </div>
          <div style={{display: 'flex', flexDirection: 'column', width: '10%', padding: '2px', justifyContent: 'center', alignItems: 'center'}}>
            <button style={{color: 'red',}} onClick={handleRebootClick}>Reboot Drone</button>
            <vaadin-progress-bar value={rebootProgress}></vaadin-progress-bar>
          </div>
        </div>
        <div className='glp-viewer'>
          <GLPViewer Pitch={orientation.Pitch} Yaw={orientation.Yaw} Roll={orientation.Roll} step={0.1}/>
          <div style={{width: "30%", height: "100%", padding: "5px", backgroundColor: "white"}}>
            <GraficoPequeno chartTitle={'Orientação'} axisName={'name'} data={data}/>
          </div>
        </div>
        <div class='map-map'>
          <Map latitude={-25.553483} longitute={-49.200533} latitudeObj={-25.55163933661852} longituteObj={-49.20139214196535}/>
        </div>
        
      </div>
    </div>
  )
}

export default FlyPage