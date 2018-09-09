//MCP23017 stuff
#include <Wire.h>
#include "Adafruit_MCP23017.h"

Adafruit_MCP23017 mcp;

const byte LED_ACTIVATE = 10; // + led (2 bytes)
const byte LED_DEACTIVATE = 11; // + led (2 bytes)

boolean currentState = false;
const byte SWITCH_ACTIVE = 30; // 2 bytes switch id
const byte SWITCH_INACTIVE = 31; // 2 bytes switch id

void setup() {
  // put your setup code here, to run once:

   //MCP23017
    mcp.begin(0);      // use mcp with address 0
    mcp.pinMode(0, INPUT);
    mcp.pullUp(0, HIGH);  // turn on a 100K pullup internally

    pinMode(13, OUTPUT);  // use the p13 LED as debugging

    Serial.begin(9600);

    //while (!Serial);
}

int16_t read16(byte data[2]) {
  int16_t result;
  ((int8_t *)&result)[0] = data[0]; // LSB
  ((int8_t *)&result)[1] = data[1]; // MSB
  return result;
}

int32_t read32(byte data[4]) {
  int32_t result;
  ((int8_t *)&result)[0] = data[0]; // LSB
  ((int8_t *)&result)[1] = data[1];
  ((int8_t *)&result)[2] = data[2];
  ((int8_t *)&result)[3] = data[3]; // MSB
  return result;
}

void ReadUnityData(){
  byte incomingByte = Serial.read();
  
  switch(incomingByte){
    case LED_ACTIVATE:
      byte ledByte1[2];
      ledByte1[0] = Serial.read();
      ledByte1[1] = Serial.read();
      ActivateLED(read16(ledByte1));
    break;
    
    case LED_DEACTIVATE:
    byte ledByte2[2];
      ledByte2[0] = Serial.read();
      ledByte2[1] = Serial.read();
      DeactivateLED(read16(ledByte2));
    break;
  }
}

void ActivateLED(short led){
  digitalWrite(led, HIGH);
}

void DeactivateLED(short led){
  digitalWrite(led, LOW);
}

void loop() {
  // put your main code here, to run repeatedly:

  //Receive data from unity
  if (Serial.available() > 0) {
    ReadUnityData();
  }
  
  
  //Check switch and send state to unity
  if(mcp.digitalRead(0) == LOW && currentState == true)
  {
    currentState = false;
    byte bytes[3];
    bytes[0] = SWITCH_INACTIVE;
    //Switch number
    bytes[1] = 1;
    bytes[2] = 0;
    Serial.write(bytes, 3);
    //Wait for bytes to be written before continuing
    Serial.flush();
  }
  else if(mcp.digitalRead(0) == HIGH && currentState == false){
    currentState = true;
    byte bytes[3];
    bytes[0] = SWITCH_ACTIVE;
    //Switch number
    bytes[1] = 1;
    bytes[2] = 0;
    Serial.write(bytes, 3);
    //Wait for bytes to be written before continuing
    Serial.flush();
  }
}
