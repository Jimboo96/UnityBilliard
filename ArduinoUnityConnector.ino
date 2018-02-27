#include <SoftwareSerial.h>
#include <SerialCommand.h>

SerialCommand sCmd;

 const int VCCPin = A0;
 const int xPin   = A1;
 const int yPin   = A2;
 const int zPin   = A3;
 const int GNDPin = A4;

 int x = 0;
 int y = 0;
 int z = 0;
 float previousAx = 0;
 float ax = 0.0;
 double ay = 0.0;
 double az = 0.0;

void setup() {
  pinMode(A0, OUTPUT);
  pinMode(A4, OUTPUT);
  digitalWrite(14, HIGH);
  digitalWrite(18, LOW);
  Serial.begin(9600);
  while (!Serial);

  sCmd.addCommand("PING", pingHandler);
  sCmd.addCommand("ECHO", echoHandler);
  sCmd.setDefaultHandler(errorHandler);
}

void loop() {
  if (Serial.available() > 0)
    sCmd.readSerial();
    x = analogRead(xPin);
    //y = analogRead(yPin);
    //z = analogRead(zPin);
    //ay = 0.145333 * y - 48.9047;
    //az = 0.147519 * z - 49.345;
    ax = 0.145333 * x - 47.554;
    Serial.println(ax); 
    delay(100);
}

void pingHandler (const char *command) {
 Serial.println("PONG");
}

void echoHandler ()
{
  char *arg;
  arg = sCmd.next();
  if (arg != NULL)
    Serial.println(arg);
  else
    Serial.println("nothing to echo");
}

void errorHandler (const char *command)
{
  // TODO
}
