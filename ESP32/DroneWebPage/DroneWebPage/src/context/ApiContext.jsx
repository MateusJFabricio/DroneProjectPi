import { createContext, useRef } from "react";

export const ApiContext = createContext();

export const ApiContextProvider = ({children})=>{
    const URL_API = useRef("http://localhost:3000/")

    return(
        <ApiContext.Provider value={{URL_API}}>
            {children}
        </ApiContext.Provider >
    )
}