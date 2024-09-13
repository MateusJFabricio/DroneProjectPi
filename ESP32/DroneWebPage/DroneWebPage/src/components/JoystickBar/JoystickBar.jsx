import React, {useState, useEffect} from 'react'
import './JoystickBar.css'

const JoystickBar = ({title, value, min, max, color}) => {
    const [estiloBarra, setEstiloBarra] = useState(
      {
        backgroundColor: color, 
        width: "95%"
      });

    function map_range(value, low1, high1, low2, high2) {
        return low2 + (high2 - low2) * (value - low1) / (high1 - low1);
    }

    useEffect(() => {
      setEstiloBarra(
        {
            backgroundColor: color, 
            width: map_range(value, min, max, 0, 100) + "%",
        })
        
    }, [value])
    

  return (
    <div className='joystickbar-container'>
        <div id='joystickbar-container-title'>
            {title}
        </div>
        <div className='joystickbar-bar'>
            <span id='joystickbar-bar-span'>{value}</span>
            <div className='joystickbar-bar-movel' style={estiloBarra}></div>
        </div>
    </div>
  )
}

export default JoystickBar