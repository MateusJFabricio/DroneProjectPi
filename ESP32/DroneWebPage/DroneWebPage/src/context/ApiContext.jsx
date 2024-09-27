import { createContext, useRef, useState, useEffect, useContext } from "react";
import useWebSocket, { ReadyState } from 'react-use-websocket';
import {JoystickContext} from './JoystickContext'

export const ApiContext = createContext();

export const ApiContextProvider = ({children})=>{
    const {joystick} = useContext(JoystickContext);
    const joystickRef = useRef(joystick)
    const connectionStatusRef = useRef('Closed')
    const [socketUrl, setSocketUrl] = useState("wss://" + (localStorage.getItem('IP_DRONE') || 'drone') + ":81/");
    const [ip, setIp] = useState((localStorage.getItem('IP_DRONE') || 'drone'));
    const enableDrone = useRef(false);
    const stopDrone = useRef(false);

    const { sendMessage, lastMessage, readyState } = useWebSocket(socketUrl, {
        onError: (event)=>{
        },
        shouldReconnect: (closeEvent) => {
          return true;
        },
        heartbeat: true,
        reconnectAttempts: 2,
        reconnectInterval: 1000,
      });
    
    const connectionStatus = {
        [ReadyState.CONNECTING]: 'Connecting',
        [ReadyState.OPEN]: 'Open',
        [ReadyState.CLOSING]: 'Closing',
        [ReadyState.CLOSED]: 'Closed',
        [ReadyState.UNINSTANTIATED]: 'Uninstantiated',
      }[readyState];

    const setUrl = (ip)=>{
      localStorage.setItem('IP_DRONE', ip);
      setSocketUrl("ws://"+ ip + ":81");
      setIp(ip);
    }
    
    const controlData = ()=>{
      //Enable
      if(joystickRef.current.buttons[5].pressed){
        enableDrone.current = true;
        joystickRef.current.vibrationActuator.playEffect("dual-rumble", {
          startDelay: 0,
          duration: 200,
          weakMagnitude: 1.0,
          strongMagnitude: 1.0,
        });
      }

      if(joystickRef.current.buttons[4].pressed){
        enableDrone.current = false;
        joystickRef.current.vibrationActuator.playEffect("dual-rumble", {
          startDelay: 0,
          duration: 200,
          weakMagnitude: 1.0,
          strongMagnitude: 1.0,
        });
      }
      console.log("YAW: " + enableDrone.current)
      return {
        YAW: joystickRef.current.axes[0],
        PITCH: joystickRef.current.axes[3] * -1,
        ROLL: joystickRef.current.axes[2],
        TROTLE: joystickRef.current.axes[1] * -1,
        ENABLE: enableDrone.current,
        STOP: stopDrone.current
      }
    }

    useEffect(() => {
      const interval = setInterval(()=>{
        //console.log("rodando...")
        if(joystickRef.current !== null){
          if (connectionStatusRef.current === 'Open'){
            let ctrlData = controlData();
            sendMessage(JSON.stringify(ctrlData));
          }
        }
      }, 50);
      
      return () =>{ 
        clearInterval(interval)
      }
    }, [])

    useEffect(() => {
      const interval = setInterval(()=>{
        if (connectionStatusRef.current === 'Open'){
          sendMessage(JSON.stringify({lifebit: true}))
        }
      }, 1000);
      
      return () =>{ 
        clearInterval(interval)
      }
    }, [])  

    useEffect(() => {
      joystickRef.current = joystick;
    }, [joystick])

    useEffect(() => {
      connectionStatusRef.current = connectionStatus;
    }, [connectionStatus])

    return(
        <ApiContext.Provider value={{setUrl, connectionStatus, ip}}>
            {children}
        </ApiContext.Provider >
    )
}