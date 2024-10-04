import React, {useContext, useEffect, useState } from 'react'
import './ControlePage.css'
import PS4Joystick from '../../components/Ps4Joystick/PS4Joystick'
import JoystickBar from '../../components/JoystickBar/JoystickBar'
import {JoystickContext} from '../../context/JoystickContext'


const ControlePage = () => {

  const {joystick} = useContext(JoystickContext);
  const [yaw, setYaw] = useState(1500)
  const [pitch, setPitch] = useState(1500)
  const [roll, setRoll] = useState(1500)
  const [trotle, setTrotle] = useState(1500)

  function map_range(value, low1, high1, low2, high2) {
    return Math.trunc(low2 + (high2 - low2) * (value - low1) / (high1 - low1));
  }

  useEffect(()=>{
    if (joystick !== 'undefined' && joystick !== null){
      setYaw( map_range(joystick.axes[0], -1, 1, 1000, 2000));
      setPitch( map_range(joystick.axes[3] * -1, -1, 1, 1000, 2000));
      setRoll( map_range(joystick.axes[2], -1, 1, 1000, 2000));
      setTrotle( map_range(joystick.axes[1] * -1, -1, 1, 1000, 2000));
      
    }
}, [joystick])

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
          <PS4Joystick joystick={joystick}/>
        </div>
      </div>
      <div id='controlepage-joystickbars'>
          <JoystickBar title={"ROLL"} value={roll} min={1000} max={2000} color={"red"}/>
          <JoystickBar title={"PITCH"} value={pitch} min={1000} max={2000} color={"green"}/>
          <JoystickBar title={"YAW"} value={yaw} min={1000} max={2000} color={"yellow"}/>
          <JoystickBar title={"TROTTLE"} value={trotle} min={1000} max={2000} color={"blue"}/>
      </div>
      
    </div>
  )
}

export default ControlePage