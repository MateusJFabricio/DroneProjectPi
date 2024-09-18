import { Stats, OrbitControls, Circle } from '@react-three/drei'
import { Canvas, useLoader } from '@react-three/fiber'
import { GLTFLoader } from 'three/addons/loaders/GLTFLoader'


const GLPViewer = ({Yaw, Pitch, Roll}) => {
    const gltf = useLoader(GLTFLoader, '/Drone3d.gltf')
    const degToRad = (degrees)=>{
        return degrees * Math.PI / 180.0;
    };
    return (
      <Canvas camera={{ position: [-15, 0, 0] }} shadows>
        <directionalLight
          position={[3.3, 1.0, 4.4]}
          castShadow
          intensity={Math.PI * 2}
        />
        <primitive
          object={gltf.scene}
          position={[0.4, -0.8, 0]}
          //rotation={[degToRad(Yaw), degToRad(180 + Pitch), degToRad(Roll)]}
          rotation={[degToRad(Roll), degToRad(180 + 0), degToRad(Pitch * -1)]}
          children-0-castShadow
        />
        <OrbitControls target={[0, 0, 0]} />
        <axesHelper args={[5]} />
      </Canvas>
    )
};

export default GLPViewer;
