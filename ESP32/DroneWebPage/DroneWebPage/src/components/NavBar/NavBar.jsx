import React, {useState, useEffect, useContext, useRef } from 'react'
import './NavBar.css'
import Logo from "../../assets/drone.png"

import { ApiContext } from "../../context/ApiContext";
import { JoystickContext } from "../../context/JoystickContext";

const NavBar = () => {
  const {setUrl, connectionStatus, sendData, ip} = useContext(ApiContext);
  const {joystick} = useContext(JoystickContext);
  const [joystickConectado, setJoystickConectado] = useState(false);
  const connectionStatusRef = useRef(connectionStatus);
  const [statusDrone, setStatusDrone] = useState({
    DRONE_ARMADO: false
  })

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
    console.log("alskdaaaaaaaaa")
    
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
      <div className='nav-status'>
        <div className={joystickConectado ? 'nav-status-conexao conectado' : 'nav-status-conexao desconectado'}>
            <span>JOYSTCIK</span>
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
      </div>
    </div>
  )
}

export default NavBar