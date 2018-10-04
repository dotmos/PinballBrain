//LEDs - WS2812
#include <WS2812Serial.h>
const int ledSerialPin = 1;


//MCP23017 stuff
#include <Wire.h>
#include "Adafruit_MCP23017.h"

//Display stuff
#include <Adafruit_GFX.h>    // Core graphics library
#include <Teensy_ST7735.h> // Hardware-specific library
#include <SPI.h>

//SD Card stuff
#include <SdFat.h>

//generic helpers
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

//definitions
#define SWITCH_MAX_COUNT 16 //Maximum amount of switches. Change to your needs.
#define LED_MAX_COUNT 128 //Maximum amount of LEDs. Change to your needs.
#define SOLENOID_MAX_COUNT 32 //Maximum amount of solenoid. Change to your needs.
#define SOLENOID_MAX_CONCURRENT 4 //max amount of solenoids being active at the same time.  Change to your needs. (Make sure your power supply can handle amount of solenoids).
#define DISPLAY_MAX_COUNT 1 //Maximum amount of displays. Change to your needs.
int display_CS[DISPLAY_MAX_COUNT] = {10}; //Chip select pins for displays

//Message headers
const byte LED_ACTIVATE = 10; // + led (2 bytes) + color (3 bytes)
const byte LED_DEACTIVATE = 11; // + led (2 bytes)
const byte LED_BLINK = 12; // + led (2 bytes) + color (3 bytes) + interval (2 bytes)
const byte LED_BLINK_AMOUNT = 13; // + led (2 bytes) + color (3 bytes) + interval (2 bytes) + times (byte)

const byte SOLENOID_ACTIVATE = 1; // + solenoid (byte)
const byte SOLENOID_DEACTIVATE = 2; // + solenoid (byte)
const byte SOLENOID_TRIGGER = 3; // + solenoid (byte) + time (2 bytes)

const byte SWITCH_ACTIVE = 30; // + switch id (2 bytes)
const byte SWITCH_INACTIVE = 31; // + switch id (2 bytes)

const byte DISPLAY_SET_IMAGE = 20; //  + display (byte) + image (2 byte)
const byte DISPLAY_CLEAR_IMAGE = 21; // + display (byte)

//Logic
#include "SDCard.h"
#include "Switches.h"
#include "LEDs.h"
#include "Solenoids.h"
#include "Displays.h"
#include "SerialParse.h"

//Core
unsigned long time;

void setup() {

  // put your setup code here, to run once:
    pinMode(13, OUTPUT);  // use the p13 LED as debugging

    //while (!Serial);

    SDCard_Setup();
    Switch_Setup();
    Serial_Setup();
    LEDs_Setup();
    Solenoids_Setup();
    Display_Setup();
}


void loop() {
  int deltaTime = millis() - time;
  time = millis();

  Switch_Update(deltaTime);

  //Receive data from unity
  Serial_Update(deltaTime);

  Solenoids_Update(deltaTime);
  LEDs_Update(deltaTime);
  Display_Update(deltaTime);
}
