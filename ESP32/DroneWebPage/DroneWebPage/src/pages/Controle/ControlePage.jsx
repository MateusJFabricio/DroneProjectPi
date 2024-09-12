import React from 'react'
import './ControlePage.css'
import PS4Joystick from '../../components/Ps4Joystick/PS4Joystick'

const ControlePage = () => {
  return (
    <div className='controlepage-container'>
      <div className='title'>CONTROLE</div>
      <div className='controle-data-container'>
        <div className='control-data'>
          <p id='control-data-container'>Joystick Data</p>
          <p>Valores aqui</p>
        </div>
        <div className='control-joystick'>
          <div id='control-joystick-title'>Joystick</div>
          <PS4Joystick/>
        </div>
      </div>
    </div>
  )
}

export default ControlePage