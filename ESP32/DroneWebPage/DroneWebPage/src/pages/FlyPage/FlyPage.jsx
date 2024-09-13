import React from 'react'
import './FlyPage.css'
import Map from '../../components/Maps/Map'

const FlyPage = () => {
  return (
    <div className='flypage-container'>
      <div className='title'>
        NAVEGAÇÃO
      </div>
      <div className='flypage-map-container'>
        <div id='map-title'>Mapa</div>
        <div id='map-map'>
          <Map latitude={-25.553483} longitute={-49.200533} latitudeObj={-25.55163933661852} longituteObj={-49.20139214196535}/>
        </div>
      </div>
    </div>
  )
}

export default FlyPage