import React, {useState, useEffect, useRef} from 'react'
import {
    LineChart,
    Line,
    XAxis,
    YAxis,
    CartesianGrid,
    Tooltip,
    Legend,
    Bar,
    Area,
    BarChart,
    Rectangle,
    ReferenceLine,
    ComposedChart,
    ResponsiveContainer,
  } from 'recharts';

const PIDChart = ({enable, data, zoomOut, zoomIn, maxElements, reset, showGains, showLimits, showRate, save}) => {
    const dataTotal = useRef([])
    const [dataChart, setDataChart] = useState([])
    const lastIndex = useRef(0)
    const [domain, setDomain] = useState([-100, 100])
    
    useEffect(() => {
      if (reset){
        setDataChart([])
        setDomain([-100, 100])
        dataTotal.current = []
      }
     
    }, [reset])
    

    useEffect(() => {
      if (zoomIn){
        let d = domain
        if (d[1] > 25){
          setDomain([d[0] + 5, d[1] - 5])
        }else if (d[1] > 10){
          setDomain([d[0] + 2, d[1] - 2])
        }else if (d[1] > 1){
          setDomain([d[0] + 1, d[1] - 1])
        }
      }
    }, [zoomIn])

    useEffect(() => {
      if (zoomOut){
        let d = domain
        if (d[1] > 25){
          setDomain([d[0] - 5, d[1] + 5])
        }else if (d[1] > 10){
          setDomain([d[0] - 2, d[1] + 2])
        }else if (d[1] > 1){
          setDomain([d[0] - 1, d[1] + 1])
        }
      }
    }, [zoomOut])
    
    useEffect(() => {
      if (enable){
        data[0].name = lastIndex.current;
        let newData = [...dataChart, ...data]
        dataTotal.current = [...dataTotal.current, ...data];
        if (dataTotal.current.length > 5000){
          dataTotal.current = dataTotal.current.slice(5000 * -1, dataTotal.current.length);
        }
        if (newData.length > maxElements){
          newData = newData.slice(maxElements * -1, newData.length);
        }
        setDataChart(newData)
        lastIndex.current++;
      }
    }, [data])

    useEffect(() => {
      if (save){
        saveJson("DataChartLog", dataTotal.current)
      }
    
    }, [save])

    const saveJson = (fileName, jsonData)=>{
      if (jsonData !== null && jsonData !== ''){
        var blob = new Blob([JSON.stringify(jsonData)], {type: "application/json"});
        const date = new Date();
        let dateTime = String(date.getFullYear());
        dateTime += String(date.getMonth() + 1).padStart(2, '0');
        dateTime += String(date.getDate()).padStart(2, '0');
        dateTime += '_';
        dateTime += String(date.getHours()).padStart(2, '0');
        dateTime += String(date.getMinutes()).padStart(2, '0');
    
        saveAs(blob, fileName + "_"+ dateTime +".json");
      }
    }

  return (
    <ResponsiveContainer width="100%" height="100%">
        <ComposedChart
            width={500}
            height={300}
            data={dataChart}
            margin={{
            top: 5,
            right: 30,
            left: 20,
            bottom: 5,
            }}
        >
            <CartesianGrid strokeDasharray="3 3" />
            <XAxis dataKey="name" />
            <YAxis domain={domain} />
            <Tooltip />
            <Legend />
            <Line type="monotone" dataKey="Output" stroke="#8884d8" dot={false}/>
            <Line type="monotone" dataKey="Actual" stroke="#82ca9d"  dot={false} />
            <Line type="monotone" dataKey="SP"     stroke="red"  dot={false} />
            {showLimits && <Area type="monotone" dataKey="MinMax"     stroke="gray" fill="#cccccc"  dot={false} />}
            {showGains && <Line type="monotone" dataKey="kP"     stroke="Orange"  dot={false} />}
            {showGains && <Line type="monotone" dataKey="kI"     stroke="Gold"  dot={false} />}
            {showGains && <Line type="monotone" dataKey="kD"     stroke="Green"  dot={false} />}
            {showRate && <Area type="monotone" dataKey="PidTimeSinceLastCall_Millis" stroke="#ebebf3" fill="#8884d8" />}
        </ComposedChart>
    </ResponsiveContainer>
  )
}

export default PIDChart