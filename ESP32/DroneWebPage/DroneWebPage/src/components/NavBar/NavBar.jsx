import React from 'react'
import './NavBar.css'
import Logo from "../../assets/drone.png"

const NavBar = () => {
  return (
    <div className="nav-bar">
      <div className="logo">
        <img className="logo-image" alt="Logo" src={Logo} />
        <span>Robosoft Pilot</span>
      </div>
      <div className='nav-status'>
        <div className='nav-status-conexao'>
          <p>CONECTADO</p>
          <p>IP: 192.168.1.45</p>
        </div>
      </div>
    </div>
  )
}

export default NavBar