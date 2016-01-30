/*     Simple Stepper Motor Control Exaple Code
 *      
 *  by Dejan Nedelkovski, www.HowToMechatronics.com
 *  
 */

// defines pins numbers
int stepperPosition = 90;
int rotateDegree;
const int stepPin = 5; 
const int dirPin = 4; 
 
void setup() {
  // Sets the two pins as Outputs
  Serial.begin(9600); // // opens serial port, sets data rate to 9600 bps
  Serial.setTimeout(10);  
  pinMode(stepPin,OUTPUT); 
  pinMode(dirPin,OUTPUT);
}
void loop() {
  if (Serial.available() > 0) {
    rotateDegree = 0;
    rotateDegree = Serial.parseInt();   
    Rotate(rotateDegree);
   
  }
 // Serial.println(stepperPosition);
}
void Rotate(int degree) {
  if(degree > 0 /*&& stepperPosition + degree <= 180*/) {
    digitalWrite(dirPin,LOW); // Enables the motor to move in a particular direction
  // Makes 200 pulses for making one full cycle rotation
    int steps = (int)((float)degree / 360.0f * 200.0f);
    int delayTime = 500;
 //  int delayTime = 80000 / steps;
 //   Serial.println(delayTime);
    for(int i = 0; i < steps; i++) {
      digitalWrite(stepPin,HIGH); 
      delayMicroseconds(delayTime); 
      digitalWrite(stepPin,LOW); 
      delayMicroseconds(delayTime); 
    }
 /*   stepperPosition += degree;
    if (stepperPosition > 180)
    {
        stepperPosition = 180;
    }*/
    //delay(20); 
  }
  else if(degree < 0 /*&& stepperPosition + degree >= 0*/){
      digitalWrite(dirPin, HIGH); // Enables the motor to move in a particular direction
  // Makes 200 pulses for making one full cycle rotation
      int steps = (int)((float)-degree / 360.0f * 200.0f);
      int delayTime = 500;
 //     int delayTime = 80000 / steps;
//      Serial.println(delayTime);
      for(int i = 0; i < steps; i++) {
        digitalWrite(stepPin,HIGH); 
        delayMicroseconds(delayTime);
        digitalWrite(stepPin,LOW); 
        delayMicroseconds(delayTime);
      }
   /*   stepperPosition += degree;
      if (stepperPosition < 0)
      {
        stepperPosition = 0;
      }*/
     // delay(20); 
  }
  
}




