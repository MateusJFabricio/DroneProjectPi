import React, {useEffect, useContext, useState, useRef} from 'react'
import './ConfigGeralPage.css'
import {ApiContext} from '../../context/ApiContext'

const ConfigGeralPage = () => {
  const {setUrl, connectionStatus, sendMessage, ip} = useContext(ApiContext);
  const [motor1, setMotor1] = useState("")
  const [motor2, setMotor2] = useState("")
  const [motor3, setMotor3] = useState("")
  const [motor4, setMotor4] = useState("")
  const [trotleLimit, setTrotleLimit] = useState("")
  const [formMesssage, setFormMesssage] = useState("Atualizando....")
  const [formTrotleLimitMessage, setformTrotleLimitMessage] = useState("Atualizando....")
  const [trotleIncremental, setTrotleIncremental] = useState(false)
  const [trotleIncremento, setTrotleIncremento] = useState(0)

  useEffect(() => {
      formMotorsGainUpdate();
      formLimiteTrotleUpdate();
  }, [connectionStatus])
  

  const formSubmit = (event)=>{
    event.preventDefault();

    const formData = new FormData(event.currentTarget);
    setFormMesssage('')
    //Valida os dados
    let m1 = formData.get("motor_1")
    if (m1 === '' || m1 === 'Atualizando...'){
      setFormMesssage('Valor invalido para o Motor 1')
      return;
    }

    let m2 = formData.get("motor_2")
    if (m2 === '' || m2 === 'Atualizando...'){
      setFormMesssage('Valor invalido para o Motor 2')
      return;
    }

    let m3 = formData.get("motor_3")
    if (m3 === '' || m3 === 'Atualizando...'){
      setFormMesssage('Valor invalido para o Motor 3')
      return;
    }

    let m4 = formData.get("motor_4")
    if (m4 === '' || m4 === 'Atualizando...'){
      setFormMesssage('Valor invalido para o Motor 4')
      return;
    }

    const data = {
      GANHO_M1: m1,
      GANHO_M2: m2,
      GANHO_M3: m3,
      GANHO_M4: m4
    };

    try {
      fetch("http://" + ip + ":80/postGainMotors", {
        method: "POST",
        body: JSON.stringify(data),
        headers: {
          "Content-type": "text/plain;"
        }
      })
      .then((response) => response.json())
      .then((json) => console.log(json))
      .catch(()=>{
        setFormMesssage('Houve um erro ao salvar os dados')
      })
    } catch (error) {
      setFormMesssage('Houve um erro ao salvar os dados')
    }
  }

  const formMotorsGainUpdate = () =>{
    fetch("http://" + ip + ":80/getGainMotors")
      .then(response => response.json())
      .then(data => {
        setMotor1(data.GANHO_M1);
        setMotor2(data.GANHO_M2);
        setMotor3(data.GANHO_M3);
        setMotor4(data.GANHO_M4);
        setFormMesssage('')
      })
      .catch(()=>{
        setFormMesssage('Erro ao atualizar')
      })
  }

  const formLimiteTrotleUpdate = () =>{
    fetch("http://" + ip + ":80/getTrotleLimit")
    .then(response => response.json())
    .then(data => {
      setTrotleLimit(data.TROTLE_LIMIT);
      setformTrotleLimitMessage('');
    })
    .catch(()=>{
      setformTrotleLimitMessage('Erro ao atualizar');
    })

    fetch("http://" + ip + ":80/getFlightParameters")
    .then(response => response.json())
    .then(data => {
      setTrotleIncremental(data.Trotle_Incremental);
      setTrotleIncremento(data.Trotle_Incremento)
      setformTrotleLimitMessage('');
    })
    .catch(()=>{
      setformTrotleLimitMessage('Erro ao atualizar');
    })
  }

  const formLimiteTrotleSubmit = (event) =>{
    event.preventDefault();
    
    const formData = new FormData(event.currentTarget);
    setformTrotleLimitMessage('')
    
    let limit = formData.get("trotle_limit")
    if (limit <= 0){
      setformTrotleLimitMessage('O limite não pode ser 0');
      return;
    }

    if (limit > 100){
      setformTrotleLimitMessage('O limite não pode ser maior que 100%');
      return;
    }

    if (limit === ''){
      setformTrotleLimitMessage('O limite deve ser preenchido!');
      return;
    }
    const dataLimit = {
      TROTLE_LIMIT: limit
    }

    const dataTrotleConfig = {
      Trotle_Incremental: formData.get("checkbox_incremental"),
      Trotle_Incremento: formData.get("trotle_incremento")
    }

    try{
      fetch("http://" + ip + ":80/postTrotleLimit", {
        method: "POST",
        body: JSON.stringify(dataLimit),
        headers: {
          "Content-type": "text/plain;"
        }
      })
      .then((response) => response.json())
      .catch(()=>{
        setformTrotleLimitMessage('Houve um erro ao salvar os dados')
      })

      fetch("http://" + ip + ":80/postFligthParam", {
        method: "POST",
        body: JSON.stringify(dataTrotleConfig),
        headers: {
          "Content-type": "text/plain;"
        }
      })
      .then((response) => response.json())
      .catch(()=>{
        setformTrotleLimitMessage('Houve um erro ao salvar os dados')
      })
    }catch{
      setformTrotleLimitMessage('Houve um erro ao salvar os dados')
    }

  }
  return (
    <div>
      <form onSubmit={formSubmit}>
        <fieldset>
          <legend>Ganho dos motores</legend>
          <p style={{color: "red"}}>*O comando do motor no ESP vai de 1000 a 2000</p>
          <p style={{color: "red"}}>*Os valores a seguir são fatores de multiplicação</p>
          <p>
            <label htmlFor="motor_1">
              Motor 1: <input type="text" id="motor_1" name='motor_1' value={motor1} onChange={(e)=> setMotor1(e.target.value)}/>
            </label>
          </p>
          <p>
            <label htmlFor="motor_2">
                Motor 2: <input type="text" id="motor_2" name='motor_2' value={motor2} onChange={(e)=> setMotor2(e.target.value)}/>
              </label>
          </p>
          <p>
            <label htmlFor="motor_3">
                Motor 3: <input type="text" id="motor_3" name='motor_3' value={motor3} onChange={(e)=> setMotor3(e.target.value)}/>
              </label>
          </p>
          <p>
            <label htmlFor="motor_4">
                Motor 4: <input type="text" id="motor_4" name='motor_4' value={motor4} onChange={(e)=> setMotor4(e.target.value)}/>
              </label>
          </p>
          <p style={{color: "red"}}>
            {formMesssage}
          </p>
          <div style={{display: "flex", gap: "10px"}}>
            <input type="button" value="Atualizar" onClick={formMotorsGainUpdate}/>
            <input type="submit" value="Salvar"/>
          </div>
        </fieldset>
      </form>
      <form onSubmit={formLimiteTrotleSubmit}>
        <fieldset>
          <legend>Configurações Trotle</legend>
          <p>
            <label htmlFor="trotle_config">
              <p>
                Incremental:
                <input type="checkbox" name="checkbox_incremental" checked={trotleIncremental} onClick={(e)=>setTrotleIncremental(!trotleIncremental)} />
              </p>
              <p>
                Incremento:
                <input type="text" name="trotle_incremento" value={trotleIncremento} onChange={(e)=> setTrotleIncremento(e.target.value)} />
              </p>
              
            </label>
          </p>
          <p style={{color: "red"}}>*Range de 0 até 1</p>
          <p>
            <label htmlFor="trotle_limit">
              Limite: 
              <input type="range" id="trotle_limit" name='trotle_limit' min={0} max={1} step={0.01} value={trotleLimit} onChange={(e)=> setTrotleLimit(e.target.value)}/> 
              {trotleLimit}%
            </label>
          </p>
          <p style={{color: "red"}}>
            {formTrotleLimitMessage}
          </p>
          <div style={{display: "flex", gap: "10px"}}>
            <input type="button" value="Atualizar" onClick={formLimiteTrotleUpdate}/>
            <input type="submit" value="Salvar"/>
          </div>
        </fieldset>
      </form>
    </div>
  )
}

export default ConfigGeralPage