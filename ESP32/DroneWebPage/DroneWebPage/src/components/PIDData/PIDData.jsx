import React, {useState, useContext, useEffect} from 'react'
import '@vaadin/progress-bar';
import PIDChart from '../PIDChart/PIDChart';
import {ApiContext} from '../../context/ApiContext'

const PIDData = ({title, pidData, pidParamDataUpdate, pidParamDataSave, enableFetchData, chartData, save, load}) => {
    const {setUrl, connectionStatus, sendMessage, ip} = useContext(ApiContext);
    const [kPValue, setkPValue] = useState(0)
    const [kIValue, setkIValue] = useState(0)
    const [kDValue, setPkDValue] = useState(0)
    const [enableChart, setEnableChart] = useState(false)
    const [pitchPidProgresso, setPitchPidProgresso] = useState(false)
    const [showLimits, setShowLimits] = useState(false)
    const [showGains, setShowGains] = useState(false)
    const [showRates, setShowRates] = useState(false)
    const [zoomOutClick, setZoomOutClick] = useState(false)
    const [zoomInClick, setZoomInClick] = useState(false)
    const [resetChartData, setResetChartData] = useState(false)
    const [maxData, setMaxData] = useState(300)
    const [saveData, setSaveData] = useState(false)
    const [autoTuneProgress, setAutoTuneProgress] = useState(false)
    const [tuneMin, setTuneMin] = useState(-10)
    const [tuneMax, setTuneMax] = useState(10)

    useEffect(() => {
        setkPValue(pidData.P)
        setkIValue(pidData.I)
        setPkDValue(pidData.D)
    }, [pidData])    
    
  const formPIDUpdate = ()=>{
    setPitchPidProgresso(true)
    pidParamDataUpdate();
    setPitchPidProgresso(false)
  }

  const formSubmitPitch = (event)=>{
    event.preventDefault();
    setPitchPidProgresso(true)
    const formData = new FormData(event.currentTarget);
    const data = {
      kP: parseFloat(formData.get("kP").replace(",", ".")),
      kI: parseFloat(formData.get("kI").replace(",", ".")),
      kD: parseFloat(formData.get("kD").replace(",", ".")),
    };

    pidParamDataSave(data)

    setPitchPidProgresso(false)
  }

  useEffect(() => {
    enableFetchData(enableChart)
  }, [enableChart])

  const startAutoTune = ()=>{
    if (connectionStatus === 'Open'){
        setAutoTuneProgress(true)
        fetch("http://" + ip + ":80/postAutoTune", {
            method: "POST",
            body: JSON.stringify({
                TuneMin: tuneMin,
                TuneMax: tuneMax
            }),
            headers: {
              "Content-type": "text/plain;"
            }
          })
          .then((response) => response.json())
    }

  }

  const cancelAutoTune = ()=>{
    if (connectionStatus === 'Open'){
        
        fetch("http://" + ip + ":80/postAutoTuneCancel", {
            method: "POST",
            body: "nada",
            headers: {
              "Content-type": "text/plain;"
            }
          })
          .then((response) => setAutoTuneProgress(false))
    }
  }

  useEffect(() => {
    let interval = null
    if (autoTuneProgress){
        interval = setInterval(() => {
            fetch("http://" + ip + ":80/getAutoTuneProgress")
            .then((response) => response.json())
            .then((json) => {
                if (!json.AutoTuneRunning){
                    setAutoTuneProgress(false)
                }
            })

        }, 1000);
    }
  
    return () => {
        if(interval !== null){
            clearInterval(interval)
        }
    }
  }, [autoTuneProgress])
  

  return (
    <div style={{display: 'flex', flexDirection: 'row', width: '90%', height: '40%'}}>
        <form onSubmit={formSubmitPitch} style={{height: '90%'}}>
        <fieldset style={{height: '100%'}}>
            <legend>{title}</legend>
            <p>
            <label htmlFor="kP">
                kP: <input type="text" id="kP" name='kP' value={kPValue} onChange={(e)=> setkPValue(e.target.value)}/>
            </label>
            </p>
            <p>
            <label htmlFor="kI">
                kI: <input type="text" id="kI" name='kI' value={kIValue} onChange={(e)=> setkIValue(e.target.value)}/>
                </label>
            </p>
            <p>
            <label htmlFor="kD">
                kD: <input type="text" id="kD" name='kD' value={kDValue} onChange={(e)=> setPkDValue(e.target.value)}/>
            </label>
            </p>
            {
            pitchPidProgresso && <vaadin-progress-bar indeterminate></vaadin-progress-bar>
            }
            <div style={{display: "flex", gap: "10px"}}>
                <input type="button" value="Atualizar" onClick={formPIDUpdate}/>
                <input type="submit" value="Salvar"/>
            </div>
            <br />
            TuneMin: <input type="text" value={tuneMin} onChange={(e)=> setTuneMin(e.target.value)}/>
            <br />
            TuneMax: <input type="text" value={tuneMax} onChange={(e)=> setTuneMax(e.target.value)}/>
            <input type="button" value="Start Auto Tunning" onClick={startAutoTune}/>
            <input type="button" value="Cancel Auto Tunning" onClick={cancelAutoTune}/>
            {
                autoTuneProgress && <vaadin-progress-bar indeterminate></vaadin-progress-bar>
            }
        </fieldset>
        </form>
        <form onSubmit={(e)=>e.preventDefault()}>
            <fieldset style={{width: '120px'}}>
                <legend>Chart</legend>
                <label htmlFor="chart-enable">
                    Enable:<input type="checkbox" checked={enableChart} onChange={()=>{setEnableChart((e)=>!enableChart)}}/>
                </label>
                <br />
                <label htmlFor="chart-enable">
                    Show Limits:<input type="checkbox" checked={showLimits} onChange={()=>{setShowLimits((e)=>!showLimits)}}/>
                </label>
                <br />
                <label htmlFor="chart-enable">
                    Show Gains:<input type="checkbox" checked={showGains} onChange={()=>{setShowGains((e)=>!showGains)}}/>
                </label>
                <br />
                <label htmlFor="chart-enable">
                    Show Rate:<input type="checkbox" checked={showRates} onChange={()=>{setShowRates((e)=>!showRates)}}/>
                </label>
                <br />
                <label htmlFor="chart-maxData">
                    Max Data:<input style={{width: '40px'}} type="text" value={maxData} onChange={(e)=> setMaxData(e.target.value)}/>
                </label>
                <label htmlFor="chart-zoom-out">
                    <input type="button" onMouseDown={()=>setSaveData(true)} onMouseUp={()=>setSaveData(false)} value={"Save"}/>
                    {/* <input type="button" onClick={load} value={"Load"}/> */}
                    <input type="button" onClick={()=>setResetChartData(!resetChartData)} value={"Reset"}/>
                </label>
                <br />
                <label htmlFor="chart-zoom-out">
                    <input type="button" onMouseDown={()=>setZoomInClick(true)} onMouseUp={()=>setZoomInClick(false)} value={"Zoom In"} />
                    <input type="button" onMouseDown={()=>setZoomOutClick(true)} onMouseUp={()=>setZoomOutClick(false)} value={"Zoom Out"} />
                </label>
                <br />
            </fieldset>
        </form>
        <PIDChart enable={enableChart} save={saveData} showLimits={showLimits} showGains={showGains} showRate={showRates} data={chartData} zoomOut={zoomOutClick} zoomIn={zoomInClick} maxElements={maxData} reset={resetChartData}/>
    </div>
  )
}

export default PIDData