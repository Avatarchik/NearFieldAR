/* Sweep
 by BARRAGAN <http://barraganstudio.com>
 This example code is in the public domain.

 modified 8 Nov 2013
 by Scott Fitzgerald
 http://www.arduino.cc/en/Tutorial/Sweep
*/

#include <Servo.h>


Servo myservo;  // create servo object to control a servo

// twelve servo objects can be created on most boards

int pos = 90;
int diff = 0;
int left = 0;
int right = 0;
int rotateDegree;
int delayTime = 25;
const float FRAME_WIDTH = 1024.0f;
const float FRAME_HEIGHT = 768.0f;
void setup() {
  /*
  //myservo995.attach(9);  // attaches the servo on pin 9 to the servo object
  Serial.begin(9600); // // opens serial port, sets data rate to 9600 bps
  Serial.setTimeout(50);  
  myservo.attach(9);
  myservo.write(90);  
  //myservo.attach(10);
  //myservo.write(90);
  */


}



void move_motor(int value)
{

    rotateDegree = value * 77.0f / FRAME_HEIGHT; 
    delayTime = 300 / abs(rotateDegree);
//    Serial.println(delayTime);
    delayTime = constrain(delayTime, 4, 100);


      if(rotateDegree > 0){
        for (int i = pos; i <= (pos + rotateDegree); i += 1) { // goes from 0 degrees to 180 degrees
            myservo.write(i);// tell servo to go to position in variable 'pos'
            
            delay(delayTime/2);                       // waits 15ms for the servo to reach the position
        }
        pos += rotateDegree;
        if(pos > 180)  pos = 180;
      }else{
        for (int i = pos; i >= (pos + rotateDegree); i -= 1) { // goes from 0 degrees to 180 degrees
            myservo.write(i);\ // tell servo to go to position in variable 'pos'
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
