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

// TFT display and SD card will share the hardware SPI interface.
// Hardware SPI pins are specific to the Arduino board type and
// cannot be remapped to alternate pins.  For Arduino Uno,
// Duemilanove, etc., pin 11 = MOSI, pin 12 = MISO, pin 13 = SCK.
#define TFT_CS  10  // Chip select line for TFT display
#define TFT_RST  8  // Reset line for TFT (or see below...)
#define TFT_DC   9  // Data/command line for TFT

#define SD_CS    4  // Chip select line for SD card

// Test with reduced SPI speed for breadboards.
// Change spiSpeed to SPI_FULL_SPEED for better performance
// Use SPI_QUARTER_SPEED for even slower SPI bus speed
const uint8_t spiSpeed = SPI_HALF_SPEED;

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

//Message headers
const byte LED_ACTIVATE = 10; // + led (2 bytes) + color (3 bytes)
const byte LED_DEACTIVATE = 11; // + led (2 bytes)
const byte LED_BLINK = 12; // + led (2 bytes) + color (3 bytes) + interval (2 bytes)
const byte LED_BLINK_AMOUNT = 13; // + led (2 bytes) + color (3 bytes) + interval (2 bytes) + times (byte)

//const byte SOLENOID_ACTIVATE = 1; // + solenoid (byte)
//const byte SOLENOID_DEACTIVATE = 2; // + solenoid (byte)
//const byte SOLENOID_TRIGGER = 3; // + solenoid (byte) + time (2 bytes)

const byte SWITCH_ACTIVE = 30; // 2 bytes switch id
const byte SWITCH_INACTIVE = 31; // 2 bytes switch id

const byte DISPLAY_SET_IMAGE = 20; //  + display (byte) + image (2 byte)

//Logic
#include "SDCard.h"
#include "Switches.h"
#include "Display.h"
#include "PlayfieldParts.h"
#include "SerialParse.h"

//Core
unsigned long time;

void setup() {

  // put your setup code here, to run once:
    pinMode(13, OUTPUT);  // use the p13 LED as debugging

    //while (!Serial);

    SDCard_Setup();
    Display_Setup();
    Switch_Setup();
    Serial_Setup();
    PlayfieldParts_Setup();

    Display_SetImage(0, 1);
}


void loop() {
  int deltaTime = millis() - time;
  time = millis();

  Switch_Update(deltaTime);

  //Receive data from unity
  Serial_Update(deltaTime);

  PlayfieldParts_Update(deltaTime);

  Display_Update(deltaTime);
}
