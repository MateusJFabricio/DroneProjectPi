import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import App from './App.jsx'
import './index.css'

import {createBrowserRouter,RouterProvider} from "react-router-dom";

import { ApiContextProvider } from './context/ApiContext.jsx';

import ConexaoPage from './pages/Conexao/ConexaoPage.jsx';
import ConfiguracaoPage from './pages/ConfigPIDPage/ConfigPidPage.jsx';
import ControlePage from './pages/Controle/ControlePage.jsx';
import ConfigGeralPage from './pages/ConfigGeralPage/ConfigGeralPage.jsx';
import ConfiPidPage from './pages/ConfigPIDPage/ConfigPidPage.jsx';
import FlyPage from './pages/FlyPage/FlyPage.jsx';
import { JoystickContextProvider } from './context/JoystickContext.jsx';
import BackupPage from './pages/BackupPage/BackupPage.jsx';

const router = createBrowserRouter([
  {
    path: "/",
    element: (
      <JoystickContextProvider>
        <ApiContextProvider>
          <App/>
        </ApiContextProvider>
      </JoystickContextProvider>
      ),
    children:[
      {
        path: "/",
        element: <ConexaoPage/>
      },
      {
        path: "/Fly",
        element: <FlyPage/>
      },
      {
        path: "/Conexao",
        element: <ConexaoPage/>
      },
      {
        path: "/ConfigGeral",
        element: (
            <ConfigGeralPage/>
        )
      },
      {
        path: "/ConfigPID",
        element: (
            <ConfiPidPage/>
        )
      },
      {
        path: "/Controle",
        element: (
            <ControlePage/>
        )
      },
      {
        path: "/Backup",
        element: (
            <BackupPage/>
        )
      }
    ]
  }
]);

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <RouterProvider router={router} />
  </StrictMode>,
)
