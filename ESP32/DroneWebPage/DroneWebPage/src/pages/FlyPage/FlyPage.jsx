import React, { createRef, useContext, useEffect, useRef, useState }  from 'react'
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

  const [dadosTestes, setDadosTeste] = useState([{

  }])

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
      }, 1000)
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

  return (
    <div className='flypage-container'>
      <div className='title'>
        NAVEGAÇÃO
      </div>
      <div className='flypage-map-container'>
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