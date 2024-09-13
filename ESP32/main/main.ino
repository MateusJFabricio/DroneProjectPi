
//THERE IS NO WARRANTY FOR THE SOFTWARE, TO THE EXTENT PERMITTED BY APPLICABLE LAW. EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR 
//OTHER PARTIES PROVIDE THE SOFTWARE “AS IS” WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES 
//OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE SOFTWARE IS WITH THE CUSTOMER. SHOULD THE 
//SOFTWARE PROVE DEFECTIVE, THE CUSTOMER ASSUMES THE COST OF ALL NECESSARY SERVICING, REPAIR, OR CORRECTION EXCEPT TO THE EXTENT SET OUT UNDER THE HARDWARE WARRANTY IN THESE TERMS.

#include <WiFi.h>
#include <WebServer.h>
#include <WebSocketsServer.h>
#include <ArduinoJson.h>
#include <AsyncTCP.h>
//#include <ESPAsyncWebServer.h>
#include <SPIFFS.h>

#include <Wire.h>
#include <ESP32Servo.h> // Change to the standard Servo library for ESP32
#include <DShotRMT.h>

uint16_t throttle = 0;
uint16_t target = 0;

WebSocketsServer webSocket = WebSocketsServer(81);
WebServer server(80);

// REPLACE WITH YOUR NETWORK CREDENTIALS
const char* ssid = "ALHN-7030";
const char* password = "cLA7pFG8CL";

const char* PARAM_TIME_CYCLE = "tc";  //Computation time cycle

//FlyController variables
uint16_t ROLL = 992;
uint16_t PITCH = 992;
uint16_t YAW = 992;
uint16_t TROTLE = 172;
uint16_t ENABLE = 992;

volatile float RatePitch, RateRoll, RateYaw;
volatile float RateCalibrationPitch, RateCalibrationRoll, RateCalibrationYaw;
int RateCalibrationNumber;

Servo mot1;
Servo mot2;
Servo mot3;
Servo mot4;
const int mot1_pin = 13;
const int mot2_pin = 12;
const int mot3_pin = 14;
const int mot4_pin = 27;

//DShotRMT esc1(mot1_pin, RMT_CHANNEL_0);

volatile int ReceiverValue[6]; // Increase the array size to 6 for Channel 1 to Channel 6

// float voltage;

float DesiredRateRoll, DesiredRatePitch, DesiredRateYaw;
float ErrorRateRoll, ErrorRatePitch, ErrorRateYaw;
float InputRoll, InputThrottle, InputPitch, InputYaw;
float PrevErrorRateRoll, PrevErrorRatePitch, PrevErrorRateYaw;
float PrevItermRateRoll, PrevItermRatePitch, PrevItermRateYaw;
float PIDReturn[] = {0, 0, 0};

// float AccX, AccY, AccZ;
// float AngleRoll, AnglePitch;
// float KalmanAngleRoll=0, KalmanUncertaintyAngleRoll=2*2;
// float KalmanAnglePitch=0, KalmanUncertaintyAnglePitch=2*2;
// float Kalman1DOutput[]={0,0};

float PRateRoll = 0.6; //For outdoor flights, keep this gain to 0.75 and for indoor flights keep the gain to be 0.6

float IRateRoll = 0.012;
float DRateRoll = 0.0085;

float PRatePitch = PRateRoll;
float IRatePitch = IRateRoll;
float DRatePitch = DRateRoll;

float PRateYaw = 4.2;
float IRateYaw = 2.8;
float DRateYaw = 0;

uint32_t LoopTimer;
float t=0.006;      //time cycle

//Kalman filters for angle mode
volatile float AccX, AccY, AccZ;
volatile float AngleRoll, AnglePitch;
volatile float KalmanAngleRoll=0, KalmanUncertaintyAngleRoll=2*2;
volatile float KalmanAnglePitch=0, KalmanUncertaintyAnglePitch=2*2;
volatile float Kalman1DOutput[]={0,0};
volatile float DesiredAngleRoll, DesiredAnglePitch;
volatile float ErrorAngleRoll, ErrorAnglePitch;
volatile float PrevErrorAngleRoll, PrevErrorAnglePitch;
volatile float PrevItermAngleRoll, PrevItermAnglePitch;
float PAngleRoll=2; float PAnglePitch=PAngleRoll;
float IAngleRoll=0; float IAnglePitch=IAngleRoll;
float DAngleRoll=0; float DAnglePitch=DAngleRoll;

volatile float MotorInput1, MotorInput2, MotorInput3, MotorInput4;

void setup(void) {
  
  Serial.begin(115200);
  // Initialize SPIFFS
  #ifdef ESP32
    if(!SPIFFS.begin(true)){
      Serial.println("An Error has occurred while mounting SPIFFS");
      return;
    }
  #else
    if(!SPIFFS.begin()){
      Serial.println("An Error has occurred while mounting SPIFFS");
      return;
    }
  #endif

  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password);
  if (WiFi.waitForConnectResult() != WL_CONNECTED) {
    Serial.println("WiFi Failed!");
    return;
  }
  Serial.println();
  Serial.print("IP Address: ");
  Serial.println(WiFi.localIP());

  //Configura MPU6050
  Wire.setClock(400000);
  Wire.begin();
  delay(250);
  Wire.beginTransmission(0x68);
  Wire.write(0x6B);
  Wire.write(0x00);
  Wire.endTransmission();

  //Configura Motor
  mot1.attach(mot1_pin,1000,2000);
  mot2.attach(mot2_pin,1000,2000);
  mot3.attach(mot3_pin,1000,2000);
  mot4.attach(mot4_pin,1000,2000);

  //to stop esc from beeping
  mot1.write(0);
  mot2.write(0);
  mot3.write(0);
  mot4.write(0); 

  //Beep de inicializacao
  pinMode(15, OUTPUT);
  Beep(1);

  //Calibra o MPU6050
  for (RateCalibrationNumber = 0; RateCalibrationNumber < 4000; RateCalibrationNumber++)
  {
    gyro_signals();
    RateCalibrationRoll += RateRoll;
    RateCalibrationPitch += RatePitch;
    RateCalibrationYaw += RateYaw;
    delay(1);
  }

  RateCalibrationRoll /= 4000;
  RateCalibrationPitch /= 4000;
  RateCalibrationYaw /= 4000;
  //Gyro Calibrated Values
  // Serial.print("Gyro Calib: ");
  // Serial.print(RateCalibrationRoll);
  // Serial.print("  ");
  // Serial.print(RateCalibrationPitch);
  // Serial.print("  ");
  // Serial.print(RateCalibrationYaw);
  // Serial.print(" -- ");

  LoopTimer = micros();

  // Inicia o WebSocket server
  webSocket.begin();
  webSocket.onEvent(webSocketEvent);

  server.enableCORS();
  server.on("/getValues", RestAPI);
  server.begin();

  xTaskCreatePinnedToCore(
      TaskWebServer, /* Function to implement the task */
      "TaskWebServer", /* Name of the task */
      10000,  /* Stack size in words */
      NULL,  /* Task input parameter */
      0,  /* Priority of the task */
      NULL,  /* Task handle. */
      1);

  xTaskCreatePinnedToCore(
      TaskWebServerAPI, /* Function to implement the task */
      "TaskWebServerAPI", /* Name of the task */
      10000,  /* Stack size in words */
      NULL,  /* Task input parameter */
      0,  /* Priority of the task */
      NULL,  /* Task handle. */
      1);
  }

void loop(void) {
  //FlyController();

  //esc1.sendThrottleValue(target);
  mot3.write(map(target, 48, 2047, 0, 180));
  
  //Serial.print(",Target: ");
  //Serial.println(target);
  //Serial.print(",Trottle: ");
  //Serial.println(throttle);
  
  //delay(1);
}

void TaskWebServer( void * parameter) {
  while(true){
    //Roda a API
    server.handleClient();
  }
}

void TaskWebServerAPI( void * parameter) {
  while(true){
    // Mantém o WebSocket ativo
    webSocket.loop();
  }
}

void Beep(int code){
  int pinBuzzer = 15;
  int led_time=100;
  switch(code){
    case 1: //Inicializacao
      tone(pinBuzzer, 2273, 100);
      delay(50);
      tone(pinBuzzer, 2273, 100);
      delay(50);
      tone(pinBuzzer, 2273, 100);
      break;
    default: //ERRO
      tone(pinBuzzer, 2273, 100);
      delay(500);
      tone(pinBuzzer, 2273, 100);
      break;
  }
}

void FlyController(){
  if(ReceiverValue[2] < 1030 && ReceiverValue[4] > 1500 )
  {
    PRateRoll = readFile(SPIFFS, "/pGain.txt").toFloat();
    IRateRoll = readFile(SPIFFS, "/iGain.txt").toFloat();
    DRateRoll = readFile(SPIFFS, "/dGain.txt").toFloat();

    PRateYaw = readFile(SPIFFS, "/pYaw.txt").toFloat();
    IRateYaw = readFile(SPIFFS, "/iYaw.txt").toFloat();
    DRateYaw = readFile(SPIFFS, "/dYaw.txt").toFloat();

    t = readFile(SPIFFS, "/tc.txt").toFloat();
  }

  //enter your loop code here
  gyro_signals();
  RateRoll -= RateCalibrationRoll;
  RatePitch -= RateCalibrationPitch;
  RateYaw -= RateCalibrationYaw;


  kalman_1d(KalmanAngleRoll, KalmanUncertaintyAngleRoll, RateRoll, AngleRoll);
  KalmanAngleRoll=Kalman1DOutput[0]; KalmanUncertaintyAngleRoll=Kalman1DOutput[1];
  kalman_1d(KalmanAnglePitch, KalmanUncertaintyAnglePitch, RatePitch, AnglePitch);
  KalmanAnglePitch=Kalman1DOutput[0]; KalmanUncertaintyAnglePitch=Kalman1DOutput[1];
  
  neutralPositionAdjustment();

  DesiredAngleRoll=0.1*(ReceiverValue[0]-1500);
  DesiredAnglePitch=0.1*(ReceiverValue[1]-1500);
  InputThrottle=ReceiverValue[2];
  DesiredRateYaw=0.15*(ReceiverValue[3]-1500);

  ErrorAngleRoll=DesiredAngleRoll-KalmanAngleRoll;
  ErrorAnglePitch=DesiredAnglePitch-KalmanAnglePitch;

  pid_equation(ErrorAngleRoll, PAngleRoll, IAngleRoll, DAngleRoll, PrevErrorAngleRoll, PrevItermAngleRoll);     
  DesiredRateRoll=PIDReturn[0]; 
  PrevErrorAngleRoll=PIDReturn[1];
  PrevItermAngleRoll=PIDReturn[2];

  pid_equation(ErrorAnglePitch, PAnglePitch, IAnglePitch, DAnglePitch, PrevErrorAnglePitch, PrevItermAnglePitch);
  DesiredRatePitch=PIDReturn[0]; 
  PrevErrorAnglePitch=PIDReturn[1];
  PrevItermAnglePitch=PIDReturn[2];

  ErrorRateRoll=DesiredRateRoll-RateRoll;
  ErrorRatePitch=DesiredRatePitch-RatePitch;
  ErrorRateYaw=DesiredRateYaw-RateYaw;

  pid_equation(ErrorRateRoll, PRateRoll, IRateRoll, DRateRoll, PrevErrorRateRoll, PrevItermRateRoll);
       InputRoll=PIDReturn[0];
       PrevErrorRateRoll=PIDReturn[1]; 
       PrevItermRateRoll=PIDReturn[2];

  pid_equation(ErrorRatePitch, PRatePitch,IRatePitch, DRatePitch, PrevErrorRatePitch, PrevItermRatePitch);
       InputPitch=PIDReturn[0]; 
       PrevErrorRatePitch=PIDReturn[1]; 
       PrevItermRatePitch=PIDReturn[2];

  pid_equation(ErrorRateYaw, PRateYaw,IRateYaw, DRateYaw, PrevErrorRateYaw, PrevItermRateYaw);
       InputYaw=PIDReturn[0]; 
       PrevErrorRateYaw=PIDReturn[1]; 
       PrevItermRateYaw=PIDReturn[2];

  if (InputThrottle > 1800)
  {
    InputThrottle = 1800;
  }

  Serial.print(",InputTrotle:");
  Serial.print(InputThrottle);
  Serial.print(",InputRoll:");
  Serial.print(InputRoll);
  Serial.print(",InputPitch:");
  Serial.print(InputPitch);
  Serial.print(",InputYaw:");
  Serial.print(InputYaw);
  Serial.print("ROLL: ");
  Serial.print(ROLL);
  Serial.print(", PITCH: ");
  Serial.print(PITCH);
  Serial.print(", YAW: ");
  Serial.print(YAW);
  Serial.print(", TROTLE: ");
  Serial.println(TROTLE);

  MotorInput1 =  (InputThrottle - InputRoll - InputPitch - InputYaw); // front right - counter clockwise
  MotorInput2 =  (InputThrottle - InputRoll + InputPitch + InputYaw); // rear right - clockwise
  MotorInput3 =  (InputThrottle + InputRoll + InputPitch - InputYaw); // rear left  - counter clockwise
  MotorInput4 =  (InputThrottle + InputRoll - InputPitch + InputYaw); //front left - clockwise


  if (MotorInput1 > 2000)
  {
    MotorInput1 = 1999;
  }

  if (MotorInput2 > 2000)
  {
    MotorInput2 = 1999;
  }

  if (MotorInput3 > 2000)
  {
    MotorInput3 = 1999;
  }

  if (MotorInput4 > 2000)
  {
    MotorInput4 = 1999;
  }


  int ThrottleIdle = 1150;
  if (MotorInput1 < ThrottleIdle)
  {
    MotorInput1 = ThrottleIdle;
  }
  if (MotorInput2 < ThrottleIdle)
  {
    MotorInput2 = ThrottleIdle;
  }
  if (MotorInput3 < ThrottleIdle)
  {
    MotorInput3 = ThrottleIdle;
  }
  if (MotorInput4 < ThrottleIdle)
  {
    MotorInput4 = ThrottleIdle;
  }

  int ThrottleCutOff = 1000;
  if (ReceiverValue[2] < 1050)
  {
    MotorInput1 = ThrottleCutOff;
    MotorInput2 = ThrottleCutOff;
    MotorInput3 = ThrottleCutOff;
    MotorInput4 = ThrottleCutOff;
    reset_pid();
  }

  mot1.write(map(MotorInput1, 1000, 2000, 0, 180));
  mot2.write(map(MotorInput2, 1000, 2000, 0, 180));
  mot3.write(map(MotorInput3, 1000, 2000, 0, 180));
  mot4.write(map(MotorInput4, 1000, 2000, 0, 180));

  // voltage= (analogRead(36)/4096)*12.46*(35.9/36);
  // if(voltage<11.1)
  // {

  // }

  //Reciever signals
    // Serial.print(ReceiverValue[0]);
    // Serial.print(" - ");
    // Serial.print(ReceiverValue[1]);
    // Serial.print(" - ");
    // Serial.print(ReceiverValue[2]);
    // Serial.print(" - ");
    // Serial.print(ReceiverValue[3]);
    // Serial.print(" --- ");
  
  //   // Serial.print(ReceiverValue[4]);
  //   // Serial.print(" - ");
  //   // Serial.print(ReceiverValue[5]);
  //   // Serial.print(" - ");

  //Motor PWMs in us
    // Serial.print("MotVals-");
    // Serial.print(MotorInput1);
    // Serial.print("  ");
    // Serial.print(MotorInput2);
    // Serial.print("  ");
    // Serial.print(MotorInput3);
    // Serial.print("  ");
    // Serial.print(MotorInput4);
    // Serial.print(" -- ");

  // //Reciever translated rates
  //   Serial.print(DesiredRateRoll);
  //   Serial.print("  ");
  //   Serial.print(DesiredRatePitch);
  //   Serial.print("  ");
  //   Serial.print(DesiredRateYaw);
  //   Serial.print(" -- ");

  // //Gyro Rates
    // Serial.print(" Gyro rates:");
    // Serial.print(RateRoll);
    // Serial.print("  ");
    // Serial.print(RatePitch);
    // Serial.print("  ");
    // Serial.print(RateYaw);
    // Serial.print(" -- ");



  //PID outputs
  // Serial.print("PID O/P ");
  // Serial.print(InputPitch);
  //   Serial.print("  ");
  // Serial.print(InputRoll);
  //   Serial.print("  ");
  // Serial.print(InputYaw);
  //   Serial.print(" -- ");

  //Angles from MPU
    // Serial.print("AngleRoll:");
    // Serial.print(AngleRoll);
    // //serial.print("  ");
    //   Serial.print("AnglePitch:");
    // Serial.println(AnglePitch);

    
    //  Serial.println(" ");


 
  while (micros() - LoopTimer < (t*1000000));
  {
     LoopTimer = micros();

  }
}

void kalman_1d(float KalmanState, float KalmanUncertainty, float KalmanInput, float KalmanMeasurement) {
  KalmanState=KalmanState + (t*KalmanInput);
  KalmanUncertainty=KalmanUncertainty + (t*t*4*4); //here 4 is the vairnece of IMU i.e 4 deg/s
  float KalmanGain=KalmanUncertainty * 1/(1*KalmanUncertainty + 3 * 3); //std deviation of error is 3 deg
  KalmanState=KalmanState+KalmanGain * (KalmanMeasurement-KalmanState);
  KalmanUncertainty=(1-KalmanGain) * KalmanUncertainty;
  Kalman1DOutput[0]=KalmanState; 
  Kalman1DOutput[1]=KalmanUncertainty;
}

/* Tratamento de sinais RadioFrequencia
void channelInterruptHandler()
{
  current_time = micros();
  // Channel 1
  if (digitalRead(channel_1_pin))
  {
    if (last_channel_1 == 0)
    {
      last_channel_1 = 1;
      timer_1 = current_time;
    }
  }
  else if (last_channel_1 == 1)
  {
    last_channel_1 = 0;
    ReceiverValue[0] = current_time - timer_1;
  }

  // Channel 2
  if (digitalRead(channel_2_pin))
  {
    if (last_channel_2 == 0)
    {
      last_channel_2 = 1;
      timer_2 = current_time;
    }
  }
  else if (last_channel_2 == 1)
  {
    last_channel_2 = 0;
    ReceiverValue[1] = current_time - timer_2;
  }

  // Channel 3
  if (digitalRead(channel_3_pin))
  {
    if (last_channel_3 == 0)
    {
      last_channel_3 = 1;
      timer_3 = current_time;
    }
  }
  else if (last_channel_3 == 1)
  {
    last_channel_3 = 0;
    ReceiverValue[2] = current_time - timer_3;
  }

  // Channel 4
  if (digitalRead(channel_4_pin))
  {
    if (last_channel_4 == 0)
    {
      last_channel_4 = 1;
      timer_4 = current_time;
    }
  }
  else if (last_channel_4 == 1)
  {
    last_channel_4 = 0;
    ReceiverValue[3] = current_time - timer_4;
  }

  // Channel 5
  if (digitalRead(channel_5_pin))
  {
    if (last_channel_5 == 0)
    {
      last_channel_5 = 1;
      timer_5 = current_time;
    }
  }
  else if (last_channel_5 == 1)
  {
    last_channel_5 = 0;
    ReceiverValue[4] = current_time - timer_5;
  }

  // Channel 6
  if (digitalRead(channel_6_pin))
  {
    if (last_channel_6 == 0)
    {
      last_channel_6 = 1;
      timer_6 = current_time;
    }
  }
  else if (last_channel_6 == 1)
  {
    last_channel_6 = 0;
    ReceiverValue[5] = current_time - timer_6;
  }
}
*/

void neutralPositionAdjustment()
{
  return;
  int min = 1490;
  int max = 1510;
  if (ReceiverValue[0] < max && ReceiverValue[0] > min)
  {
    ReceiverValue[0]= 1500;
  } 
  if (ReceiverValue[1] < max && ReceiverValue[1] > min)
  {
    ReceiverValue[1]= 1500;
  } 
  if (ReceiverValue[3] < max && ReceiverValue[3] > min)
  {
    ReceiverValue[3]= 1500;
  } 
  if(ReceiverValue[0]==ReceiverValue[1] && ReceiverValue[1]==ReceiverValue[3] && ReceiverValue[3]==ReceiverValue[0] )
  {
    ReceiverValue[0]= 1500;
    ReceiverValue[1]= 1500;
    ReceiverValue[3]= 1500;
  }
}

void webSocketEvent(uint8_t num, WStype_t type, uint8_t * payload, size_t length) {
  if(type == WStype_TEXT) {
    // Recebe a mensagem do cliente
    String message = String((char*) payload);
    
    // Cria um documento JSON
    StaticJsonDocument<200> doc;

    // Tenta deserializar o JSON
    DeserializationError error = deserializeJson(doc, message);

    // Acessa os valores do JSON
    if (doc.containsKey("PID_P")){
      float pid_p = doc["PID_P"];
      writeFile(SPIFFS, "/pGain.txt", String(pid_p).c_str());
    }

    if (doc.containsKey("PID_I")){
      float pid_i = doc["PID_I"];
      writeFile(SPIFFS, "/iGain.txt", String(pid_i).c_str());
    }

    if (doc.containsKey("PID_D")){
      float pid_d = doc["PID_D"];
      writeFile(SPIFFS, "/dGain.txt", String(pid_d).c_str());
    }

    if (doc.containsKey("YAW_P")){
      float yaw_p = doc["YAW_P"];
      writeFile(SPIFFS, "/pYaw.txt", String(yaw_p).c_str());
    }

    if (doc.containsKey("YAW_I")){
      float yaw_i = doc["YAW_I"];
      writeFile(SPIFFS, "/iYaw.txt", String(yaw_i).c_str());
    }

    if (doc.containsKey("YAW_D")){
      float yaw_d = doc["YAW_D"];
      writeFile(SPIFFS, "/dYaw.txt", String(yaw_d).c_str());
    }

    if (doc.containsKey("PID_TIME")){
      float pid_time = doc["PID_TIME"];
      writeFile(SPIFFS, "/tc.txt", String(pid_time).c_str());
    }

    if (doc.containsKey("ROLL")){
      float roll = doc["ROLL"];
      ROLL = roll * 1000 + 1000;
    }

    ROLL = ROLL > 1811 ? 1811 : ROLL;
    ROLL = ROLL < 172 ? 172 : ROLL;

    if (doc.containsKey("PITCH")){
      float pitch = doc["PITCH"];
      PITCH = pitch * 1000 + 1000;
    }

    PITCH = PITCH > 1811 ? 1811 : PITCH;
    PITCH = PITCH < 172 ? 172 : PITCH;

    if (doc.containsKey("YAW")){
      float yaw = doc["YAW"];
      YAW = yaw * 1000 + 1000;
    }

    YAW = YAW > 1811 ? 1811 : YAW;
    YAW = YAW < 172 ? 172 : YAW;

    if (doc.containsKey("TROTLE")){
      float trotle = doc["TROTLE"];
      target = map(trunc(trotle * 1000), -1000, 1000, 48, 2047);
      //Serial.print("Convertido(float): ");
      //Serial.println(target);
      //if(trotle >= 0){
      //  target = (uint16_t) map(trotle * 1000, 0, 1000, 48, 2047); 
      //}

      TROTLE = trotle * 1000 + 1000;
      //Serial.print("TROTLE:");
      //Serial.println(TROTLE);
      //if (trotle > 0.1 || trotle < -0.1){
      //  TROTLE += trotle;
      //}
    }

    TROTLE = TROTLE > 1811 ? 1811 : TROTLE;
    TROTLE = TROTLE < 172 ? 172 : TROTLE;

    if (ENABLE != 992){
      TROTLE -= 5;
    }

    ReceiverValue[0] = ROLL;
    ReceiverValue[1] = PITCH;
    ReceiverValue[2] = TROTLE;
    ReceiverValue[3] = YAW;

    if (doc.containsKey("ENABLE")){
      bool enable = doc["ENABLE"];
      if (enable){
        ENABLE = 992;
      }else{
        ENABLE = 1400;
      }
    }

    /*
    Serial.print("ROLL: ");
    Serial.print(ROLL);
    Serial.print(", PITCH: ");
    Serial.print(PITCH);
    Serial.print(", YAW: ");
    Serial.print(YAW);
    Serial.print(", TROTLE: ");
    Serial.print(TROTLE);
    Serial.print(", ENABLE: ");
    Serial.println(ENABLE);
    */
  }
}

void RestAPI(){
  StaticJsonDocument<1024> jsonDocument;
  char buffer[1024];

  jsonDocument.clear(); // Clear json buffer
  JsonObject orientation = jsonDocument.to<JsonObject>();
  orientation["Yaw"] = 0;
  orientation["Pitch"] = AnglePitch;
  orientation["Roll"] = 0;

  serializeJson(jsonDocument, buffer);
  server.send(200, "application/json", buffer);
}

void gyro_signals(void)
{
  Wire.beginTransmission(0x68);
  Wire.write(0x1A);
  Wire.write(0x05);
  Wire.endTransmission();
  Wire.beginTransmission(0x68);
  Wire.write(0x1C);
  Wire.write(0x10);
  Wire.endTransmission();
  Wire.beginTransmission(0x68);
  Wire.write(0x3B);
  Wire.endTransmission(); 
  Wire.requestFrom(0x68,6);
  int16_t AccXLSB = Wire.read() << 8 | Wire.read();
  int16_t AccYLSB = Wire.read() << 8 | Wire.read();
  int16_t AccZLSB = Wire.read() << 8 | Wire.read();
  Wire.beginTransmission(0x68);
  Wire.write(0x1B); 
  Wire.write(0x8);
  Wire.endTransmission();                                                   
  Wire.beginTransmission(0x68);
  Wire.write(0x43);
  Wire.endTransmission();
  Wire.requestFrom(0x68,6);
  int16_t GyroX=Wire.read()<<8 | Wire.read();
  int16_t GyroY=Wire.read()<<8 | Wire.read();
  int16_t GyroZ=Wire.read()<<8 | Wire.read();
  RateRoll=(float)GyroZ/65.5;
  RatePitch=(float)GyroX/65.5;
  RateYaw=(float)GyroY/65.5;
  AccX=(float)AccXLSB/4096;
  AccY=(float)AccYLSB/4096;
  AccZ=(float)AccZLSB/4096;
  AccZ=AccZ-1; // calibration offset
  AngleRoll=atan(AccY/sqrt(AccX*AccX+AccZ*AccZ))*1/(3.142/180);
  AnglePitch=-atan(AccX/sqrt(AccY*AccY+AccZ*AccZ))*1/(3.142/180);
}

void pid_equation(float Error, float P, float I, float D, float PrevError, float PrevIterm)
{
  float Pterm = P * Error;
  float Iterm = PrevIterm +( I * (Error + PrevError) * (t/2));
  if (Iterm > 400)
  {
    Iterm = 400;
  }
  else if (Iterm < -400)
  {
  Iterm = -400;
  }
  float Dterm = D *( (Error - PrevError)/t);
  float PIDOutput = Pterm + Iterm + Dterm;
  if (PIDOutput > 400)
  {
    PIDOutput = 400;
  }
  else if (PIDOutput < -400)
  {
    PIDOutput = -400;
  }
  PIDReturn[0] = PIDOutput;
  PIDReturn[1] = Error;
  PIDReturn[2] = Iterm;
}

void reset_pid(void)
{
  PrevErrorRateRoll=0; PrevErrorRatePitch=0; PrevErrorRateYaw=0;
  PrevItermRateRoll=0; PrevItermRatePitch=0; PrevItermRateYaw=0;
  PrevErrorAngleRoll=0; PrevErrorAnglePitch=0;    
  PrevItermAngleRoll=0; PrevItermAnglePitch=0;
}

String readFile(fs::FS &fs, const char * path){
  Serial.printf("Reading file: %s\r\n", path);
  File file = fs.open(path, "r");
  if(!file || file.isDirectory()){
    Serial.println("- empty file or failed to open file");
    return String();
  }
  Serial.println("- read from file:");
  String fileContent;
  while(file.available()){
    fileContent+=String((char)file.read());
  }
  file.close();
  Serial.println(fileContent);
  return fileContent;
}

void writeFile(fs::FS &fs, const char * path, const char * message){
  Serial.printf("Writing file: %s\r\n", path);
  File file = fs.open(path, "w");
  if(!file){
    Serial.println("- failed to open file for writing");
    return;
  }
  if(file.print(message)){
    Serial.println("- file written");
  } else {
    Serial.println("- write failed");
  }
  file.close();
}

// Replaces placeholder with stored values
String processor(const String& var){
  //Serial.println(var);
  if(var == "pGain"){
      return readFile(SPIFFS, "/pGain.txt");
  }
  else if(var == "iGain"){
      return readFile(SPIFFS, "/iGain.txt");
  }
  else if(var == "dGain"){
      return readFile(SPIFFS, "/dGain.txt");
  }
  else if(var == "pYaw"){
      return readFile(SPIFFS, "/pYaw.txt");
  }
  else if(var == "dYaw"){
      return readFile(SPIFFS, "/dYaw.txt");
  }
  else if(var == "iYaw"){
      return readFile(SPIFFS, "/iYaw.txt");
  }
  else if(var == "tc"){
      return readFile(SPIFFS, "/tc.txt");
  }

}
