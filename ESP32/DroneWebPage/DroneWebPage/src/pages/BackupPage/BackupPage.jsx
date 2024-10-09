import React, {useState, useContext} from 'react'
import JsonView from '@uiw/react-json-view';
import { saveAs } from 'file-saver';
import 'react-json-view-lite/dist/index.css';
import './BackupPage.css'
import { useFilePicker } from 'use-file-picker';
import {ApiContext} from '../../context/ApiContext'
import { FigureEightPolynomialKnot } from 'three/examples/jsm/curves/CurveExtras.js';


const BackupPage = () => {
  const {setUrl, connectionStatus, sendMessage, ip} = useContext(ApiContext);
  const { openFilePicker, filesContent, loading } = useFilePicker({
    accept: '.json',
    onFilesSuccessfullySelected: ({ plainFiles, filesContent }) => {
      setJsonRestore(JSON.parse(filesContent[0].content));
      setRestoreMessage("Aquivo carregado: " + filesContent[0].name)
      setRestoreMessageColor('blue')
    },
  });
  const [jsonBackup, setJsonBackup] = useState({})
  const [jsonRestore, setJsonRestore] = useState({})
  const [backupMensagem, setBackupMensagem] = useState('')
  const [backupMessageColor, setBackupMessageColor] = useState('black')
  const [restoreMessage, setRestoreMessage] = useState('')
  const [restoreMessageColor, setRestoreMessageColor] = useState('black')


  const handleBackup = ()=>{
    setBackupMessageColor('black')
    setBackupMensagem('Carregando backup. Aguarde....');
    setJsonBackup({})
    try {
      fetch("http://" + ip + ":80/getBackup")
          .then(response => response.json())
          .then(dados => {
            setBackupMensagem('Backup carregado com sucesso')
            setBackupMessageColor('blue')
            setJsonBackup(dados)
          })
          .catch((error)=>{
            setBackupMensagem('Erro ao carregar o backup do drone: ' + error.message)
            setBackupMessageColor('red')
          })
    } catch (error) {
      setBackupMensagem('Erro ao carregar o backup do drone: ' + error.message)
      setBackupMessageColor('red')
    }
    
  }
  const handleRestoreFile = ()=>{
    openFilePicker()
  }

  const handleDownloadBackup = ()=>{
    if (jsonRestore === null || jsonRestore === ''){
      setRestoreMessage("Não há backup a ser carregado")
      setRestoreMessageColor('red')
      return
    }

    setRestoreMessage("Carregando backup. Aguarde....")
    setRestoreMessageColor('black')

    try {
      fetch("http://" + ip + ":80/postRestoreParameters",
          {
            method: "POST",
            body: JSON.stringify(jsonRestore),
            headers: {
              "Content-type": "text/plain;"
            }
          }
        )
          .then(() => {
            setRestoreMessage('Backup carregado com sucesso. Reinicie o Drone')
            setRestoreMessageColor('blue')
          })
          .catch((error)=>{
            setRestoreMessage('Erro ao carregar o backup no drone: ' + error.message)
            setRestoreMessageColor('red')
          })
    } catch (error) {
      setRestoreMessage("Erro ao carregar o backup no Drone: " + error.message)
      setRestoreMessageColor('red')
    }
  }

  const handleSalvarBackup = ()=>{
    if (jsonBackup !== ''){
      var blob = new Blob([JSON.stringify(jsonBackup)], {type: "application/json"});
      const date = new Date();
      let dateTime = String(date.getFullYear());
      dateTime += String(date.getMonth() + 1).padStart(2, '0');
      dateTime += String(date.getDate()).padStart(2, '0');
      dateTime += '_';
      dateTime += String(date.getHours()).padStart(2, '0');
      dateTime += String(date.getMinutes()).padStart(2, '0');

      saveAs(blob, "BackupDrone_" + dateTime +".json");
    }
  }

  return (
    <div className='backup-page-container'>
      <div style={{display: 'flex', gap: '10px'}}>
        <button onClick={handleBackup}>Upload Backup</button>
        <button onClick={handleSalvarBackup}>Salvar</button>
        <button onClick={()=>{setJsonBackup('')}}>Limpar</button>
        <span style={{width: '70%', textAlign: 'right', color: backupMessageColor}}>{backupMensagem}</span>
      </div>
      
      <div style={{borderStyle: 'solid', borderWidth: 1, height: '100%', width: '99%'}}>
        <JsonView value={jsonBackup} displayObjectSize={ false} style={{'--w-rjv-background-color': '#ffffff',}}/>
      </div>
      <div style={{display: 'flex', flexDirection: 'row', gap: '10px'}}>
        <button onClick={handleDownloadBackup}>Download Backup</button>
        <button onClick={handleRestoreFile}>Carregar arquivo</button>
        <button onClick={()=>{setJsonRestore('')}}>Limpar</button>
        <span style={{width: '70%', textAlign: 'right', color: restoreMessageColor}}>{restoreMessage}</span>
      </div>
      <div style={{borderStyle: 'solid', borderWidth: 1, height: '100%', width: '99%'}}>
        <JsonView value={jsonRestore} displayObjectSize={ false} style={{'--w-rjv-background-color': '#ffffff',}}/>
      </div>
    </div>
  )
}

export default BackupPage