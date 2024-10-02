import React, {useState, useContext, useEffect} from 'react'
import '@vaadin/progress-bar';
import PIDChart from '../PIDChart/PIDChart';

const PIDData = ({title, pidData, pidParamDataUpdate, pidParamDataSave, enableFetchData, chartData, save, load}) => {
    const [kPValue, setkPValue] = useState(0)
    const [kIValue, setkIValue] = useState(0)
    const [kDValue, setPkDValue] = useState(0)
    const [enableChart, setEnableChart] = useState(false)
    const [pitchPidProgresso, setPitchPidProgresso] = useState(false)
    const [showSP, setShowSP] = useState(true)
    const [showOutput, setShowOutput] = useState(true)
    const [zoomOutClick, setZoomOutClick] = useState(false)
    const [resetChartData, setResetChartData] = useState(false)

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
      kP: formData.get("kP"),
      kI: formData.get("kI"),
      kD: formData.get("kD"),
    };

    pidParamDataSave(data)

    setPitchPidProgresso(false)
  }

  useEffect(() => {
    enableFetchData(enableChart)
  }, [enableChart])

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
        </fieldset>
        </form>
        <form>
            <fieldset style={{width: '120px'}}>
                <legend>Chart</legend>
                <label htmlFor="chart-enable">
                    Enable:<input type="checkbox" checked={enableChart} onChange={()=>{setEnableChart((e)=>!enableChart)}}/>
                </label>
                <br />
                <label htmlFor="chart-enable">
                    Show SP:<input type="checkbox" checked={showSP} onChange={()=>{setShowSP((e)=>!showSP)}}/>
                </label>
                <br />
                <label htmlFor="chart-enable">
                    Show Output:<input type="checkbox" checked={showOutput} onChange={()=>{setShowOutput((e)=>!showOutput)}}/>
                </label>
                <label htmlFor="chart-zoom-out">
                    <input type="button" onClick={save} value={"Save"}/>
                    <input type="button" onClick={load} value={"Load"}/>
                    <input type="button" onClick={()=>setResetChartData(!resetChartData)} value={"Reset"}/>
                </label>
                <label htmlFor="chart-zoom-out">
                    <input type="button" onMouseDown={()=>setZoomOutClick(true)} onMouseUp={()=>setZoomOutClick(false)} value={"Zoom Out"} />
                </label>
            </fieldset>
        </form>
        <PIDChart enable={enableChart} data={chartData} zoomOut={zoomOutClick} maxElements={300} reset={resetChartData}/>
    </div>
  )
}

export default PIDData