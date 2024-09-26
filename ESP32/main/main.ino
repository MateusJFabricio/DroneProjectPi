#include <WiFi.h>
#include <WebServer.h>
#include <WebSocketsServer.h>
#include <ArduinoJson.h>
#include <AsyncTCP.h>
#include <SPIFFS.h>

#include <Wire.h>
#include <ESP32Servo.h> // Change to the standard Servo library for ESP32
#include <SimpleKalmanFilter.h>
#include <ESPmDNS.h>

bool ESC_CALIB_ENABLED = false;

WebSocketsServer webSocket = WebSocketsServer(81);
WebServer server(80);

// REPLACE WITH YOUR NETWORK CREDENTIALS
const char* ssid = "ROBOSOFT";
const char* password = "robosoft";
unsigned long timeSpanLoop;
unsigned long timeSpanTaskAPI;
unsigned long timeSpanTaskSocket;
unsigned long lastPingTime = millis();
bool taskPingEnable = false;
int BeepCode = 0;

//Parameters
StaticJsonDocument<2000> Parameters;

const char* PARAM_TIME_CYCLE = "tc";  //Computation time cycle

//FlyController variables
struct ControlType{
  uint16_t ROLL; // de 1000 a 2000 com idle em 1500
  uint16_t PITCH; // de 1000 a 2000 com idle em 1500
  uint16_t YAW; // de 1000 a 2000 com idle em 1500
  uint16_t TROTLE; //incremental de 1000 a 2000
  bool ENABLE; //Habilita o Drone pra voo
  bool STOP; //Para imediatamente o drone
};

ControlType Control = {1500, 1500, 1500, 1000, false, false};

struct ModeType{
  bool Angle;
  bool Acro;
  bool EscCalibration;
  bool GyroCalibration;
  bool GyroAnalisys;
  bool ReturnHome;
};

ModeType Mode = {false, false, false, false, false};

volatile float RatePitch, RateRoll, RateYaw;
volatile float RateCalibrationPitch, RateCalibrationRoll, RateCalibrationYaw;
int RateCalibrationNumber;
unsigned long lastSampleMicros = 0, lastPrintMillis = 0;
float Gyroscope_x = 0, Gyroscope_y = 0, Gyroscope_z = 0;
float Accelerometer_x = 0, Accelerometer_y = 0, Accelerometer_z = 0;
float GyroX_CalibFactor = 0, GyroY_CalibFactor = 0, GyroZ_CalibFactor = 0;
float AccX_CalibFactor = 0, AccY_CalibFactor = 0, AccZ_CalibFactor = 0;

/*SimpleKalmanFilter(e_mea, e_est, q);
 e_mea: Measurement Uncertainty 
 e_est: Estimation Uncertainty 
 q: Process Noise
 */
SimpleKalmanFilter kalmanFilterGX(1, 1, 0.05);
SimpleKalmanFilter kalmanFilterGY(1, 1, 0.05);
SimpleKalmanFilter kalmanFilterGZ(1, 1, 0.05);
SimpleKalmanFilter kalmanFilterAX(1, 1, 10);
SimpleKalmanFilter kalmanFilterAY(1, 1, 10);
SimpleKalmanFilter kalmanFilterAZ(1, 1, 10);

#define INTERVAL_MS_PRINT 1000

float GainMotor1 = 1.0;
float GainMotor2 = 1.0;
float GainMotor3 = 1.0;
float GainMotor4 = 1.0;
float TrotleLimit = 1;

Servo mot1;
Servo mot2;
Servo mot3;
Servo mot4;
const int mot1_pin = 13;
const int mot2_pin = 12;
const int mot3_pin = 14;
const int mot4_pin = 27;

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
volatile float AccX, AccY, AccZ, GyroX, GyroY, GyroZ;
volatile float AngleRoll, AnglePitch, AngleYaw;
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
  timeSpanLoop = micros();
  timeSpanTaskAPI = micros();
  timeSpanTaskSocket = micros();

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

  ReadParameters();

  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password);
  while(WiFi.waitForConnectResult() != WL_CONNECTED) {
    Serial.println("WiFi Failed!");
    delay(1000);
  }
  Serial.println();
  Serial.print("IP Address: ");
  Serial.println(WiFi.localIP());

  // Inicia o mDNS e registra o nome do host (por exemplo, "drone")
  if (!MDNS.begin("drone")) {
    Serial.println
    ("Error starting mDNS");
    return;
  }

  //Configura MPU6050
  Serial.println("Configurano o MPU6050");

  Wire.setClock(400000);
  Wire.begin();
  delay(250);
  Wire.beginTransmission(0x68);
  Wire.write(0x6B); //PWR_MGMT_1
  Wire.write(0x00); //Internal 8MHz oscillator 
  Wire.endTransmission();

  Wire.beginTransmission(0x68);
  Wire.write(0x19); //Register 15: SRD
  Wire.write(0x01); //1
  Wire.endTransmission();
  
  Wire.beginTransmission(0x68);
  Wire.write(0x1A); //CONFIG
  Wire.write(0x01); //DLPF_CFG Código 5
  Wire.endTransmission();

  Wire.beginTransmission(0x68);
  Wire.write(0x1B); // GYRO_CONFIG
  Wire.write(0x10); // +-1000 °/s
  Wire.endTransmission();

  Wire.beginTransmission(0x68);
  Wire.write(0x1C); //ACCEL_CONFIG
  Wire.write(0x00); // +-2g
  Wire.endTransmission();

  Serial.println("Finalizado a configuracao do MPU6050");

  //Configura Motor
  Serial.println("Configurando motores");
  mot1.attach(mot1_pin,1000,2000);
  mot2.attach(mot2_pin,1000,2000);
  mot3.attach(mot3_pin,1000,2000);
  mot4.attach(mot4_pin,1000,2000);
  Serial.println("Finalizado as configuracoes dos motores");

  //to stop esc from beeping
  mot1.write(0);
  mot2.write(0);
  mot3.write(0);
  mot4.write(0);

  //Beep de inicializacao
  pinMode(15, OUTPUT);

  RateCalibrationRoll /= 4000;
  RateCalibrationPitch /= 4000;
  RateCalibrationYaw /= 4000;

  LoopTimer = micros();

  // Inicia o WebSocket server
  Serial.println("Iniciando o wesocket");
  webSocket.begin();
  webSocket.onEvent(webSocketEvent);

  server.enableCORS();
  server.on("/getValues", APIGetAngles);
  server.on("/getBackup", APIGetBackup);
  server.on("/getTrotleLimit", APIGetTrotleLimit);
  server.on("/getGainMotors", APIGetGainMotors);
  server.on("/getDroneStatus", APIGetStatusDrone);
  //server.on("/getEscCalibration", APIGetEscCalibration);
  server.on("/postAnglesCalibration", HTTP_POST, APIPostAnglesCalibration);
  server.on("/postGainMotors", HTTP_POST, APIPostGainMotors);
  server.on("/postRestoreParameters", HTTP_POST, APIPostRestoreParameters);
  server.on("/postGainMotors", HTTP_OPTIONS, APIOptions);
  server.on("/postTrotleLimit", HTTP_POST, APIPostTrotleLimit);
  server.on("/postFlightMode", HTTP_POST, APIPostFlightMode);
  //server.on("/postEscCalibration", HTTP_POST, APIPostEscCalibration);
  server.begin();

  Serial.println("Inicializa as tasks");
  xTaskCreatePinnedToCore(
      TaskWebServer, /* Function to implement the task */
      "TaskWebServer", /* Name of the task */
      20000,  /* Stack size in words */
      NULL,  /* Task input parameter */
      1,  /* Priority of the task */
      NULL,  /* Task handle. */
      1);

  xTaskCreatePinnedToCore(
      TaskWebServerAPI, /* Function to implement the task */
      "TaskWebServerAPI", /* Name of the task */
      20000,  /* Stack size in words */
      NULL,  /* Task input parameter */
      1,  /* Priority of the task */
      NULL,  /* Task handle. */
      1);
  xTaskCreatePinnedToCore(
      TaskBeep, /* Function to implement the task */
      "TaskBeep", /* Name of the task */
      1000,  /* Stack size in words */
      NULL,  /* Task input parameter */
      1,  /* Priority of the task */
      NULL,  /* Task handle. */
      1);

  GainMotor1 = readFile(SPIFFS, "/motorGainM1.txt").toFloat();
  GainMotor2 = readFile(SPIFFS, "/motorGainM2.txt").toFloat();
  GainMotor3 = readFile(SPIFFS, "/motorGainM3.txt").toFloat();
  GainMotor4 = readFile(SPIFFS, "/motorGainM4.txt").toFloat();

  TrotleLimit = readFile(SPIFFS, "/trotleLimit.txt").toFloat();

  GyroX_CalibFactor = readFile(SPIFFS, "/GyroX_CalibFactor.txt").toFloat();
  GyroY_CalibFactor = readFile(SPIFFS, "/GyroY_CalibFactor.txt").toFloat();
  GyroZ_CalibFactor = readFile(SPIFFS, "/GyroZ_CalibFactor.txt").toFloat();
  AccX_CalibFactor = readFile(SPIFFS, "/AccX_CalibFactor.txt").toFloat();
  AccY_CalibFactor = readFile(SPIFFS, "/AccY_CalibFactor.txt").toFloat();
  AccZ_CalibFactor = readFile(SPIFFS, "/AccZ_CalibFactor.txt").toFloat();
  
}

void loop(void) {
  
  //Modo de calibracao do ESC
  if (Mode.EscCalibration){
    EscCalibrationMode();
  }

  if (Mode.Angle){
    AngleMode();
  }

  if (Mode.Acro){
    AcroMode();
  }

  if (Mode.GyroCalibration){
    CalibrarAngulos();
  }

  if (Mode.GyroAnalisys){
    GyroAnalisys();
  }

  if (Mode.ReturnHome){
    ReturnHomeMode();
  }
}

bool isConnected(){
  return (millis() - lastPingTime) < 2000;
}

void TaskPing(int id, unsigned long *timeSpan){
  if(taskPingEnable){
    String p = ",TimeSpan(" + String(id) + "):";
    Serial.print(p);
    Serial.println(micros() - *timeSpan);
    *timeSpan = micros();
  }
}
void TaskWebServer( void * parameter) {
  while(true){
    //Roda a API
    server.handleClient();

    if (!isConnected()){
      BeepCode = 2;
      ReturnHomeMode();
    }else{
      BeepCode = 0;
    }
  }
}

void TaskWebServerAPI( void * parameter) {
  while(true){
    // Mantém o WebSocket ativo
    webSocket.loop();

    TaskPing(2, &timeSpanTaskAPI);
  }
}
void TaskBeep(void * parameter){
  while(true){
    Beep(BeepCode);
  }
}
void Beep(int code){
  int pinBuzzer = 15;
  int led_time=100;
  switch(code){
    case 0:
      break;
    case 1: //Inicializacao
      tone(pinBuzzer, 2273, 100);
      delay(50);
      tone(pinBuzzer, 2273, 100);
      delay(50);
      tone(pinBuzzer, 2273, 100);
      break;
    case 2: //Sem conexao
      tone(pinBuzzer, 2273, 100);
      delay(100);
      tone(pinBuzzer, 2273, 100);
      delay(1000);
      break;
    default: //ERRO
      tone(pinBuzzer, 2273, 100);
      delay(500);
      tone(pinBuzzer, 2273, 100);
      break;
  }
}

void GyroAnalisys(){
  Gyro_signals();

  if (Control.ENABLE){
    mot1.write(map(Control.TROTLE, 1500, 2000, 0, 180));
    mot2.write(map(Control.TROTLE, 1500, 2000, 0, 180));
    mot3.write(map(Control.TROTLE, 1500, 2000, 0, 180));
    mot4.write(map(Control.TROTLE, 1500, 2000, 0, 180));
  }else{
    mot1.write(0);
    mot2.write(0);
    mot3.write(0);
    mot4.write(0);
  }

  Serial.print(",AnglePitch:");
  Serial.print(AnglePitch);
  Serial.print(",AngleRoll:");
  Serial.print(AngleRoll);
  Serial.print(",AngleYaw:");
  Serial.print(AngleYaw);
  Serial.print(",MenosCem:");
  Serial.print(-50);
  Serial.print(",Zero:");
  Serial.print(0);
  Serial.print(",Cem:");
  Serial.print(50);
  Serial.println();
}
void EscCalibrationMode(){
  if (Control.ENABLE){
    mot1.write(map(Control.TROTLE, 1500, 2000, 0, 180));
    mot2.write(map(Control.TROTLE, 1500, 2000, 0, 180));
    mot3.write(map(Control.TROTLE, 1500, 2000, 0, 180));
    mot4.write(map(Control.TROTLE, 1500, 2000, 0, 180));
  }else{
    mot1.write(0);
    mot2.write(0);
    mot3.write(0);
    mot4.write(0);
  }
}

void AcroMode(){
  return;
  float AnguloMax_Pitch = 10;
  float AnguloMax_Roll = 10;
  float AnguloBySec_Yaw = 10;

  neutralPositionAdjustment();

  InputPitch=map(trunc(Control.PITCH), 1000, 2000, AnguloMax_Pitch * -1, AnguloMax_Pitch);
  InputRoll=map(trunc(Control.ROLL), 1000, 2000, AnguloMax_Roll * -1, AnguloMax_Roll);
  InputYaw=map(trunc(Control.YAW), 1000, 2000, AnguloBySec_Yaw * -1, AnguloBySec_Yaw);

  MotorInput1 = InputThrottle - InputPitch + InputRoll - InputYaw;
  MotorInput2 = InputThrottle - InputPitch - InputRoll + InputYaw;
  MotorInput3 = InputThrottle + InputPitch + InputRoll + InputYaw;
  MotorInput4 = InputThrottle + InputPitch - InputRoll - InputYaw;
}

void AngleMode(){
  Gyro_signals();

  neutralPositionAdjustment();

  float AnguloMax_Pitch = 10;
  float AnguloMax_Roll = 10;
  float AnguloBySec_Yaw = 10;
  
  DesiredAnglePitch=map(trunc(Control.PITCH), 1000, 2000, -10, 10);
  DesiredAngleRoll=map(trunc(Control.ROLL), 1000, 2000, -10, 10);
  DesiredRateYaw=map(trunc(Control.YAW), 1000, 2000, -10, 10);
  InputThrottle=map(trunc(Control.TROTLE), 1500, 2000, 0, 100);

  ErrorAnglePitch = AnglePitch - DesiredAnglePitch;
  ErrorAngleRoll = AngleRoll - DesiredAngleRoll;

  InputPitch = ErrorAnglePitch * 0.5;
  InputRoll = ErrorAngleRoll * 0.5;
  InputYaw = DesiredRateYaw * 0.5;
  
  MotorInput1 = InputThrottle - InputPitch + InputRoll - InputYaw;
  MotorInput2 = InputThrottle - InputPitch - InputRoll + InputYaw;
  MotorInput3 = InputThrottle + InputPitch + InputRoll + InputYaw;
  MotorInput4 = InputThrottle + InputPitch - InputRoll - InputYaw;
  
  if (Control.ENABLE){
    MotorInput1 = range(MotorInput1 * GainMotor1, 0, 100);
    mot1.write(map(MotorInput1, 0, 100, 0, 180));

    MotorInput2 = range(MotorInput2 * GainMotor2, 0, 100);
    mot2.write(map(MotorInput2, 0, 100, 0, 180));

    MotorInput3 = range(MotorInput3 * GainMotor3, 0, 100);
    mot3.write(map(MotorInput3, 0, 100, 0, 180));

    MotorInput4 = range(MotorInput4 * GainMotor4, 0, 100);
    mot4.write(map(MotorInput4, 0, 100, 0, 180));

  }else{
    mot1.write(0);
    mot2.write(0);
    mot3.write(0);
    mot4.write(0);
  }

  Serial.print(",Cem:");
  Serial.print(100);
  Serial.print(",MenosCem:");
  Serial.print(-100);
  Serial.print(",Zero:");
  Serial.print(0);
  /*
  Serial.print(",InputThrottle:");
  Serial.print(InputThrottle);
  Serial.print(",InputPitch:");
  Serial.print(InputPitch);
  Serial.print(",InputRoll:");
  Serial.print(InputRoll);
  Serial.print(",InputYaw:");
  Serial.print(InputYaw);
  */
  Serial.print(",MotorInput1:");
  Serial.print(MotorInput1);
  Serial.print(",MotorInput2:");
  Serial.print(MotorInput2);
  Serial.print(",MotorInput3:");
  Serial.print(MotorInput3);
  Serial.print(",MotorInput4:");
  Serial.println(MotorInput4);
  /*
  Serial.print(",DesiredAnglePitch:");
  Serial.print(DesiredAnglePitch);
  Serial.print(",DesiredAngleRoll:");
  Serial.print(DesiredAngleRoll);
  Serial.print(",DesiredRateYaw:");
  Serial.println(DesiredRateYaw);
  */
}

void ReturnHomeMode(){
  Control.YAW = 1500;
  Control.PITCH = 1500;
  Control.ROLL = 1500;
  Control.TROTLE -= 1;
  
  if (Control.TROTLE < 1000){
    Control.TROTLE = 1000;
  }
}

void FlyController(){
  if(Control.TROTLE < 1030 && Control.ENABLE )
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
  Gyro_signals();
  RateRoll -= RateCalibrationRoll;
  RatePitch -= RateCalibrationPitch;
  RateYaw -= RateCalibrationYaw;

  kalman_1d(KalmanAngleRoll, KalmanUncertaintyAngleRoll, RateRoll, AngleRoll);
  KalmanAngleRoll=Kalman1DOutput[0]; KalmanUncertaintyAngleRoll=Kalman1DOutput[1];
  kalman_1d(KalmanAnglePitch, KalmanUncertaintyAnglePitch, RatePitch, AnglePitch);
  KalmanAnglePitch=Kalman1DOutput[0]; KalmanUncertaintyAnglePitch=Kalman1DOutput[1];
  
  neutralPositionAdjustment();

  DesiredAngleRoll=0.1*(Control.ROLL-1500);
  DesiredAnglePitch=0.1*(Control.PITCH-1500);
  InputThrottle=Control.TROTLE;
  DesiredRateYaw=0.15*(Control.YAW-1500);

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

  /*
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
  */

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
  if (Control.TROTLE < 1050)
  {
    MotorInput1 = ThrottleCutOff;
    MotorInput2 = ThrottleCutOff;
    MotorInput3 = ThrottleCutOff;
    MotorInput4 = ThrottleCutOff;
    reset_pid();
  }

  /*
  mot1.write(map(MotorInput1, 1000, 2000, 0, 180));
  mot2.write(map(MotorInput2, 1000, 2000, 0, 180));
  mot3.write(map(MotorInput3, 1000, 2000, 0, 180));
  mot4.write(map(MotorInput4, 1000, 2000, 0, 180));
  mot1.write(map(Control.TROTLE, 1000, 2000, 0, 180));
  mot2.write(map(Control.TROTLE, 1000, 2000, 0, 180));
  mot3.write(map(Control.TROTLE, 1000, 2000, 0, 180));
  mot4.write(map(Control.TROTLE, 1000, 2000, 0, 180));

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
  /*
     Serial.print(",MotorInput1:");
     Serial.print(MotorInput1);
     Serial.print(",MotorInput2:");
     Serial.print(MotorInput2);
     Serial.print(",MotorInput3:");
     Serial.print(MotorInput3);
     Serial.print(",MotorInput4:");
     Serial.println(MotorInput4);
  */
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

void neutralPositionAdjustment()
{
  int min = 1490;
  int max = 1510;
  if (Control.ROLL < max && Control.ROLL > min)
  {
    Control.ROLL= 1500;
  } 

  if (Control.YAW < max && Control.YAW > min)
  {
    Control.YAW= 1500;
  } 

  if (Control.PITCH < max && Control.PITCH > min)
  {
    Control.PITCH= 1500;
  }

  if (Control.TROTLE < max && Control.TROTLE > min)
  {
    Control.TROTLE= 1500;
  }
}

void webSocketEvent(uint8_t num, WStype_t type, uint8_t * payload, size_t length) {
  if(type != WStype_DISCONNECTED || type != WStype_ERROR){
    lastPingTime = millis();
  }
  
  if(type == WStype_TEXT) {
    // Recebe a mensagem do cliente
    String message = String((char*) payload);
    
    // Cria um documento JSON
    StaticJsonDocument<200> doc;

    // Tenta deserializar o JSON
    DeserializationError error = deserializeJson(doc, message);
    switch (error.code()) {
    case DeserializationError::Ok:
        break;
    case DeserializationError::InvalidInput:
        Serial.println("Invalid input!");
        return;
    case DeserializationError::NoMemory:
        Serial.println("Not enough memory");
        return;
    default:
        Serial.println("Deserialization failed");
        return;
    }

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
      Control.ROLL = map(trunc(roll * 1000), -1000, 1000, 1000, 2000);
    }

    if (doc.containsKey("PITCH")){
      float pitch = doc["PITCH"];
      Control.PITCH = map(trunc(pitch * 1000), -1000, 1000, 1000, 2000);
    }

    if (doc.containsKey("YAW")){
      float yaw = doc["YAW"];
      Control.YAW = map(trunc(yaw * 1000), -1000, 1000, 1000, 2000);
    }

    if (doc.containsKey("TROTLE")){
      float trotle = doc["TROTLE"];
      const float trotleFactor = 200;
      Control.TROTLE = map(trunc(trotle * 1000), -1000, 1000, 1000, 2000);
      Control.TROTLE = Control.TROTLE > 2000 ? 2000 : Control.TROTLE;
      Control.TROTLE = Control.TROTLE < 1000 ? 1000 : Control.TROTLE;
    }

    if (!Control.ENABLE){
      Control.TROTLE -= 5;
    }

    if (doc.containsKey("ENABLE")){
      bool enable = doc["ENABLE"];

      if (!Control.ENABLE && enable){
        if (Mode.EscCalibration){
          Control.ENABLE = true;
        }else{
          if (Control.TROTLE < 1100){
            Control.ENABLE = true;
          }
        }
      }

      if (!enable){
        Control.ENABLE = false;
      }
    }

    //Stop Drone
    if (doc.containsKey("STOP")){
      bool stop = doc["STOP"];
      Control.STOP = stop;
      if(Control.STOP){
        Control.ENABLE = false;
        Control.TROTLE = 1000;
      }
    }
  }
}

void APIPostAnglesCalibration(){
  if (!Control.ENABLE){
    CalibrarAngulos();
    // Resposta ao cliente
    server.send(200, "application/json", "{\"status\":\"sucesso\",\"msg\":\"Calibracao concluida\"}");
  }else{
    server.send(400, "application/json", "{\"status\":\"erro\",\"msg\":\"O drone deve estar desarmado\"}");
  }
}

void APIGetStatusDrone(){
  
  StaticJsonDocument<1024> jsonDocument;
  char buffer[1024];

  jsonDocument.clear(); // Clear json buffer
  JsonObject status = jsonDocument.to<JsonObject>();
  status["DRONE_ARMADO"] = Control.ENABLE;
  status["Angle"] = Mode.Angle;
  status["Acro"] = Mode.Acro;
  status["EscCalibration"] = Mode.EscCalibration;

  status["GyroCalibration"] = Mode.GyroCalibration;
  status["GyroAnalisys"] = Mode.GyroAnalisys;
  status["ReturnHome"] = Mode.ReturnHome;

  serializeJson(jsonDocument, buffer);
  server.send(200, "application/json", buffer);
  
}

void APIGetGainMotors(){
  StaticJsonDocument<1024> jsonDocument;
  char buffer[1024];

  jsonDocument.clear(); // Clear json buffer
  JsonObject gains = jsonDocument.to<JsonObject>();
  gains["GANHO_M1"] = readFile(SPIFFS, "/motorGainM1.txt").toFloat();
  gains["GANHO_M2"] = readFile(SPIFFS, "/motorGainM2.txt").toFloat();
  gains["GANHO_M3"] = readFile(SPIFFS, "/motorGainM3.txt").toFloat();
  gains["GANHO_M4"] = readFile(SPIFFS, "/motorGainM4.txt").toFloat();

  serializeJson(jsonDocument, buffer);
  server.send(200, "application/json", buffer);
}

void APIGetTrotleLimit(){
  StaticJsonDocument<1024> jsonDocument;
  char buffer[1024];

  jsonDocument.clear(); // Clear json buffer
  JsonObject json = jsonDocument.to<JsonObject>();
  json["TROTLE_LIMIT"] = readFile(SPIFFS, "/trotleLimit.txt").toFloat();

  serializeJson(jsonDocument, buffer);
  server.send(200, "application/json", buffer);
}
void APIGetBackup(){
  char buffer[2000];

  serializeJson(Parameters, buffer);
  server.send(200, "application/json", buffer);
}
void APIGetAngles(){
  StaticJsonDocument<1024> jsonDocument;
  char buffer[1024];

  jsonDocument.clear(); // Clear json buffer
  JsonObject orientation = jsonDocument.to<JsonObject>();
  orientation["Yaw"] = (int)AngleYaw;
  orientation["Pitch"] = (int)AnglePitch;
  orientation["Roll"] = (int)AngleRoll;

  orientation["GyroX"] = (int)Gyroscope_x;
  orientation["GyroY"] = (int)Gyroscope_y;
  orientation["GyroZ"] = (int)Gyroscope_z;

  orientation["AccX"] = (int)Accelerometer_x;
  orientation["AccY"] = (int)Accelerometer_y;
  orientation["AccZ"] = (int)Accelerometer_z;

  serializeJson(jsonDocument, buffer);
  server.send(200, "application/json", buffer);
}

void APIOptions(){
  server.sendHeader("Access-Control-Allow-Origin", "*");
  server.sendHeader("Access-Control-Allow-Methods", "POST, GET, OPTIONS");
  server.sendHeader("Access-Control-Allow-Headers", "Content-Type");
  server.send(204);
}

void APIPostGainMotors() {
  if (server.hasArg("plain")) {
    String json = server.arg("plain");  // Recebe o corpo da requisição

    // Cria um objeto JSON para armazenar os dados recebidos
    StaticJsonDocument<200> doc;

    // Deserializa o JSON recebido
    DeserializationError error = deserializeJson(doc, json);

    if (error) {
      Serial.print("Erro ao parsear JSON: ");
      Serial.println(error.c_str());
      server.send(400, "application/json", "{\"status\":\"erro\",\"msg\":\"JSON inválido\"}");
      return;
    }

    // Exemplo de como acessar os campos do JSON
    GainMotor1 = doc["GANHO_M1"];
    GainMotor2 = doc["GANHO_M2"];
    GainMotor3 = doc["GANHO_M3"];
    GainMotor4 = doc["GANHO_M4"];

    writeFile(SPIFFS, "/motorGainM1.txt", String(GainMotor1).c_str());
    writeFile(SPIFFS, "/motorGainM2.txt", String(GainMotor2).c_str());
    writeFile(SPIFFS, "/motorGainM3.txt", String(GainMotor3).c_str());
    writeFile(SPIFFS, "/motorGainM4.txt", String(GainMotor4).c_str());

    // Resposta ao cliente
    //server.sendHeader("Access-Control-Allow-Origin", "*");
    server.send(200, "application/json", "{\"status\":\"sucesso\",\"msg\":\"JSON recebido com sucesso\"}");
  } else {
    server.send(400, "application/json", "{\"status\":\"erro\",\"msg\":\"Corpo vazio no POST\"}");
  }
}

void APIPostRestoreParameters(){
  if (server.hasArg("plain")) {
    String json = server.arg("plain");  // Recebe o corpo da requisição

    // Cria um objeto JSON para armazenar os dados recebidos
    StaticJsonDocument<2000> doc;

    // Deserializa o JSON recebido
    DeserializationError error = deserializeJson(doc, json);

    if (error) {
      Serial.print("Erro ao parsear JSON: ");
      Serial.println(error.c_str());
      server.send(400, "application/json", "{\"status\":\"erro\",\"msg\":\"JSON inválido\"}");
      return;
    }

    writeFile(SPIFFS, "/Parameters.json", String(json).c_str());

    // Resposta ao cliente
    server.send(200, "application/json", "{\"status\":\"sucesso\",\"msg\":\"JSON recebido com sucesso\"}");
  } else {
    server.send(400, "application/json", "{\"status\":\"erro\",\"msg\":\"Corpo vazio no POST\"}");
  }
}
void APIPostTrotleLimit() {
  if (server.hasArg("plain")) {
    String json = server.arg("plain");  // Recebe o corpo da requisição

    // Cria um objeto JSON para armazenar os dados recebidos
    StaticJsonDocument<200> doc;

    // Deserializa o JSON recebido
    DeserializationError error = deserializeJson(doc, json);

    if (error) {
      Serial.print("Erro ao parsear JSON: ");
      Serial.println(error.c_str());
      server.send(400, "application/json", "{\"status\":\"erro\",\"msg\":\"JSON inválido\"}");
      return;
    }

    // Exemplo de como acessar os campos do JSON
    TrotleLimit = doc["TROTLE_LIMIT"];

    writeFile(SPIFFS, "/trotleLimit.txt", String(TrotleLimit).c_str());

    // Resposta ao cliente
    server.send(200, "application/json", "{\"status\":\"sucesso\",\"msg\":\"JSON recebido com sucesso\"}");
  } else {
    server.send(400, "application/json", "{\"status\":\"erro\",\"msg\":\"Corpo vazio no POST\"}");
  }
}
void APIPostFlightMode(){
  if (server.hasArg("plain")) {
    String json = server.arg("plain");  // Recebe o corpo da requisição

    // Cria um objeto JSON para armazenar os dados recebidos
    StaticJsonDocument<200> doc;

    // Deserializa o JSON recebido
    DeserializationError error = deserializeJson(doc, json);

    if (error) {
      Serial.print("Erro ao parsear JSON: ");
      Serial.println(error.c_str());
      server.send(400, "application/json", "{\"status\":\"erro\",\"msg\":\"JSON inválido\"}");
      return;
    }

    //Exemplo de como acessar os campos do JSON
    Mode.Angle            = doc["Angle"];
    Mode.Acro             = doc["Acro"];
    Mode.EscCalibration   = doc["EscCalibration"];
    Mode.GyroCalibration  = doc["GyroCalibration"];
    Mode.GyroAnalisys     = doc["GyroAnalisys"];
    Mode.ReturnHome       = doc["ReturnHome"];

    // Resposta ao cliente
    server.send(200, "application/json", "{\"status\":\"sucesso\",\"msg\":\"JSON recebido com sucesso\"}");
  } else {
    server.send(400, "application/json", "{\"status\":\"erro\",\"msg\":\"Corpo vazio no POST\"}");
  }
}
void Gyro_signals()
{
  unsigned long sampleMicros = (lastSampleMicros > 0) ? micros() - lastSampleMicros : 0;
  lastSampleMicros = micros();

  //Request Acc data
  Wire.beginTransmission(0x68);
  Wire.write(0x3B); //Request Data
  Wire.endTransmission(); 
  Wire.requestFrom(0x68,6);

  int16_t AccXLSB = Wire.read() << 8 | Wire.read();
  int16_t AccYLSB = Wire.read() << 8 | Wire.read();
  int16_t AccZLSB = Wire.read() << 8 | Wire.read();
  
  //Request Gyro data
  Wire.beginTransmission(0x68);
  Wire.write(0x43);
  Wire.endTransmission();
  Wire.requestFrom(0x68,6);

  int16_t gX=Wire.read()<<8 | Wire.read();
  int16_t gY=Wire.read()<<8 | Wire.read();
  int16_t gZ=Wire.read()<<8 | Wire.read();

  //Sensibilide do gyro
  // ± 250 °/s : 131 LSB/°/s  
  // ± 500 °/s : 65.5 LSB/°/s  
  // ± 1000 °/s: 32.8 LSB/°/s
  // ± 2000 °/s: 16.4 LSB/°/s 

  GyroX=((float)gX/32.8);
  GyroY=((float)gY/32.8);
  GyroZ=((float)gZ/32.8) * -1;

  //Sensibilide do acc
  // +- 2g : 16384 LSB/g
  // +- 4g : 8192 LSB/g
  // +- 8g : 4096 LSB/g
  // +- 16g: 2048 LSB/g

  AccX=(float)AccXLSB / 16384; 
  AccY=(float)AccYLSB / 16384;
  AccZ=(float)AccZLSB / 16384;

  AccX = AccX * 9.80665;
  AccY = AccY * 9.80665;
  AccZ = AccZ * 9.80665;

  Accelerometer_x=atan(AccY / sqrt(sq(AccX) + sq(AccZ))) - AccX_CalibFactor; //Ao longo do eixo X
  Accelerometer_y=atan(-1 * AccX / sqrt(sq(AccY) + sq(AccZ))) - AccY_CalibFactor; //Ao longo do eixo Y
  Accelerometer_z=atan2(Accelerometer_y, Accelerometer_x) - AccZ_CalibFactor;

  kalmanFilterGX.updateEstimate(GyroX);
  kalmanFilterGY.updateEstimate(GyroY);
  kalmanFilterGZ.updateEstimate(GyroZ);
  kalmanFilterAX.updateEstimate(Accelerometer_x);
  kalmanFilterAY.updateEstimate(Accelerometer_y);
  kalmanFilterAZ.updateEstimate(Accelerometer_x);

  Accelerometer_x = degrees(kalmanFilterAX.getEstimate());
  Accelerometer_y = degrees(kalmanFilterAY.getEstimate());
  Accelerometer_z = degrees(kalmanFilterAZ.getEstimate());

  Gyroscope_x += (kalmanFilterGX.getEstimate() - GyroX_CalibFactor) * sampleMicros / 1000000;
  Gyroscope_y += (kalmanFilterGY.getEstimate() - GyroY_CalibFactor) * sampleMicros / 1000000;
  Gyroscope_z += (kalmanFilterGZ.getEstimate() - GyroZ_CalibFactor) * sampleMicros / 1000000;
  AnglePitch = Accelerometer_x;//(Gyroscope_x + Accelerometer_x) / 2;
  AnglePitch *= -1;
  AngleRoll = Accelerometer_y;//(Gyroscope_y + Accelerometer_y) / 2;
  AngleYaw = Gyroscope_z;

  /*
  Serial.print(",AnglePitch:");
  Serial.print(AnglePitch);
  Serial.print(",AngleRoll:");
  Serial.print(AngleRoll);
  Serial.print(",AngleYaw:");
  Serial.print(AngleYaw);
  Serial.print(",MenosCem:");
  Serial.print(-50);
  Serial.print(",Zero:");
  Serial.print(0);
  Serial.print(",Cem:");
  Serial.print(50);
  Serial.println();

  Serial.print(",Kalman:");
  Serial.print(kalman);
  Serial.print(",accelerometer_x:");
  Serial.print(degrees(accelerometer_x));
  Serial.print(",AnglePitch:");
  Serial.println(degrees(gyroscope_y));
  */

}

void CalibrarAngulos(){
  float lastValueX = 0, lastValueY = 0, lastValueZ = 0;
  Serial.println("Iniciando a calibracao do MPU6050");

  //Zera os angulos
  Gyroscope_x = 0;
  Gyroscope_y = 0;
  Gyroscope_z = 0;

  GyroX_CalibFactor = 0;
  GyroY_CalibFactor = 0;
  GyroZ_CalibFactor = 0;

  AccX_CalibFactor = 0;
  AccY_CalibFactor = 0;
  AccZ_CalibFactor = 0;

  //Salva a calibracao
  writeFile(SPIFFS, "/GyroX_CalibFactor.txt", String(GyroX_CalibFactor).c_str());
  writeFile(SPIFFS, "/GyroY_CalibFactor.txt", String(GyroY_CalibFactor).c_str());
  writeFile(SPIFFS, "/GyroZ_CalibFactor.txt", String(GyroZ_CalibFactor).c_str());
  writeFile(SPIFFS, "/AccX_CalibFactor.txt", String(AccX_CalibFactor).c_str());
  writeFile(SPIFFS, "/AccY_CalibFactor.txt", String(AccY_CalibFactor).c_str());
  writeFile(SPIFFS, "/AccZ_CalibFactor.txt", String(AccZ_CalibFactor).c_str());
  
  float calibGyroX = 0, calibGyroY = 0, calibGyroZ = 0;
  float calibAccX = 0, calibAccY = 0, calibAccZ = 0;

  //Executa a calibracao
  for (int i = 0; i < 500; i++) {
    Gyro_signals();

    calibGyroX += kalmanFilterGX.getEstimate();
    calibGyroY += kalmanFilterGY.getEstimate();
    calibGyroZ += kalmanFilterGZ.getEstimate();

    calibAccX += kalmanFilterAX.getEstimate();
    calibAccY += kalmanFilterAY.getEstimate();
    calibAccZ += kalmanFilterAZ.getEstimate();
    
    delay(1);
  }

  calibGyroX /= 500;
  calibGyroY /= 500;
  calibGyroZ /= 500;

  calibAccX /= 500;
  calibAccY /= 500;
  calibAccZ /= 500;

  Serial.print(",CalibAccX:");
  Serial.print(calibAccX);
  Serial.print(",CalibAccY:");
  Serial.print(calibAccY);
  Serial.print(",CalibAccZ:");
  Serial.print(calibAccZ);

  Serial.print(",CalibGyroX:");
  Serial.print(calibGyroX);
  Serial.print(",CalibGyroY:");
  Serial.print(calibGyroY);
  Serial.print(",CalibGyroZ:");
  Serial.println(calibGyroZ);

  //Salva a calibracao
  writeFile(SPIFFS, "/GyroX_CalibFactor.txt", String(calibGyroX).c_str());
  writeFile(SPIFFS, "/GyroY_CalibFactor.txt", String(calibGyroY).c_str());
  writeFile(SPIFFS, "/GyroZ_CalibFactor.txt", String(calibGyroZ).c_str());
  writeFile(SPIFFS, "/AccX_CalibFactor.txt", String(calibAccX).c_str());
  writeFile(SPIFFS, "/AccY_CalibFactor.txt", String(calibAccY).c_str());
  writeFile(SPIFFS, "/AccZ_CalibFactor.txt", String(calibAccZ).c_str());

  //Zera os angulos
  Gyroscope_x = 0;
  Gyroscope_y = 0;
  Gyroscope_z = 0;

  GyroX_CalibFactor = calibGyroX;
  GyroY_CalibFactor = calibGyroY;
  GyroZ_CalibFactor = calibGyroZ;

  AccX_CalibFactor = calibAccX;
  AccY_CalibFactor = calibAccY;
  AccZ_CalibFactor = calibAccZ;

  Mode.GyroCalibration = false;
  Serial.println("Finalizado a calibracao do MPU6050");
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

void ReadParameters(){
  Serial.println("Reading Parameters");
  String data = readFile(SPIFFS, "/Parameters.json");

  if(data == ""){
    Serial.println("Parameters not found!");

    Parameters["motorGainM1"] = 1.0;
    Parameters["motorGainM2"] = 1.0;
    Parameters["motorGainM3"] = 1.0;
    Parameters["motorGainM4"] = 1.0;
    Parameters["trotleLimit"] = 1.0;

    Parameters["GyroX_CalibFactor"] = 1.0;
    Parameters["GyroY_CalibFactor"] = 1.0;
    Parameters["GyroZ_CalibFactor"] = 1.0;
    Parameters["AccX_CalibFactor"] = 1.0;
    Parameters["AccY_CalibFactor"] = 1.0;
    Parameters["AccZ_CalibFactor"] = 1.0;

    Parameters["pGain"] = 1.0;
    Parameters["iGain"] = 1.0;
    Parameters["dGain"] = 1.0;

    Parameters["pidTimer"] = 1.0;

    char buffer[2000];
    serializeJson(Parameters, buffer);
    writeFile(SPIFFS, "/Parameters.json", buffer);
  }else{
    DeserializationError error = deserializeJson(Parameters, data);

    if (error){
      Serial.println("Erro ao ler os Parametros do Drone");
    }else{
      Serial.println("Parameters read with success");
    }
  }
}

float range(float value, float min, float max){
  if (value < min){
    value = min;
  }

  if (value > max){
    value = max;
  }

  return value;
}