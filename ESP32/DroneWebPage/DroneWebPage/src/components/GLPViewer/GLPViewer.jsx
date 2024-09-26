import React, {useEffect, useContext, useState, useRef, startTransition} from 'react'
import { Stats, OrbitControls, Circle } from '@react-three/drei'
import { Canvas, useLoader } from '@react-three/fiber'
import { GLTFLoader } from 'three/addons/loaders/GLTFLoader'


const GLPViewer = ({Yaw, Pitch, Roll, step}) => {
  const [solido, setSolido] = useState(null)
  //const gltf = useLoader(GLTFLoader, '/Drone3d.gltf')
    
  const [stepAngles, setstepAngles] = useState({
    Pitch: 0,
    Roll: 0,
    Yaw: 0
  })
  const degToRad = (degrees)=>{
      return degrees * Math.PI / 180.0;
  };
  
  useEffect(() => {
    const loader = new GLTFLoader();

    startTransition(() => {
      
      loader.load('/Drone3d.gltf', async (gltf) => {
        setSolido(gltf);
      });
    });
  }, []);
  
  useEffect(() => {
    let interval = null;
    if (stepAngles.Pitch !== Pitch ||
      stepAngles.Roll !== Roll ||
      stepAngles.Yaw !== Yaw)
      {
        interval = setInterval(()=>{
          let angles = {
            Pitch: (stepAngles.Pitch > Pitch ? stepAngles.Pitch - step : stepAngles.Pitch + step),
            Roll: (stepAngles.Roll > Roll ? stepAngles.Roll - step : stepAngles.Roll + step),
            Yaw: (stepAngles.Yaw > Yaw ? stepAngles.Yaw - step : stepAngles.Yaw + step),
          }
          
          angles.Pitch = Math.abs(angles.Pitch - Pitch) <= step ? Pitch : angles.Pitch;
          angles.Roll = Math.abs(angles.Roll - Roll) <= step ? Roll : angles.Roll;
          angles.Yaw = Math.abs(angles.Yaw - Yaw) <= step ? Yaw : angles.Yaw;

          setstepAngles(angles)
        }, 1);
      }

    return () =>{
      if(interval !== null){
        clearInterval(interval)
      }
    }
  }, [Yaw, Pitch, Roll, stepAngles])
  
  return (
    <>
      <Canvas camera={{ position: [-15, 0, 0] }} shadows>
        <directionalLight
          position={[3.3, 1.0, 4.4]}
          castShadow
          intensity={Math.PI * 2}
        />
        {solido &&
          <primitive
          object={solido.scene}
          position={[0.4, -0.8, 0]}
          //rotation={[degToRad(Yaw), degToRad(180 + Pitch), degToRad(Roll)]}
          rotation={[degToRad(stepAngles.Roll), degToRad(180 + stepAngles.Yaw * -1), degToRad(stepAngles.Pitch)]}
          children-0-castShadow
        />
        }
          
        <OrbitControls target={[0, 0, 0]} />
        <axesHelper args={[5]} rotation={[degToRad(90), 0, 0]} />
      </Canvas>
    </>
  )
};

export default GLPViewer;
