import React, {useState, useContext} from 'react'
import '@vaadin/progress-bar';
import './ConfigPidPage.css'
import {ApiContext} from '../../context/ApiContext'

const ConfiPidPage = () => {
  const [pitch_kP, setPitch_kP] = useState(0)
  const [pitch_kI, setPitch_kI] = useState(0)
  const [pitch_kD, setPitch_kD] = useState(0)
  const [pitchPidProgresso, setPitchPidProgresso] = useState(false)
  const {setUrl, connectionStatus, sendMessage, ip} = useContext(ApiContext);

  const formPitchPIDUpdate = ()=>{
    setPitchPidProgresso(true)
    
    try{
      fetch("http://" + ip + ":80/getPID")
      .then((response) => response.json())
      .then((json) => {
        setPitch_kP(json.Pitch_kP)
        setPitch_kI(json.Pitch_kI)
        setPitch_kD(json.Pitch_kD)
      })
      .catch((erro)=>{
        console.log(erro)
      })
    }catch{
    }finally{
      setPitchPidProgresso(false)
    }
  }

  const formSubmit = (event)=>{
    event.preventDefault();
    setPitchPidProgresso(true)
    const formData = new FormData(event.currentTarget);

    const data = {
      Pitch_kP: formData.get("kP"),
      Pitch_kI: formData.get("kI"),
      Pitch_kD: formData.get("kD"),
    };
    
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
    }finally{
      setPitchPidProgresso(false)
    }
  }

  return (
    <div className='configpidpage-container'>
      <form onSubmit={formSubmit}>
        <fieldset>
          <legend>Pitch PID</legend>
          <p>
            <label htmlFor="kP">
              kP: <input type="text" id="kP" name='kP' value={pitch_kP} onChange={(e)=> setPitch_kP(e.target.value)}/>
            </label>
          </p>
          <p>
            <label htmlFor="kI">
                kI: <input type="text" id="kI" name='kI' value={pitch_kI} onChange={(e)=> setPitch_kI(e.target.value)}/>
              </label>
          </p>
          <p>
            <label htmlFor="kD">
                kD: <input type="text" id="kD" name='kD' value={pitch_kD} onChange={(e)=> setPitch_kD(e.target.value)}/>
              </label>
          </p>
          {
            pitchPidProgresso && <vaadin-progress-bar indeterminate></vaadin-progress-bar>
          }
          <div style={{display: "flex", gap: "10px"}}>
            <input type="button" value="Atualizar" onClick={formPitchPIDUpdate}/>
            <input type="submit" value="Salvar"/>
          </div>
        </fieldset>
      </form>
    </div>
  )
}

export default ConfiPidPage