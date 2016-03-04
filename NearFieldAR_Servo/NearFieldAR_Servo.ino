/* Sweep
 by BARRAGAN <http://barraganstudio.com>
 This example code is in the public domain.

 modified 8 Nov 2013
 by Scott Fitzgerald
 http://www.arduino.cc/en/Tutorial/Sweep
*/

#include <Servo.h>

Servo myservoR;  // create servo object to control a servo
Servo myservoL;
// twelve servo objects can be created on most boards

int posL = 90;
int posR = 90; // variable to store the servo position
int diff = 0;
int left = 0;
int right = 0;
int rotateDegree;
int delayTime = 5;
const float FRAME_WIDTH = 1024.0f;
const float FRAME_HEIGHT = 768.0f;
void setup() {
  //myservo995.attach(9);  // attaches the servo on pin 9 to the servo object
  Serial.begin(9600); // // opens serial port, sets data rate to 9600 bps
  Serial.setTimeout(50);  
  myservoR.attach(9);
  myservoR.write(90);  
  myservoL.attach(10);
  myservoL.write(90);

}



void move_motor(int left_right, int value)
{

        rotateDegree = value * 77.0f / FRAME_HEIGHT; 
    delayTime = 230 / abs(rotateDegree);
    Serial.println(delayTime);
    delayTime = constrain(delayTime, 4, 100);

    if(left_right == 0) // LEFT
    {
        if(rotateDegree > 0){
          for (int i = posL; i <= (posL + rotateDegree); i += 1) { // goes from 0 degrees to 180 degrees
              myservoL.write(i);// tell servo to go to position in variable 'pos'
            delay(delayTime/2);                       // waits 15ms for the servo to reach the position
          }
          posL += rotateDegree;
          if(posL > 180)  posL = 180;
        }else{
          for (int i = posL; i >= (posL + rotateDegree); i -= 1) { // goes from 0 degrees to 180 degrees
              myservoL.write(i);\ // tell servo to go to position in variable 'pos'
            delay(delayTime/2);                       // waits 15ms for the servo to reach the position
          }
          posL += rotateDegree;
          if(posL < 0)  posL = 0;   
        }
    }
    else // RIGHT
    {
        if(rotateDegree > 0){
          for (int i = posR; i <= (posR + rotateDegree); i += 1) { // goes from 0 degrees to 180 degrees
              myservoR.write(i);// tell servo to go to position in variable 'pos'
           delay(delayTime/2);                       // waits 15ms for the servo to reach the position
          }
          posR += rotateDegree;
          if(posR > 180)  posR = 180;
        }else{
          for (int i = posR; i >= (posR + rotateDegree); i -= 1) { // goes from 0 degrees to 180 degrees
              myservoR.write(i);\ // tell servo to go to position in variable 'pos'
           delay(delayTime/2);                       // waits 15ms for the servo to reach the position
          }
          posR += rotateDegree;
          if(posR < 0)  posR = 0;   
        }
    }
}


void loop() {
  if (Serial.available() > 0) {
    
    diff = 0;
    left = 0;
    right = 0;  

    /*String left  = Serial.readStringUntil(';');
    Serial.read();
    String right  = Serial.readStringUntil('\0');

    diff = right.toInt();
 */
    String entire_string = Serial.readStringUntil('\n');
    int comma_indx = entire_string.indexOf(',');
    left = entire_string.substring(0, comma_indx).toInt();
    right = entire_string.substring(comma_indx+1).toInt();

    diff = right;
    Serial.print("Left: ");
    Serial.println(left);
    Serial.print("Right: ");
    Serial.println(right);


    move_motor(0, left);
    move_motor(1, right);
    
    //diff = Serial.parseInt();  
delay(20);
  }
}
