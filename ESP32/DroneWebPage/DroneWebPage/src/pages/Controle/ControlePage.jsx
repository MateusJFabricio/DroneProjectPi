import React, {useContext, useEffect, useState} from 'react'
import './ControlePage.css'
import PS4Joystick from '../../components/Ps4Joystick/PS4Joystick'
import JoystickBar from '../../components/JoystickBar/JoystickBar'
import {JoystickContext} from '../../context/JoystickContext'

const ControlePage = () => {

  const {joystick} = useContext(JoystickContext);
  const [yaw, setYaw] = useState(500)
  const [pitch, setPitch] = useState(500)
  const [roll, setRoll] = useState(500)
  const [trotle, setTrotle] = useState(500)

  function map_range(value, low1, high1, low2, high2) {
    return Math.trunc(low2 + (high2 - low2) * (value - low1) / (high1 - low1));
  }

  useEffect(()=>{
    if (joystick !== null){
      setYaw( map_range(joystick.axes[2], -1, 1, 48, 2047));
      setPitch( map_range(joystick.axes[3] * -1, -1, 1, 48, 2047));
      setRoll( map_range(joystick.axes[0], -1, 1, 48, 2047));
      setTrotle( map_range(joystick.axes[1] * -1, -1, 1, 48, 2047));
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
          <JoystickBar title={"YAW"} value={yaw} min={48} max={2047} color={"red"}/>
          <JoystickBar title={"PITCH"} value={pitch} min={48} max={2047} color={"green"}/>
          <JoystickBar title={"ROLL"} value={roll} min={48} max={2047} color={"yellow"}/>
          <JoystickBar title={"TROTTLE"} value={trotle} min={48} max={2047} color={"blue"}/>
      </div>
    </div>
  )
}

export default ControlePage