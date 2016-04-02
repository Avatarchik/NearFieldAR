/* Sweep
 by BARRAGAN <http://barraganstudio.com>
 This example code is in the public domain.

 modified 8 Nov 2013
 by Scott Fitzgerald
 http://www.arduino.cc/en/Tutorial/Sweep10
*/

#include <Servo.h>
#include <DynamixelSerial1.h>

//Servo myservo;  // create servo object to control a servo

// twelve servo objects can be created on most boards

int pos = 60;
int diff = 0;
int rotateDegree;
int delayTime;
const float FRAME_WIDTH = 1024.0f;
const float FRAME_HEIGHT = 768.0f;
int ID = 1;
void setup() {
  Serial.begin(9600); // // opens serial port, sets data rate to 9600 bps
  Serial.setTimeout(10);  
  Dynamixel.begin(1000000,2);  // Inicialize the servo at 1Mbps and Pin Control 2
  delay(1000);
  Dynamixel.ledStatus(ID,ON);
  Dynamixel.move(ID, degrees_to_value(pos));
}


int degrees_to_value(int value)
{
  return map(value, 0, 300, 0, 1023);
}

void move_degrees(int degree, int _speed)
{
  //Dynamixel.move(ID, degrees_to_value(0));
  Dynamixel.moveSpeed(ID, degrees_to_value(degree), _speed);
}


void move_motor(int value)
{
    rotateDegree = value * 77.0f / FRAME_HEIGHT; 
  //  delayTime = 200 / abs(rotateDegree);
   // delayTime = constrain(delayTime, 4, 150);
    
    int _speed = map(abs(rotateDegree), 0, 39, 0, 1023);
    pos += rotateDegree;
    move_degrees(pos, _speed);
    if(pos > 300)  pos = 300;
    else if(pos < 0) pos = 0;
  //  int rpm = map(_speed, 0, 1023, 0, 111);
    //float delayTime = 60.0f / (rpm * 360.0f) * abs(rotateDegree) * 1000;
    /// 111rpm   60 / (rpm * 360)
   // Serial.println(rpm);
  //  delay((int)(delayTime) + 5);
}


void loop() {
  if (Serial.available() > 0) {
    int value = 0;
    String entire_string = Serial.readStringUntil('\n');
    value = entire_string.toInt();
    move_motor(value);
   
  }
}
