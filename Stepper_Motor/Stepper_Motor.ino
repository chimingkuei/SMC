const int dirPin = 2; 
const int stepPin = 5; 
const int enPin = 8;
char pulse[9];
char delaytime[9];

void setup() {
  Serial.begin(115200);
  pinMode(stepPin,OUTPUT); 
  pinMode(dirPin,OUTPUT);
  pinMode(enPin,OUTPUT);
  digitalWrite(enPin,LOW);
}

void loop() {
  if (Serial.available() > 0) {
    char inByte = Serial.read();
    switch (inByte) {
      case '1':
        digitalWrite(dirPin,HIGH);
        while(true)
        {
          Serial.readBytes(pulse,9);
          if(atoi(pulse)>0)
          {
            break;
          }
        }
        while(true)
        {
          Serial.readBytes(delaytime,9);
          if(atoi(delaytime)>0)
          {
            break;
          }
        }
        Serial.println("Start.........");
        while(digitalRead(dirPin)==HIGH)
        {
          for(int x = 0; x < atoi(pulse); x++) {
            digitalWrite(stepPin,HIGH); 
            delayMicroseconds(atoi(delaytime)); 
            digitalWrite(stepPin,LOW); 
            delayMicroseconds(atoi(delaytime)); 
          }
          char sign = Serial.read();
          if (sign=='0'){
            digitalWrite(dirPin,LOW);
            memset(pulse, 0, 9);
            memset(delaytime, 0, 9);
            Serial.println("Stop.........");
          }
        }
        break;
    }
  }
}
