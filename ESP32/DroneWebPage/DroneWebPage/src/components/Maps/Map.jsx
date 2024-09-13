import React from "react";
import { MapContainer, Marker, Popup, TileLayer, useMap } from "react-leaflet";
import './Map.css'
import DroneUrl from "../../assets/drone-merker.png";
import TargetUrl from "../../assets/target-merker.png";

export default function Map({latitude, longitute, latitudeObj, longituteObj}) {
    const droneIcon = L.icon({
        iconUrl: DroneUrl,
        iconSize: [20, 20],
        iconAnchor: [25, 50],
      });

    const targetIcon = L.icon({
        iconUrl: TargetUrl,
        iconSize: [20, 20],
        iconAnchor: [25, 50],
      });

  return (
    <MapContainer center={[latitude, longitute]} zoom={17} scrollWheelZoom={true}>
      <TileLayer
        attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
        url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
      />
      <Marker position={[latitude, longitute]} icon={droneIcon}>
        <Popup>
          {latitude + ", " + longitute}
        </Popup>
      </Marker>
      <Marker position={[latitudeObj, longituteObj]} icon={targetIcon} draggable={true} >
        <Popup>
            {latitudeObj + ", " + longituteObj}
        </Popup>
      </Marker>
    </MapContainer>
  );
}