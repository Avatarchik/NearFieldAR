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
    stepperPosition += rotateDegree;
  }
}
void Rotate(int degree) {
  if(degree > 0) {
    digitalWrite(dirPin,LOW); // Enables the motor to move in a particular direction
  // Makes 200 pulses for making one full cycle rotation
    int steps = (int)((float)degree / 360.0f * 200.0f);
   // Serial.println(steps);
    for(int i = 0; i < steps; i++) {
      digitalWrite(stepPin,HIGH); 
      delay(1); 
      digitalWrite(stepPin,LOW); 
      delay(1); 
    }
  }
  else if(degree < 0){
      digitalWrite(dirPin, HIGH); // Enables the motor to move in a particular direction
  // Makes 200 pulses for making one full cycle rotation
      int steps = (int)((float)-degree / 360.0f * 200.0f);
      for(int i = 0; i < steps; i++) {
        digitalWrite(stepPin,HIGH); 
        delay(1);
        digitalWrite(stepPin,LOW); 
        delay(1);
      }
  }
  
}

