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

int pos = 90;
int diff = 0;
int left = 0;
int right = 0;
int rotateDegree;
int delayTime = 0;
const float FRAME_WIDTH = 1024.0f;
const float FRAME_HEIGHT = 768.0f;
int ID = 1;
void setup() {
  //myservo995.attach(9);  // attaches the servo on pin 9 to the servo object
  Serial.begin(9600); // // opens serial port, sets data rate to 9600 bps
  Serial.setTimeout(50);  
//  myservo.attach(9);
//  myservo.write(90);  
  //myservo.attach(10);
  //myservo.write(90);

  Dynamixel.begin(1000000,2);  // Inicialize the servo at 1Mbps and Pin Control 2
  delay(1000);
  Dynamixel.ledStatus(ID,ON);

  Dynamixel.move(ID, degrees_to_value(0));
}


int degrees_to_value(int value)
{
  return map(value, -300, 300, 0, 1023);
}

void move_degrees(int degree)
{
  Dynamixel.move(ID, degrees_to_value(degree));
}


void move_motor(int value)
{

    rotateDegree = value * 77.0f / FRAME_HEIGHT; 
    delayTime = 150 / abs(rotateDegree);
//    Serial.println(delayTime);
    delayTime = constrain(delayTime, 4, 100);


      if(rotateDegree > 0){
        for (int i = pos; i <= (pos + rotateDegree); i += 1) { // goes from 0 degrees to 180 degrees
            move_degrees(i);
            delay(delayTime/2);                       // waits 15ms for the servo to reach the position
        }
        pos += rotateDegree;
        if(pos > 180)  pos = 180;
      }else{
        for (int i = pos; i >= (pos + rotateDegree); i -= 1) { // goes from 0 degrees to 180 degrees
            move_degrees(i);
            delay(delayTime/2);                       // waits 15ms for the servo to reach the position
        }
        pos += rotateDegree;
        if(pos < 0)  pos = 0;   
      }


}


void loop() {
  if (Serial.available() > 0) {
    int value = 0;

    /*String left  = Serial.readStringUntil(';');
    Serial.read();
    String right  = Serial.readStringUntil('\0');

    diff = right.toInt();
 */
    String entire_string = Serial.readStringUntil('\n');
    value = entire_string.toInt();
    Serial.println(value);
    move_motor(value);
    //diff = Serial.parseInt();  
    //delay(20);
  }
}
