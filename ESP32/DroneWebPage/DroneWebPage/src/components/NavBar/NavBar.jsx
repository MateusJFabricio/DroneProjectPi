import React, {useState, useEffect, useContext } from 'react'
import './NavBar.css'
import Logo from "../../assets/drone.png"

import { ApiContext } from "../../context/ApiContext";
import { JoystickContext } from "../../context/JoystickContext";

const NavBar = () => {
  const [joystickConectado, setJoystickConectado] = useState(false);
  const {joystick} = useContext(JoystickContext);
  const {setUrl, connectionStatus, sendMessage, ip} = useContext(ApiContext);

  useEffect(() => {
    console.log(connectionStatus);
  }, [connectionStatus])
  

  useEffect(()=>{
      setJoystickConectado(joystick !== null);
  }, [joystick])

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
      </div>
    </div>
  )
}

export default NavBar