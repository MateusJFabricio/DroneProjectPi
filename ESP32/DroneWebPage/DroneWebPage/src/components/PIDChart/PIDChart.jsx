import React, {useState, useEffect, useRef} from 'react'
import {
    LineChart,
    Line,
    XAxis,
    YAxis,
    CartesianGrid,
    Tooltip,
    Legend,
    ReferenceLine,
    ResponsiveContainer,
  } from 'recharts';

const PIDChart = ({enable, data, zoomOut, maxElements, reset}) => {
    const [dataChart, setDataChart] = useState([])
    const lastIndex = useRef(0)

    const initialState = {
        data: data,
        left: 'dataMin',
        right: 'dataMax',
        refAreaLeft: '',
        refAreaRight: '',
        top: 'dataMax+1',
        bottom: 'dataMin-1',
        top2: 'dataMax+20',
        bottom2: 'dataMin-20',
        animation: true,
      };
    
    useEffect(() => {
      if (reset){
        setDataChart([])
      }
     
    }, [reset])
    

    useEffect(() => {
      if (zoomOut){
        //zoomOutCall();
      }
    }, [zoomOut])
    
    useEffect(() => {
      if (enable){
        data[0].name = lastIndex.current;
        let newData = [...dataChart, ...data]
        if (newData.length > maxElements){
          newData = newData.slice(maxElements * -1, newData.length);
        }
        setDataChart(newData)
        lastIndex.current++;
      }
    }, [data])

    const getAxisYDomain = (from, to, ref, offset) => {
        const refData = initialData.slice(from - 1, to);
        let [bottom, top] = [refData[0][ref], refData[0][ref]];
        refData.forEach((d) => {
            if (d[ref] > top) top = d[ref];
            if (d[ref] < bottom) bottom = d[ref];
        });
        
        return [(bottom | 0) - offset, (top | 0) + offset];
    };

    /*
    const zoom = ()=> {
        let { refAreaLeft, refAreaRight } = this.state;
        const { data } = this.state;

        if (refAreaLeft === refAreaRight || refAreaRight === '') {
            this.setState(() => ({
            refAreaLeft: '',
            refAreaRight: '',
            }));
            return;
        }

        // xAxis domain
        if (refAreaLeft > refAreaRight) [refAreaLeft, refAreaRight] = [refAreaRight, refAreaLeft];

        // yAxis domain
        const [bottom, top] = getAxisYDomain(refAreaLeft, refAreaRight, 'cost', 1);
        const [bottom2, top2] = getAxisYDomain(refAreaLeft, refAreaRight, 'impression', 50);

        this.setState(() => ({
            refAreaLeft: '',
            refAreaRight: '',
            data: data.slice(),
            left: refAreaLeft,
            right: refAreaRight,
            bottom,
            top,
            bottom2,
            top2,
        }));
    }

    const zoomOutCall = ()=> {
        const { data } = this.state;
        this.setState(() => ({
            data: data.slice(),
            refAreaLeft: '',
            refAreaRight: '',
            left: 'dataMin',
            right: 'dataMax',
            top: 'dataMax+1',
            bottom: 'dataMin',
            top2: 'dataMax+50',
            bottom2: 'dataMin+50',
        }));
    }
        */
  return (
    <ResponsiveContainer width="100%" height="100%">
        <LineChart
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
            <YAxis domain={[-100, 100]} />
            <Tooltip />
            <Legend />
            <Line type="monotone" dataKey="Output" stroke="#8884d8" dot={false}/>
            <Line type="monotone" dataKey="Actual" stroke="#82ca9d"  dot={false} />
            <Line type="monotone" dataKey="SP"     stroke="red"  dot={false} />
        </LineChart>
    </ResponsiveContainer>
  )
}

export default PIDChart