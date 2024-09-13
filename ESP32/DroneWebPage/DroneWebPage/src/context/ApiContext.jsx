import { createContext, useRef, useState, useEffect } from "react";
import useWebSocket, { ReadyState } from 'react-use-websocket';

export const ApiContext = createContext();

export const ApiContextProvider = ({children})=>{
    const [socketUrl, setSocketUrl] = useState("wss://192.168.1.45:81/");
    const [ip, setIp] = useState("192.168.1.45");
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

      useEffect(() => {
        console.log(lastMessage);
      }, [lastMessage])
      

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

    return(
        <ApiContext.Provider value={{setUrl, connectionStatus, sendMessage, ip}}>
            {children}
        </ApiContext.Provider >
    )
}