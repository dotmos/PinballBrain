//TFT stuff
#include <Adafruit_GFX.h>    // Core graphics library
#include <Teensy_ST7735.h> // Hardware-specific library
#include <SPI.h>
#include <SdFat.h>

//MCP23017 stuff
#include <Wire.h>
#include "Adafruit_MCP23017.h"

Adafruit_MCP23017 mcp;

// TFT display and SD card will share the hardware SPI interface.
// Hardware SPI pins are specific to the Arduino board type and
// cannot be remapped to alternate pins.  For Arduino Uno,
// Duemilanove, etc., pin 11 = MOSI, pin 12 = MISO, pin 13 = SCK.
#define TFT_CS  10  // Chip select line for TFT display
#define TFT_RST  8  // Reset line for TFT (or see below...)
#define TFT_DC   9  // Data/command line for TFT

#define SD_CS    4  // Chip select line for SD card

//Use this reset pin for the shield!
//#define TFT_RST  0  // you can also connect this to the Arduino reset!
Teensy_ST7735 tft = Teensy_ST7735(TFT_CS, TFT_DC, TFT_RST);


// Test with reduced SPI speed for breadboards.
// Change spiSpeed to SPI_FULL_SPEED for better performance
// Use SPI_QUARTER_SPEED for even slower SPI bus speed
const uint8_t spiSpeed = SPI_HALF_SPEED;

// File system object.
SdFat sd;

void setup(void) {
  Serial.begin(9600);
  
  
  //MCP23017
  mcp.begin(0);      // use mcp with address 0
  mcp.pinMode(0, INPUT);
  mcp.pullUp(0, HIGH);  // turn on a 100K pullup internally
  pinMode(13, OUTPUT);  // use the p13 LED as debugging for mcp23017
  
  
  
  

  // Use this initializer if you're using a 1.8" TFT
  tft.initR(INITR_BLACKTAB);

  // Use this initializer (uncomment) if you're using a 1.44" TFT
  //tft.initR(INITR_144GREENTAB);

  Serial.print("Initializing SD card...");
  if (!sd.begin(SD_CS, spiSpeed)) {
    Serial.println("failed!");
    return;
  }
  Serial.println("OK!");
  
  bmpDraw("ml.lcd", 0, 0);
  delay(100);
  //bmpDraw("/wcintro/WC_Intro30.bin", 0,0);
}

void randomMercTalk()
{
  int r = random(10);
  if(r <= 3)
    bmpDraw("ml57_791.lcd", 57, 79);
  else if(r <= 6)
    bmpDraw("ml57_792.lcd", 57, 79);
  else
    bmpDraw("ml57_793.lcd", 57, 79);
}

//needed for wc2 intro movie playback
int clipIndex = 0;
char filename[100];
String str;
void loop() {
  
  //Play wc2 intro if mcp23017 A0 is LOW
  if(mcp.digitalRead(0) == LOW)
  {
    //play wc2 intro
    str = "/wcintro/WC_Intro" + String(clipIndex) + ".bin";
    str.toCharArray(filename, 100);
    bmpDraw(filename, 0,0);
    clipIndex++;
    if(clipIndex > 1091)
      clipIndex = 0;
  } 
  else { //Otherwise show talking merc guild girl
    bmpDraw("ml.lcd", 0, 0);
    randomMercTalk();
  }
 
  
  //randomMercTalk();
  //delay(100);
  
  //bmpDraw("/wcintro/WC_Intro43.bin", 0,0);
  /*
  bmpDraw("new.bin", 0, 0);
  //delay(100);
  bmpDraw("trakath.lcd", 0, 0);
  //delay(100);
  bmpDraw("mining.lcd", 0, 0);
  //delay(100);
  */
  
  /*
  randomAnimation();
  delay(100);
  */
}

// This function opens a Windows Bitmap (BMP) file and
// displays it at the given coordinates.  It's sped up
// by reading many pixels worth of data at a time
// (rather than pixel by pixel).  Increasing the buffer
// size takes more of the Arduino's precious RAM but
// makes loading a little faster.  20 pixels seems a
// good balance.

#define BUFFPIXEL 20

void bmpDraw(char *filename, uint8_t x, uint8_t y) {
  
  File     bmpFile;
  int      bmpWidth, bmpHeight;   // W+H in pixels
  
  if((x >= tft.width()) || (y >= tft.height())) return;

  //Serial.println();
  //Serial.print("Loading image '");
  //Serial.print(filename);
  //Serial.println('\'');

  // Open requested file on SD card
  if ((bmpFile = sd.open(filename)) == NULL) {
    Serial.print("File not found");
    return;
  }
  
  //Get size (first two bytes)
  bmpWidth = bmpFile.read();
  bmpHeight = bmpFile.read();
  //get offset (next two bytes)
  //uint8_t xOffset = bmpFile.read();
  //uint8_t yOffset = bmpFile.read();
  
  //Serial.println(bmpWidth);
  //Serial.println(bmpHeight);
  
  tft.setRotation(1);
  tft.setAddrWindow(x, y, x+bmpWidth-1, y+bmpHeight-1);
  
  

  
  //Read complete file content at once and store in framebuffer (warning, this will consume up to 41kB of ram!). Then push framebuffer to display. This is very fast, but uses a lot of ram.
  /*
  //"Framebuffer"
  uint8_t colorBuffer[2*bmpWidth*bmpHeight];
  bmpFile.read(colorBuffer, sizeof(colorBuffer));
  
  uint16_t result;
  for(int i=0; i<sizeof(colorBuffer); i+=2)
  {
    ((uint8_t *)&result)[0] = colorBuffer[i]; // LSB
    ((uint8_t *)&result)[1] = colorBuffer[i+1]; // MSB
    
    tft.pushColor(result);
  }
  */
  
  /*
  //Stream image, pixel by pixel, to display. This is very slow, but uses almost no ram.
  uint16_t result;
  for(int i=0; i<bmpWidth*bmpHeight; ++i)
  {
    ((uint8_t *)&result)[0] = bmpFile.read(); // LSB
    ((uint8_t *)&result)[1] = bmpFile.read(); // MSB
    
    tft.pushColor(result);
  }
  */
  
  
  
  //Stream image, line by line, to display. This has medium speed and uses a small amount of ram.
  uint16_t result;
  uint8_t lineBuffer[bmpWidth*2];
  for(int i=0; i<bmpHeight; ++i){
    //Read line
    bmpFile.read(lineBuffer, sizeof(lineBuffer));  
    //Push to display
    for(int w=0; w<bmpWidth*2; w+=2){
      ((uint8_t *)&result)[0] = lineBuffer[w]; // LSB
      ((uint8_t *)&result)[1] = lineBuffer[w+1]; // MSB
      tft.pushColor(result);
    }
  }
  
    
  //Close file
  bmpFile.close();
}

// These read 16- and 32-bit types from the SD card file.
// BMP data is stored little-endian, Arduino is little-endian too.
// May need to reverse subscript order if porting elsewhere.

uint16_t read16(File f) {
  uint16_t result;
  ((uint8_t *)&result)[0] = f.read(); // LSB
  ((uint8_t *)&result)[1] = f.read(); // MSB
  return result;
}

uint32_t read32(File f) {
  uint32_t result;
  ((uint8_t *)&result)[0] = f.read(); // LSB
  ((uint8_t *)&result)[1] = f.read();
  ((uint8_t *)&result)[2] = f.read();
  ((uint8_t *)&result)[3] = f.read(); // MSB
  return result;
}
