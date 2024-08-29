// Load Wi-Fi library
#include <WiFi.h>
#include <WebServer.h>
#include <ArduinoJson.h>
#include <esp_wifi.h>
#include <WebSocketsServer.h>
#include <SoftwareSerial.h>
#include <Ewma.h>


bool DEBUG = false; //Exibe informacoes no Serial

//Configuracao do ESP
#define RXD2 16 //FlyController RX
#define TXD2 17 //FlyController TX
#define RXD3 5 //Antena RF RX
#define TXD3 18 //Antena RF TX

EspSoftwareSerial::UART SerialAntenaRF;

//Sensores ultrassonicos
//TaskHandle_t TaskUltrassonic;

struct UltrassonicData{
   int range; //mm
   int actualValue; //mm
   int average; //Math to make average value
   bool collisionDetected;
   bool error;
   int TriggerPin;
   int EchoPin;
   float Incremento;
};

UltrassonicData US_FrontData = {1000, 0, 0, false, false, 13, 12, 0.1};
UltrassonicData US_RearData = {1000, 0, 0, false, false, 14, 27, 0.1};
UltrassonicData US_LeftData = {1000, 0, 0, false, false, 26, 25, 0.1};
UltrassonicData US_RightData = {1000, 0, 0, false, false, 33, 32, 0.1};
UltrassonicData US_DownData = {1000, 0, 0, false, false, 22, 23, 0.1};

Ewma US_Front_Filter(0.005);
Ewma US_Read_Filter(0.01);
Ewma US_Left_Filter(0.01);
Ewma US_Right_Filter(0.01);
Ewma US_Down_Filter(0.01);
// Replace with your network credentials
//-------ACCESS POINT
//const char* ssid     = "DRONE_SERVER";
//const char* password = "123456789";
//IPAddress local_IP(192, 168, 1, 177);   // Endereço IP do AP
//IPAddress gateway(192, 168, 1, 177);    // Gateway (geralmente o mesmo que o IP)
//IPAddress subnet(255, 255, 255, 0);   // Máscara de sub-rede

//-------CONECTED TO THE NETWORK AS STATION MODE
const char* ssid = "ALHN-7030";
const char* password =  "cLA7pFG8CL";

//MAC ADDRESS
uint8_t MACAddress[] = {0x52, 0x4F, 0x42, 0x4F, 0x53, 0x01};

// Inicializa WebSocket server na porta 81
WebSocketsServer webSocket = WebSocketsServer(81);

WebServer server(80);

//WebPage variables
String header;

//FlyController variables
uint16_t ROW = 992;
uint16_t PITCH = 992;
uint16_t YAW = 992;
uint16_t TROTLE = 172;
uint16_t ENABLE = 992;

#define CRSF_ADDRESS 0xC8
#define CRSF_FRAME_SIZE 26
#define CRSF_LENGTH 24
#define CRSF_FRAME_TYPE 0x16
#define CRSF_CHANNELS 16
uint16_t canais[CRSF_CHANNELS] = {992, 992, 172, 992, 992, 992, 992, 992, 992, 992, 992, 992, 992, 992, 992, 992};
byte pacoteCRSF[CRSF_FRAME_SIZE]; //Cria o pacote

void setup() {
  //Inicializa as seriais
  Serial.begin(115200);
  Serial2.begin(416666, SERIAL_8N1, RXD2, TXD2);
  SerialAntenaRF.begin(9600, SWSERIAL_8N1, RXD3, TXD3, false); //Comunicacao RadioFrequencia
  if (!SerialAntenaRF) { // If the object did not initialize, then its configuration is invalid
  Serial.println("Invalid EspSoftwareSerial pin configuration, check config"); 
    while (1) { // Don't continue with invalid configuration
      delay (1000);
    }
  }else{
    Serial.println("Serial Antena OK"); 
  }
  //Inicializa o WIFI
  // -- START ACCESS POINT MODE
  //Serial.println("WIFI ACCESS POINT MODE");
  //if (!WiFi.softAPConfig(local_IP, gateway, subnet)) {
  //  Serial.println("Falha ao configurar IP do AP");
  //}
  //WiFi.softAP(ssid, password);  

  // -- START STATION MODE
  WiFi.mode(WIFI_MODE_STA);

  // Change ESP32 Mac Address
  esp_err_t err = esp_wifi_set_mac(WIFI_IF_STA, &MACAddress[0]);
  if (err != ESP_OK) {
    while(true){
      Serial.println("Problema com o MAC ADDRESS");
      delay(1000);
    }
  }else{
    readMacAddress();
  }

  Serial.println("WIFI STA MODE");
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) 
  {
    delay(500);
    Serial.println("Conectando ao WiFi..");
  }
  Serial.println("WiFi conectada.");
  Serial.println("Endereço de IP: ");
  Serial.println(WiFi.localIP());

  //Start web server
  server.begin();

  //Inicializa os servers
  server.on("/", HTTP_GET, handleWebPage);              // Responde com uma página web
  //server.on("/Controle", HTTP_POST, handlePostJSON_Controle);

  //Inicializa a task to Ultrassonico
  //xTaskCreatePinnedToCore(
  //           CheckColision,  /* Task function. */
  //           "TaskUltrassonic",    /* name of task. */
  //           10000,      /* Stack size of task */
  //           NULL,       /* parameter of the task */
  //           1,          /* priority of the task */
  //           &TaskUltrassonic,     /* Task handle to keep track of created task */
  //           1);         /* pin task to core 1 */
  DefineUltrassonic(US_RearData);
  DefineUltrassonic(US_FrontData);
  DefineUltrassonic(US_LeftData);
  DefineUltrassonic(US_RightData);
  DefineUltrassonic(US_DownData);

  // Inicia o WebSocket server
  webSocket.begin();
  webSocket.onEvent(webSocketEvent);
}

void loop(){
  // Mantém o WebSocket ativo
  webSocket.loop();

  //Processa a Web Page
  server.handleClient();

  //Auto Pilot
  AutoPilot(true);

  //Processa o pacote CRSF
  CRSF();

  //Comunicacao com o FlyController
  CommFlyController();

  //Read Anthena Serial
  AnthenaSerial();

  //Check collision
  CheckColision();
}

void AutoPilot(bool enable){
  if(enable){
    if (canais[4] == 992){
      if(US_DownData.collisionDetected){
        if (TROTLE < 1600){
          US_DownData.Incremento += 0.01;
          TROTLE += US_DownData.Incremento;
        }
      }else{
        if(TROTLE > 172){
          US_DownData.Incremento += 0.01;
          TROTLE -= US_DownData.Incremento;
        }
      }
      
      if(US_DownData.Incremento >= 1){
        US_DownData.Incremento = 0.01;
      }
    }
  }
}

void AnthenaSerial(){
  if (SerialAntenaRF.available()){
    char receivedChar = SerialAntenaRF.read();
        
    // Opcional: imprime o dado recebido no Serial Monitor
    Serial.print("Recebido Antena: ");
    Serial.println(receivedChar);
  }
}
// Função que lida com eventos do WebSocket
void webSocketEvent(uint8_t num, WStype_t type, uint8_t * payload, size_t length) {
  if(type == WStype_TEXT) {
    // Recebe a mensagem do cliente
    String message = String((char*) payload);
    
    // Cria um documento JSON
    StaticJsonDocument<200> doc;

    // Tenta deserializar o JSON
    DeserializationError error = deserializeJson(doc, message);

    if (error) {
      Serial.print(F("Falha ao analisar JSON: "));
      Serial.println(error.f_str());
      server.send(400, "application/json", "{\"status\":\"failed\"}");
      return;
    }

    // Acessa os valores do JSON
    float row = doc["ROW"];
    ROW = row * 1000 + 1000;

    ROW = ROW > 1811 ? 1811 : ROW;
    ROW = ROW < 172 ? 172 : ROW;

    float pitch = doc["PITCH"];
    PITCH = pitch * 1000 + 1000;

    PITCH = PITCH > 1811 ? 1811 : PITCH;
    PITCH = PITCH < 172 ? 172 : PITCH;

    float yaw = doc["YAW"];
    YAW = yaw * 1000 + 1000;

    YAW = YAW > 1811 ? 1811 : YAW;
    YAW = YAW < 172 ? 172 : YAW;

    float trotle = doc["TROTLE"];
    //TROTLE = trotle * 1000 + 1000;
    if (trotle > 0.1 || trotle < -0.1){
      TROTLE += trotle;
    }
    
    TROTLE = TROTLE > 1811 ? 1811 : TROTLE;
    TROTLE = TROTLE < 172 ? 172 : TROTLE;

    bool enable = doc["ENABLE"];
    if (enable){
      ENABLE = 992;
    }else{
      ENABLE = 1400;
    }

    Serial.print("ROW: ");
    Serial.print(ROW);
    Serial.print(", PITCH: ");
    Serial.print(PITCH);
    Serial.print(", YAW: ");
    Serial.print(YAW);
    Serial.print(", TROTLE: ");
    Serial.print(TROTLE);
    Serial.print(", ENABLE: ");
    Serial.println(ENABLE);
  }
}

void CommFlyController(){
  if (DEBUG){
    Serial.print("Pacote: ");
    for(int i = 0; i < CRSF_FRAME_SIZE; i++){
        Serial.print(pacoteCRSF[i], HEX);
        Serial.print(",");
    }
    Serial.println(" ---");
    
    Serial.print("Canais: ");
    for(int i = 0; i < 16; i++){
      Serial.print(canais[i]);
      Serial.print(",");
    }
    Serial.println(".");
  }

  Serial2.write(pacoteCRSF, CRSF_FRAME_SIZE);

}

void handleWebPage(){
  String html =
    R"rawliteral(
      <!DOCTYPE html>
      <html lang="en">
      <head>
          <meta charset="UTF-8">
          <meta name="viewport" content="width=device-width, initial-scale=1.0">
          <title>Drone Control</title>
          <style>
              body {
                  display: flex;
                  flex-direction: row;

                  align-items: center;
                  justify-content: center;
                  height: 100vh;
                  background-color: #f0f0f0;
                  margin: 0;
              }
              #enable-btn {
                  padding: 10px 20px;
                  font-size: 18px;
                  margin-bottom: 20px;
              }
              .control1 {
                  display: flex;
                  flex-direction: row;
                  align-items: center;
                  margin-bottom: 20px;
                  margin: 10px;
              }
              .control2 {
                  display: flex;
                  flex-direction: column;
                  align-items: center;
                  margin-bottom: 20px;
                  margin: 10px;
              }
              .joystick {
                  width: 150px;
                  height: 150px;
                  background-color: #ddd;
                  border-radius: 50%;
                  position: relative;
                  touch-action: none;
              }
              .knob {
                  width: 50px;
                  height: 50px;
                  background-color: #888;
                  border-radius: 50%;
                  position: absolute;
                  top: 50%;
                  left: 50%;
                  transform: translate(-50%, -50%);
              }
              #enable-btn{
                position: relative;
              }
          </style>
      </head>
      <body>
          <div class="control1">
            <p id="yaw-label">Yaw</p>
            <div class="control2">
            <p id="trotle-label">Trotle</p>
            <div class="joystick" id="joystick1">
                <div class="knob"></div>
            </div>
            </div>
          </div>
          <button id="enable-btn">Enable</button>
          <div class="control1">
              <p id="pitch-label">Pitch</p>
              <div class="control2">
                <p id="roll-label">Roll</p>
                <div class="joystick" id="joystick2">
                    <div class="knob"></div>
                </div>
              </div>
          </div>
          <div>
            <p>
                <span>Ganho:</span>
                <input id="ganho-input" type="text" value="1">
            </p>
            <p>
                <span>Ganho Trotle:</span>
                <input id="ganho-trotle" type="text" value="1">
            </p>
            <p id="joystick-label" style="font-size: 20px; color: black;">Joystick Desconectado</p>
            <p>
                <span>IP DRONE: </span>
                <input id="ip-drone-input" type="text" value="192.168.1.43">
                <button id="button-conectar">Conectar</button>
            </p>
          </div>
          <script>
              // Conecta ao WebSocket server
              const ip_drone_input = document.getElementById('ip-drone-input');
              console.log('ws://' + ip_drone_input.value +':81');
              var socket = new WebSocket('ws://' + ip_drone_input.value +':81');


              var ControleDetectado = false;
              var ControleHabilitado = false;
              var ControleDrone = {
                  YAW: 0,
                  PITCH: 0,
                  ROW: 0,
                  TROTLE: 0,
                  ENABLE: false
              };

              socket.onopen = function(event) {
                const botao = document.getElementById('button-conectar');
                botao.style.backgroundColor = 'green';
                botao.innerHTML = "Conectado";
                console.log("Conectado")
               };

              socket.onclose = function(event){
                const botao = document.getElementById('button-conectar');
                botao.style.backgroundColor = 'white';
                botao.innerHTML = "Conectar";
               }

               // Função para habilitar controle
              document.getElementById('button-conectar').addEventListener('click', function() {
                  // Conecta ao WebSocket server
                  socket = new WebSocket('ws://' + ip_drone_input.value +':81');
              });

              // Função para habilitar controle
              document.getElementById('enable-btn').addEventListener('click', function() {
                  const botao = document.getElementById('enable-btn');
                  ControleHabilitado = !ControleHabilitado;
                  if (ControleHabilitado){
                      botao.style.backgroundColor = 'green';
                  }else{
                      botao.style.backgroundColor = 'white';
                  }
              });
              

              // Função para manipular os joysticks
              function initJoystick(joystickId) {
                  const joystick = document.getElementById(joystickId);
                  const knob = joystick.querySelector('.knob');
                  const maxOffset = (joystick.clientWidth - knob.clientWidth) / 2;
                  
                  joystick.addEventListener('touchmove', function(event) {
                      const touch = event.touches[0];
                      const rect = joystick.getBoundingClientRect();
                      const x = touch.clientX - rect.left - joystick.clientWidth / 2;
                      const y = touch.clientY - rect.top - joystick.clientHeight / 2;

                      // Limitar o movimento do knob dentro do joystick
                      const distance = Math.sqrt(x * x + y * y);
                      const angle = Math.atan2(y, x);

                      const moveX = Math.cos(angle) * Math.min(distance, maxOffset);
                      const moveY = Math.sin(angle) * Math.min(distance, maxOffset);

                      knob.style.left = joystick.clientWidth / 2 + moveX +'px';
                      knob.style.top = joystick.clientHeight / 2 + moveY + 'px';
                      
                      scaleX = moveX / maxOffset;
                      scaleY = moveY / maxOffset * -1;
                      inputGanho = document.getElementById('ganho-input');
                      inputGanhoTrotle = document.getElementById('ganho-trotle');
                      if (joystickId === 'joystick1'){
                          ControleDrone.TROTLE = scaleY * inputGanhoTrotle.value;
                          ControleDrone.YAW = scaleX * inputGanho.value;
                      }

                      if (joystickId === 'joystick2'){
                          ControleDrone.ROW = scaleY * inputGanho.value;
                          ControleDrone.PITCH = scaleX * inputGanho.value;
                      }
                  });

                  joystick.addEventListener('touchend', function() {
                      knob.style.left = '50%';
                      knob.style.top = '50%';

                      ControleDrone.PITCH = 0;
                      ControleDrone.YAW = 0;
                      ControleDrone.TROTLE = 0;
                      ControleDrone.ROW = 0;
                  });
              }

              function EnviarValorCanais(){
                ControleDrone.ENABLE = ControleHabilitado; //Atualiza o valor do Enable
                try {
                    if(true){
                    socket.send(JSON.stringify(ControleDrone));
                  }else{
                    var xhr = new XMLHttpRequest();

                    xhr.open('POST', '/Controle', true);
                    xhr.setRequestHeader("Content-Type", "application/json");
                    xhr.onreadystatechange = function () {
                        if (xhr.readyState === XMLHttpRequest.DONE) {
                            if (xhr.status >= 200 && xhr.status < 300) {
                                // A requisição foi bem-sucedida
                                console.log("Success:", xhr.responseText);
                            } else {
                                // Houve algum erro
                                console.error("Error:", xhr.statusText);
                            }
                        }
                    };
                    xhr.send(JSON.stringify(ControleDrone));

                  }
                } finally{
                    console.log("Enviado comando")
                  if (ControleDrone.ENABLE){
                    setTimeout(EnviarValorCanais, 10);
                  }else{
                    setTimeout(EnviarValorCanais, 10);
                  }   
                }
              }

            window.addEventListener("gamepadconnected", (e) => {
                console.log(
                    "Gamepad connected at index %d: %s. %d buttons, %d axes.",
                    e.gamepad.index,
                    e.gamepad.id,
                    e.gamepad.buttons.length,
                    e.gamepad.axes.length,
                );

                const joystick_label = document.getElementById('joystick-label');
                joystick_label.innerHTML = "Joystick Detectado";
                joystick_label.style.backgroundColor = 'green';
                ControleDetectado = true;
                readJoystick();
            });

            window.addEventListener("gamepaddisconnected", (e) => {
                ControleDetectado = false;
                ControleHabilitado = false;

                ControleDrone.PITCH = 0;
                ControleDrone.YAW = 0;
                ControleDrone.TROTLE = 0;
                ControleDrone.ROW = 0;
                
                //Atualiza botao enable
                const botao = document.getElementById('enable-btn');
                if (ControleHabilitado){
                    botao.style.backgroundColor = 'green';
                }else{
                    botao.style.backgroundColor = 'white';
                }
                
                //Atualiza o knob 1
                let joystick = document.getElementById('joystick1');
                let knob = joystick.querySelector('.knob');
                knob.style.left = '50%';
                knob.style.top = '50%';

                //Atualiza o knob 2
                joystick = document.getElementById('joystick2');
                knob = joystick.querySelector('.knob');
                knob.style.left = '50%';
                knob.style.top = '50%';

                const joystick_label = document.getElementById('joystick-label');
                joystick_label.innerHTML = "Joystick Desconectado";
                joystick_label.style.backgroundColor = 'white';
            });

            function readJoystick() {
                if(navigator.getGamepads().length > 0){
                    const gamepad = navigator.getGamepads()[0];
                    if (gamepad) {
                        if (gamepad.buttons[5].pressed) {
                            ControleHabilitado = true;
                            console.log("Habilitado");
                        }
                        if (gamepad.buttons[4].pressed) {
                            ControleHabilitado = false;
                        }
                        
                        //Atualiza botao enable
                        const botao = document.getElementById('enable-btn');
                        if (ControleHabilitado){
                            botao.style.backgroundColor = 'green';
                        }else{
                            botao.style.backgroundColor = 'white';
                        }

                        //Atualiza o knob 1
                        let joystick = document.getElementById('joystick1');
                        let knob = joystick.querySelector('.knob');
                        knob.style.left = ((gamepad.axes[0] + 1) / 2) * 100 + '%';
                        knob.style.top = ((gamepad.axes[1] + 1) / 2) * 100 + '%';

                        //Atualiza o knob 2
                        joystick = document.getElementById('joystick2');
                        knob = joystick.querySelector('.knob');
                        knob.style.left = ((gamepad.axes[2] + 1) / 2) * 100 + '%';
                        knob.style.top = ((gamepad.axes[3] + 1) / 2) * 100 + '%';

                        // Acesse os eixos do joystick (e.g., eixo 0 e 1)
                        inputGanho = document.getElementById('ganho-input');
                        inputGanhoTrotle = document.getElementById('ganho-trotle');
                        ControleDrone.YAW  = gamepad.axes[0] * inputGanho.value;
                        ControleDrone.TROTLE  = gamepad.axes[1] * -1 * inputGanhoTrotle.value;
                        ControleDrone.PITCH  = gamepad.axes[2] * inputGanho.value;
                        ControleDrone.ROW  = gamepad.axes[3] * -1 * inputGanho.value;

                        const yaw_label = document.getElementById('yaw-label');
                        yaw_label.innerHTML = "Yaw/" + ControleDrone.YAW.toFixed(2);
                        const trotle_label = document.getElementById('trotle-label');
                        trotle_label.innerHTML = "Trotle/" + ControleDrone.TROTLE.toFixed(2);
                        const roll_label = document.getElementById('roll-label');
                        roll_label.innerHTML = "Roll/" + ControleDrone.ROW.toFixed(2);
                        const pitch_label = document.getElementById('pitch-label');
                        pitch_label.innerHTML = "Pitch/" + ControleDrone.PITCH.toFixed(2);

                    }
                }

                setTimeout(readJoystick, 10);
            }


              initJoystick('joystick1');
              initJoystick('joystick2');
              EnviarValorCanais();
          </script>
      </body>
      </html>
    )rawliteral"
  ;
  server.send(200, "text/html", html);
}
void handlePostJSON_Controle() {
  // Verifica se o corpo da requisição contém dados
  if (server.hasArg("plain")) {
    String json = server.arg("plain");

    // Cria um documento JSON
    StaticJsonDocument<200> doc;

    // Tenta deserializar o JSON
    DeserializationError error = deserializeJson(doc, json);

    if (error) {
      Serial.print(F("Falha ao analisar JSON: "));
      Serial.println(error.f_str());
      server.send(400, "application/json", "{\"status\":\"failed\"}");
      return;
    }

    // Acessa os valores do JSON
    float row = doc["ROW"];
    ROW = row * 1000 + 1000;

    float pitch = doc["PITCH"];
    PITCH = pitch * 1000 + 1000;

    float yaw = doc["YAW"];
    YAW = yaw * 1000 + 1000;

    float trotle = doc["TROTLE"];
    TROTLE = trotle * 1000 + 1000;
    TROTLE = TROTLE > 2000 ? 2000 : TROTLE;
    TROTLE = TROTLE < 172 ? 172 : TROTLE;

    bool enable = doc["ENABLE"];
    if (enable){
      ENABLE = 992;
    }else{
      ENABLE = 1400;
    }

    Serial.print("ROW: ");
    Serial.print(ROW);
    Serial.print(", PITCH: ");
    Serial.print(PITCH);
    Serial.print(", YAW: ");
    Serial.print(YAW);
    Serial.print(", TROTLE: ");
    Serial.print(TROTLE);
    Serial.print(", ENABLE: ");
    Serial.println(ENABLE);

    // Envia uma resposta de sucesso
    server.send(200, "application/json", "{\"status\":\"success\"}");
  } else {
    server.send(400, "application/json", "{\"status\":\"no data\"}");
  }
}
void CRSF(){
  //Inicializa o pacote
  for(int i = 0; i < CRSF_FRAME_SIZE; i++){
    pacoteCRSF[i] = 0x00;
  }

  //Atualiza os valores dos canais
  canais[0] = ROW;
  canais[1] = PITCH;
  canais[2] = TROTLE;
  canais[3] = YAW;
  canais[4] = ENABLE;

  //Montage o pacote
  montarPacoteCRSF(canais, pacoteCRSF);

  //Imprime o pacote
  if (DEBUG){
    Serial.print("Pacote: ");
    for(int i = 0; i < CRSF_FRAME_SIZE; i++){
      Serial.print(pacoteCRSF[i], HEX);
      Serial.print(",");
    }
    Serial.println(".");

    Serial.print("Canais: ");
    for(int i = 0; i < 16; i++){
      Serial.print(canais[i]);
      Serial.print(",");
    }
    Serial.println(".");
  }
}

void montarPacoteCRSF(uint16_t canais[CRSF_CHANNELS], byte pacote[]) {
    pacote[0] = CRSF_ADDRESS;       // Endereço
    pacote[1] = CRSF_LENGTH;    // Tamanho do frame
    pacote[2] = CRSF_FRAME_TYPE;    // Tamanho do frame

    int bitOffset = 0;
    for (int i = 0; i < CRSF_CHANNELS; i++) {
        WriteUshortInByteArray(pacote, canais[i], bitOffset);
    }

    byte crcPkt[CRSF_FRAME_SIZE - 2];
    for(int i = 0; i < CRSF_FRAME_SIZE - 2; i++){
      crcPkt[i] = pacote[i + 2];
    }
    // Calcular o checksum
    pacote[CRSF_FRAME_SIZE - 1] = calcularCRC8(crcPkt, 23);  // Adicionar o checksum ao final do pacote
}

void WriteUshortInByteArray(byte byteArray[], uint16_t channel, int &bitInicio) {
    int bytePosicao = (bitInicio / 8) + 3;   // Determina o byte inicial
    int bitOffset = bitInicio % 8;     // Determina o bit inicial dentro do byte

    // Cria uma variável temporária para o valor deslocado
    uint32_t valorDeslocado = static_cast<uint32_t>(channel) << bitOffset;

    // Escreve os bits nos bytes do array, considerando o deslocamento
    byteArray[bytePosicao] |= valorDeslocado & 0xFF;             // LSB
    byteArray[bytePosicao + 1] |= (valorDeslocado >> 8) & 0xFF;  // Byte intermediário
    byteArray[bytePosicao + 2] |= (valorDeslocado >> 16) & 0xFF; // MSB, se necessário

    bitInicio += 11;
}

void printBits(byte byteArray[], int tamanho) {
    for (int i = 0; i < tamanho; i++) {
        Serial.print("Byte ");
        Serial.print(i);
        Serial.print(": ");
        
        // Itera sobre cada bit do byte atual
        for (int bit = 7; bit >= 0; bit--) {
            if (byteArray[i] & (1 << bit)) {
                Serial.print('1');
            } else {
                Serial.print('0');
            }
        }
        Serial.print(" - ");
        Serial.print(byteArray[i]); // Nova linha após cada byte
        Serial.print(" - ");
        Serial.println(byteArray[i], HEX); // Nova linha após cada byte
        
    }
}

byte calcularCRC8(byte data[], int tamanho) {
    byte crc = 0x00;  // Valor inicial do CRC (0xFF é comum para CRC-8)

    // Polinômio CRC-8 (0xD5 ou 11010101 em binário)
    byte polinomio = 0xD5;

    for (int i = 0; i < tamanho; i++) {
        crc ^= data[i];  // XOR com o byte de dados atual
        
        for (int j = 0; j < 8; j++) {  // Itera sobre cada bit
            if (crc & 0x80) {  // Se o bit mais significativo for 1
                crc = (crc << 1) ^ polinomio;  // Desloca e XOR com o polinômio
            } else {
                crc <<= 1;  // Apenas desloca
            }
        }
    }

    return crc;  // Retorna o valor do CRC calculado
}

void readMacAddress(){
  uint8_t baseMac[6];
  esp_err_t ret = esp_wifi_get_mac(WIFI_IF_STA, baseMac);
  if (ret == ESP_OK) {
    Serial.printf("%02x:%02x:%02x:%02x:%02x:%02x\n",
                  baseMac[0], baseMac[1], baseMac[2],
                  baseMac[3], baseMac[4], baseMac[5]);
  } else {
    Serial.println("Failed to read MAC address");
  }
}

void DefineUltrassonic(UltrassonicData sensor){
  pinMode(sensor.TriggerPin, OUTPUT); // Sets the trigPin as an Output
  pinMode(sensor.EchoPin, INPUT); // Sets the echoPin as an Input
}

float ReadUltrassonic(UltrassonicData sensor, Ewma analogFilter, bool printResult){
  digitalWrite(sensor.TriggerPin, HIGH);
  delayMicroseconds(10);
  digitalWrite(sensor.TriggerPin, LOW);

  long duration = pulseIn(sensor.EchoPin, HIGH);
  //long duration = pulseIn(sensor.EchoPin, HIGH, 12000); //200 = duration * 0.034/2 -->> ~12000
  float distance = duration * 0.034/2;
  distance *= 10;
  
  distance = analogFilter.filter(distance);

  if(printResult){
    Serial.println(distance);
  }

  return distance;
}

void CheckColision(){
  //Front sensor
  //US_FrontData.actualValue = (int) ReadUltrassonic(US_FrontData,US_Front_Filter, false);
  //US_FrontData.collisionDetected = US_FrontData.actualValue < US_FrontData.range;
  //US_FrontData.error = US_FrontData.actualValue <= 0 || US_FrontData.actualValue > 3000;

  //Down sensor
  US_DownData.actualValue = (int) ReadUltrassonic(US_DownData, US_Down_Filter, true);
  US_DownData.collisionDetected = US_DownData.actualValue < US_DownData.range;
  US_DownData.error = US_DownData.actualValue <= 0 || US_DownData.actualValue > 3000;
}
