import React, {PureComponent} from 'react'
import {RadarChart, PolarGrid, PolarAngleAxis, PolarRadiusAxis, Radar, Tooltip, Legend, ResponsiveContainer, ReferenceLine} from 'recharts';

const GraficoPequeno = ({chartTitle, axisName, data}) => {
    data = [
        {
            source: 'X',
            Angle: data.Pitch,
            Acc: data.AccX,
            Gyro: data.GyroX
        },
        {
            source: 'Y',
            Angle: data.Roll,
            Acc: data.AccY,
            Gyro: data.GyroY
        },
        {
            source: 'Z',
            Angle: data.Yaw,
            Acc: data.AccZ,
            Gyro: data.GyroZ
        }

    ]

    
  return (
    <>
        <ResponsiveContainer width="100%" height="100%">
            <div style={{display: 'flex', justifyContent: 'center'}}>{chartTitle}</div>
            <RadarChart outerRadius={120} width={730} height={250} data={data}>
            <PolarGrid   stroke="gray"/>
            <PolarAngleAxis dataKey="source" />
            <PolarRadiusAxis angle={30} domain={[-45, 45]} stroke='black'/>
            <Radar name="Angle" dataKey="Angle" stroke="green" strokeWidth={2} fill="transparent" fillOpacity={0.6} />
            <Radar name="Acc" dataKey="Acc" stroke="blue" strokeWidth={2} fill="transparent" fillOpacity={0.6} />
            {/* <Radar name="Gyro" dataKey="Gyro" stroke="red" strokeWidth={2} fill="transparent" fillOpacity={0.6} /> */}
            <Legend />
            </RadarChart>
        </ResponsiveContainer>
    </>
  )
}

export default GraficoPequeno