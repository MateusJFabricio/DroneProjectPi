import { useState } from 'react'
import './App.css'
import NavBar from './components/NavBar/NavBar'
import MenuLateral from './components/MenuLateral/MenuLateral'

//Rotas
import { Outlet } from 'react-router-dom'

function App() {

  return (
    <>
      <div className='container'>
        <NavBar/>
        <div className="pages">
          <MenuLateral/>
          <Outlet/>
        </div>
      </div>
    </>
  )
}

export default App
