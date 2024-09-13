import React, {useState, useEffect, useContext } from 'react'
import './NavBar.css'
import Logo from "../../assets/drone.png"

import { ApiContext } from "../../context/ApiContext";
import { JoystickContext } from "../../context/JoystickContext";

const NavBar = () => {
  const [joystickConectado, setJoystickConectado] = useState(false);
  const {joystick} = useContext(JoystickContext);

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
          <span>DESCONECTADO</span>
        </div>
        <div className='nav-status-conexao desconectado'>
          <span>DRONE</span><br/>
          <span>DESCONECTADO</span><br />
          <span>IP: 192.168.1.45</span>
        </div>
      </div>
    </div>
  )
}

export default NavBar