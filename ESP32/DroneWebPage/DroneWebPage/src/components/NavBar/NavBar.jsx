import React, {useState, useEffect, useContext, useRef, createRef } from 'react'
import './NavBar.css'
import Logo from "../../assets/drone.png"

import { ApiContext } from "../../context/ApiContext";
import { JoystickContext } from "../../context/JoystickContext";

const NavBar = () => {
  const inputRef = createRef("drone");
  const {setUrl, connectionStatus, sendData, ip} = useContext(ApiContext);
  const {joystick} = useContext(JoystickContext);
  const [joystickConectado, setJoystickConectado] = useState(false);
  const connectionStatusRef = useRef(connectionStatus);
  const [statusDrone, setStatusDrone] = useState({
    DRONE_ARMADO: false,
    Angle: false,
    Acro: false,
    EscCalibration: false,
    GyroCalibration: false,
    GyroAnalisys: false,
    ReturnHome: false,
  })

  useEffect(() => {
    inputRef.current.value = ip;
  }, [ip])

  const btnConectar = () =>{
    console.log(inputRef.current)
    setUrl(inputRef.current.value);
  };

  useEffect(()=>{
      setJoystickConectado(joystick !== null);
  }, [joystick])  

  useEffect(() => {
    const interval = setInterval(()=>{
      connectionStatusRef.current = connectionStatus;
      if (connectionStatusRef.current === 'Open'){
        fetch("http://" + ip + ":80/getDroneStatus", {
          method: "GET",
          headers: {
            "Content-type": "text/plain;"
          }
        })
        .then((response) => response.json())
        .then((json) => {
          setStatusDrone(json);
        })
      }
    }, 1000);
    
    return () =>{ 
      clearInterval(interval)
    }
  }, [connectionStatus])
  

  return (
    <div className="nav-bar">
      <div className="logo">
        <img className="logo-image" alt="Logo" src={Logo} />
        <span>Robosoft Pilot</span>
      </div>
      <div style={{display: 'flex', padding: '5px, 5px, 5px, 15px', justifyItems: 'center', gap: '5px', alignItems: 'center', width: '40%', height: '80%', fontSize: '20px'}}>
        <span>Nome Drone:</span>
        <input type="text" ref={inputRef} />
        <button onClick={btnConectar}>Conectar</button> 
      </div>
      <div className='nav-status'>
        <div className={joystickConectado ? 'nav-status-conexao conectado' : 'nav-status-conexao desconectado'}>
            <span>JOYSTICK</span>
            <span>{joystickConectado ? "CONECTADO" : "DESCONECTADO"}</span>
        </div>
        <div className={connectionStatus === "Open" ? 'nav-status-conexao conectado' : 'nav-status-conexao desconectado'}>
          <span>DRONE</span><br/>
          <span>COM: {connectionStatus}</span><br />
          <span>IP: {ip}</span>
        </div>
        <div className='statusDroneContainer'>
          <span className={statusDrone.DRONE_ARMADO ? 'statusDroneArmado' : 'statusDroneDesarmado'}>
          {statusDrone.DRONE_ARMADO ? "DRONE ARMADO" : "DRONE DESALMADO"}
          </span>
        </div>
        <div style={{display: 'flex', alignItems: 'center', justifyContent: 'center', textAlign: 'center', height: '90%', width: '120px', border: 'solid 1px', padding: '2px'}}>
          {
            statusDrone.Angle ? "MODO ANGLE" : 
            statusDrone.Acro ? "MODO ACRO" :
            statusDrone.EscCalibration ? "MODO ESC CALIB" :
            statusDrone.GyroCalibration ? "MODO GYRO CALIB" :
            statusDrone.GyroAnalisys ? "MODO GYRO ANALYSIS" :
            statusDrone.ReturnHome ? "MODO RETURN HOME" :
            "NO MODE"
          }
        </div>
      </div>
    </div>
  )
}

export default NavBar