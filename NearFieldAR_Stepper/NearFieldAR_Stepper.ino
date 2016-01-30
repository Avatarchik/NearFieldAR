/*     Simple Stepper Motor Control Exaple Code
 *      
 *  by Dejan Nedelkovski, www.HowToMechatronics.com
 *  
 */

// defines pins numbers
int stepperPosition = 90;
int resolution = 8;
int diff = 0;
int rotateDegree;
 int delayTime;
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
    diff = 0;
    diff = Serial.parseInt();  
    if(diff > 70)
      delayTime = 650;
     else
      delayTime = 1300;
    rotateDegree = diff  * 150.0f / 150.0f * 72.0f / 600.0f; 
    Rotate(diff);
  //  Serial.println(rotateDegree);
  }

}
void Rotate(float diff) {
  int steps = (int)(diff * 150.0f * 200.0f * resolution * 72.0f / 150.0f / 360.0f / 600.0f);
  if(rotateDegree > 0 && stepperPosition + rotateDegree <= 180) {
      digitalWrite(dirPin,LOW); // Enables the motor to move in a particular direction
      stepperPosition += rotateDegree;
  }
   else if(rotateDegree < 0 && stepperPosition + rotateDegree >= 0){
      digitalWrite(dirPin, HIGH); // Enables the motor to move in a particular direction
      steps = steps * -1;
      stepperPosition += rotateDegree;
   }
   else
    steps = 0;
  // Makes 200 pulses for making one full cycle rotation
   
   for(int i = 0; i < steps; i++) {
      digitalWrite(stepPin,HIGH); 
      delayMicroseconds(delayTime); 
      digitalWrite(stepPin,LOW); 
      delayMicroseconds(delayTime); 
   }
}





