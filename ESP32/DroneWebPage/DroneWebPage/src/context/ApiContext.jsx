import { createContext, useRef, useState, useEffect, useContext } from "react";
import useWebSocket, { ReadyState } from 'react-use-websocket';
import {JoystickContext} from './JoystickContext'

export const ApiContext = createContext();

export const ApiContextProvider = ({children})=>{
    const {joystick} = useContext(JoystickContext);
    const joystickRef = useRef(joystick)
    const connectionStatusRef = useRef('Closed')
    const [socketUrl, setSocketUrl] = useState("wss://192.168.1.45:81/");
    const [ip, setIp] = useState("192.168.1.45");
    const enableDrone = useRef(false);
    const stopDrone = useRef(false);

    const { sendMessage, lastMessage, readyState } = useWebSocket(socketUrl, {
        shouldReconnect: (closeEvent) => {
          return true;
        },
        heartbeat: {
            message: 'ping',
            returnMessage: 'pong',
            timeout: 3000, // 1 minute, if no response is received, the connection will be closed
            interval: 1000, // every 25 seconds, a ping message will be sent
          },
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
        YAW: joystickRef.current.axes[2],
        PITCH: joystickRef.current.axes[3] * -1,
        ROLL: joystickRef.current.axes[0],
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