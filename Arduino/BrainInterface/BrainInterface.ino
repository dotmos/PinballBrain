//MCP23017 stuff
#include <Wire.h>
#include "Adafruit_MCP23017.h"

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

#define SWITCH_MAX_COUNT 16
#define LED_MAX_COUNT 1024
#define SOLENOID_MAX_COUNT 32
#define SOLENOID_MAX_CONCURRENT 2 //max amount of solenoids being active at the same time

const byte LED_ACTIVATE = 10; // + led (2 bytes)
const byte LED_DEACTIVATE = 11; // + led (2 bytes)
const byte LED_BLINK = 12; // + led (2 bytes) + interval (2 bytes)
const byte LED_BLINK_AMOUNT = 13; // + led (2 bytes) + interval (2 bytes) + times (byte)

const byte SOLENOID_ACTIVATE = 1; // + solenoid (byte)
const byte SOLENOID_DEACTIVATE = 2; // + solenoid (byte)
const byte SOLENOID_TRIGGER = 3; // + solenoid (byte) + time (2 bytes)

const byte SWITCH_ACTIVE = 30; // 2 bytes switch id
const byte SWITCH_INACTIVE = 31; // 2 bytes switch id

#include "Switches.h"
#include "PlayfieldParts.h"
#include "SerialParse.h"


unsigned long time;

void setup() {
  // put your setup code here, to run once:
    pinMode(13, OUTPUT);  // use the p13 LED as debugging

    //while (!Serial);

    Switch_Setup();
    Serial_Setup();
    PlayfieldParts_Setup();
}




void loop() {
  int deltaTime = millis() - time;
  time = millis();

  Switch_Update(deltaTime);

  //Receive data from unity
  Serial_Update(deltaTime);

  PlayfieldParts_Update(deltaTime);
}
