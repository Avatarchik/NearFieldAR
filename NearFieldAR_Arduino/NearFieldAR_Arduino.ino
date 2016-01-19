#include <Servo.h>

Servo servo;
int servoPosition = 90;
int rotateDegree;
int incomingByte = 0;   // for incoming serial data
unsigned long time1;
unsigned long time2;
void setup()
{
  Serial.begin(9600); // // opens serial port, sets data rate to 9600 bps
  Serial.setTimeout(10);  
  servo.attach(9); // attaches the servo on pin 5 to the servo object
  servo.write(servoPosition); // set the servo at the mid position
}

void loop()
{
  
  if (Serial.available() > 0) {
    
    // read the incoming byte:
    //incomingByte = Serial.read();
 //   time1 = millis();  
    rotateDegree = Serial.parseInt();   
 //   time2 = millis();
 //   Serial.println(time2 - time1);
    servoPosition += rotateDegree;
    if (servoPosition > 180)
    {
        servoPosition = 180;
    }
    if (servoPosition < 0)
    {
        servoPosition = 0;
    }
    servo.write(servoPosition);
  }
 
}
