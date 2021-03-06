#ifndef USE_DISPLAY
#define USE_DISPLAY
#endif
const int DISPLAY_FRAMELIMIT_DEFAULT = 32; //16ms = 60fps, 32ms = 30fps, 64ms = 15fps, etc.

//#define DISPLAY_SERIAL_DEBUG

//Split the display update between CPU cycles. Introduces screen tearing, but won't block the CPU so long and makes room for other things besides display (animation) updates.
//#define DISPLAY_PARTIAL_UPDATE


// TFT display and SD card will share the hardware SPI interface.
// Hardware SPI pins are specific to the Arduino board type and
// cannot be remapped to alternate pins.  For Arduino Uno,
// Duemilanove, etc., pin 11 = MOSI, pin 12 = MISO, pin 13 = SCK.
//#define TFT_CS  10  // Chip select line for TFT display
#define TFT_RST  8  // Reset line for TFT (or see below...)
#define TFT_DC   9  // Data/command line for TFT

Teensy_ST7735 displays[DISPLAY_MAX_COUNT] = {
  Teensy_ST7735(display_CS[0], TFT_DC, TFT_RST)
};

//char helper
String display_fileEnding = ".565";
char display_animationFilename[100];
String display_str;

struct Display_Data{
  bool drawImage;
  char imageFilename[100];
  //The time in ms before drawing a new image in the display
  short frametime;
  //Counter for checking if frametime was reached
  short frametimeCounter;

  //Animation ID. If -1, no animation will be played
  short animationID;
  //Animation type. 0 = simple, 1 = complex
  byte animationType;
  //If an animation is playing: Position of the animation in animation file
  int animationFilePosition;
  //Whether or not the animation should be looped
  byte loopAnimation;
  //imageID of the current frame
  short frameImageID;

#ifdef DISPLAY_PARTIAL_UPDATE
  //Whether or not the display should continue to update data. Needed if image is drawn line by line, so it won't block the cpu too much.
  byte continuePartialUpdate;
#endif

};

Display_Data display_data[DISPLAY_MAX_COUNT];

//When using partial display update (WIP)
#ifdef DISPLAY_PARTIAL_UPDATE
const int displayLineUpdateDivider = 32;
int displayCurrentLineUpdate = 0;
int filePos = 0;
#endif

//------------------------------------------------------------

void Display_Setup(){
  //Create and initialize displays
  for(int i=0; i<DISPLAY_MAX_COUNT; ++i){
    //Will not work, as _display is destroyed after Display_Setup() is finished.
    //Teensy_ST7735 _display = Teensy_ST7735(TFT_CS, TFT_DC, TFT_RST);
    //displays[i] = &Teensy_ST7735(display_CS[i], TFT_DC, TFT_RST);
    //displays[i] = &_display;
    //(*displays[i]).initR(INITR_BLACKTAB);

    // Use this initializer if you're using a 1.8" TFT
    displays[i].initR(INITR_BLACKTAB);
    // Use this initializer (uncomment) if you're using a 1.44" TFT
    //displays[i].initR(INITR_144GREENTAB);
    
    display_data[i].drawImage = false;
    display_data[i].frametime = DISPLAY_FRAMELIMIT_DEFAULT;
    display_data[i].animationID = -1;
    display_data[i].animationFilePosition = 0;
    display_data[i].frameImageID = -1;
    
    #ifdef DISPLAY_PARTIAL_UPDATE
    display_data[i].continuePartialUpdate = 0;
    #endif
  }

  
  // Use this initializer if you're using a 1.8" TFT
  //display1.initR(INITR_BLACKTAB);

  // Use this initializer (uncomment) if you're using a 1.44" TFT
  //display1.initR(INITR_144GREENTAB);

  //_Display_DrawImage(0, "/display/ml" + display_fileEnding, 0, 0);
}

//Read a short from a file
short Display_GetShort(File *f){
  byte shortByte[2];
  shortByte[0] = (*f).read();
  shortByte[1] = (*f).read();

  return read16(shortByte);
}

//Draws an image from a file. Image needs to be converted with "CustomImageConverter" from github.org/dotmos/CustomImageConverter . (converts .bmp/.jpg/.png to Color565 format)
void _Display_DrawImage(byte displayID, char *filename, uint8_t x, uint8_t y) {
  
  File     bmpFile;
  int      bmpWidth, bmpHeight;   // W+H in pixels

  Teensy_ST7735 _display = displays[displayID];
  
  if((x >= _display.width()) || (y >= _display.height())) return;

  //Serial.println();
  //Serial.print("Loading image '");
  //Serial.print(filename);
  //Serial.println('\'');

  // Open requested file on SD card
  if ((bmpFile = sd.open(filename)) == NULL) { 
    //Serial.print("File not found");
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

  #ifndef DISPLAY_PARTIAL_UPDATE
  _display.setRotation(1);
  _display.setAddrWindow(x, y, x+bmpWidth-1, y+bmpHeight-1);
  #endif
  
  #ifndef DISPLAY_PARTIAL_UPDATE
  //Read complete file content at once and store in framebuffer (warning, this will consume up to 41kB of ram!). Then push framebuffer to display. This is very fast, but uses a lot of ram.
  //"Framebuffer"
  uint8_t colorBuffer[2*bmpWidth*bmpHeight];
  bmpFile.read(colorBuffer, sizeof(colorBuffer));
  //Close file, we no longer need it open
  bmpFile.close();

  
  uint16_t result;
  for(int i=0; i<sizeof(colorBuffer); i+=2)
  {
    ((uint8_t *)&result)[0] = colorBuffer[i]; // LSB
    ((uint8_t *)&result)[1] = colorBuffer[i+1]; // MSB
    
    _display.pushColor(result);
  }
  //-----------------------------------------------------------------------
  #endif
  
  
  /*
  //Stream image, pixel by pixel, to display. This is very slow, but uses almost no ram.
  uint16_t result;
  //for(int i=0; i<bmpWidth*bmpHeight; ++i)
  for(int i=0; i<bmpWidth*bmpHeight; ++i)
  {
    ((uint8_t *)&result)[0] = bmpFile.read(); // LSB
    ((uint8_t *)&result)[1] = bmpFile.read(); // MSB
    
    _display.pushColor(result);
  }
  //Close file
  bmpFile.close();
  //-----------------------------------------------------------------------
  */

  /*
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
      _display.pushColor(result);
    }
  }
  //Close file
  bmpFile.close();
  //-----------------------------------------------------------------------
  */


#ifdef DISPLAY_PARTIAL_UPDATE
  //Stream image, line by line, to display, but don't stream whole display so arduino has time for other things. This has high speed and uses a small amount of ram, but will introduce image tearing if display images change very fast.
  int verticalLines = bmpHeight/displayLineUpdateDivider;
  _display.setRotation(1);
  //_display.setAddrWindow(x, y, x+bmpWidth-1, y+bmpHeight-1);
  _display.setAddrWindow(x, verticalLines*displayCurrentLineUpdate, x+bmpWidth-1, (verticalLines*displayCurrentLineUpdate)+verticalLines-1);
  uint16_t result;
  uint8_t lineBuffer[bmpWidth*2];
  if(filePos > 0){
    bmpFile.seek(filePos); //move to correct data location
  }
  //bmpFile.seek(filePos + bmpFile.position()); //move to correct data location
  for(int i=displayCurrentLineUpdate*verticalLines; i<verticalLines*(displayCurrentLineUpdate+1); ++i){
    //Read line
    bmpFile.read(lineBuffer, sizeof(lineBuffer));  
    //Push to display
    for(int w=0; w<bmpWidth*2; w+=2){
      ((uint8_t *)&result)[0] = lineBuffer[w]; // LSB
      ((uint8_t *)&result)[1] = lineBuffer[w+1]; // MSB
      _display.pushColor(result);
    }
  }
  displayCurrentLineUpdate += 1;
  if(displayCurrentLineUpdate > displayLineUpdateDivider-1){
    displayCurrentLineUpdate = 0;
    filePos = 0;
    display_data[displayID].continuePartialUpdate = 0;
  } else {
    display_data[displayID].continuePartialUpdate = 1;
    filePos = bmpFile.position();
  }
  
  //Close file
  bmpFile.close();
  //-----------------------------------------------------------------------
#endif
  
  
  
}






//Stop the current animation
void Display_StopAnimation(byte display){
  display_data[display].animationID = -1;
  display_data[display].frameImageID = -1;
  display_data[display].animationFilePosition = 0;
  display_data[display].frametime = DISPLAY_FRAMELIMIT_DEFAULT;

#ifdef DISPLAY_PARTIAL_UPDATE
  display_data[display].continuePartialUpdate = 0;
#endif
}

//Draw an image on next display refresh
void Display_SetImage(byte display, short image){
  Display_StopAnimation(display);
  
  display_str = "/display/" + String(image) + display_fileEnding;
  display_str.toCharArray(display_data[display].imageFilename, 100);
  display_data[display].drawImage = true;
}

//Internal call. Directly draw an image
void _Display_SetImage(byte display, char *filename){
  _Display_DrawImage(display, filename, 0, 0);
}

void Display_ClearImage(byte display){
  Display_StopAnimation(display);
  
  displays[display].fillScreen(ST7735_BLACK);
  display_data[display].drawImage = false;
}

void Display_LoopAnimation(byte display, short animation){
  Display_StopAnimation(display);
  display_data[display].animationFilePosition = 0;
  display_data[display].animationID = animation;
  display_data[display].loopAnimation = 1;
}

void Display_PlayAnimationOnce(byte display, short animation){
  Display_StopAnimation(display);
  display_data[display].animationFilePosition = 0;
  display_data[display].animationID = animation;
  display_data[display].loopAnimation = 0;
}



void Display_randomMercTalk()
{
  /*
  int r = random(10);
  if(r <= 3)
    _Display_DrawImage(0, "/display/ml57_791.565", 57, 79);
  else if(r <= 6)
    _Display_DrawImage(0, "/display/ml57_792.565", 57, 79);
  else
    _Display_DrawImage(0, "/display/ml57_793.565", 57, 79);
  */
  static byte animTest = 2;
  if(animTest == 0)
    _Display_DrawImage(0, "/display/anim/1/0.565", 0, 0);
  else if(animTest == 1)
    _Display_DrawImage(0, "/display/anim/1/1.565", 0, 0);
  else
    _Display_DrawImage(0, "/display/anim/1/2.565", 0, 0);
  //animTest += 1;
  if(animTest == 3) animTest = 0;
}



//Update the display animation data
void _Display_UpdateAnimationData(byte displayID){
  if(display_data[displayID].animationID >= 0){
    File animationFile;

    //Get path to animation file
    display_str = "/display/anim/" + String(display_data[displayID].animationID) + "/anim.bad";
    display_str.toCharArray(display_animationFilename, 100);
    
    // Open requested file on SD card
    if ((animationFile = sd.open(display_animationFilename)) == NULL) { 
      #ifdef DISPLAY_SERIAL_DEBUG
      Serial.println(display_animationFilename);
      #endif
      return;
    }

    #ifdef DISPLAY_SERIAL_DEBUG
    Serial.print("found: ");
    Serial.println(display_animationFilename);
    #endif

    //Go to correct file position and start reading
    animationFile.seek(display_data[displayID].animationFilePosition);
    //Cursor is at beginning of file
    if(display_data[displayID].animationFilePosition == 0){
      //version
      animationFile.read();
      display_data[displayID].animationFilePosition += 1;

      //animation type
      display_data[displayID].animationType = animationFile.read();
      display_data[displayID].animationFilePosition += 1;
      
      #ifdef DISPLAY_SERIAL_DEBUG
      Serial.print("Animation Type: ");
      Serial.println(display_data[displayID].animationType);
      #endif

      if(display_data[displayID].animationType == 0){
        //simple animation
        display_data[displayID].frametime = Display_GetShort(&animationFile);
        display_data[displayID].frameImageID = 0;
        display_str = "/display/anim/" + String(display_data[displayID].animationID) + "/" + String(display_data[displayID].frameImageID)  + display_fileEnding;
        display_str.toCharArray(display_data[displayID].imageFilename, 100);
        display_data[displayID].drawImage = true;
        display_data[displayID].animationFilePosition = animationFile.position();
        
      } else if(display_data[displayID].animationType == 1){
        //complex animation

        //total framecount of the animation
        int16_t frameCount = Display_GetShort(&animationFile);
        display_data[displayID].animationFilePosition += 2;
        
        #ifdef DISPLAY_SERIAL_DEBUG
        Serial.print("Frame Count: ");
        Serial.println(frameCount);
        #endif

        //Get image path
        display_data[displayID].frameImageID =  Display_GetShort(&animationFile);
        display_data[displayID].animationFilePosition += 2;
        display_str = "/display/anim/" + String(display_data[displayID].animationID) + "/" + String(display_data[displayID].frameImageID)  + display_fileEnding;
        display_str.toCharArray(display_data[displayID].imageFilename, 100);

        #ifdef DISPLAY_SERIAL_DEBUG
        Serial.print("Image Path: ");
        Serial.println(display_data[displayID].imageFilename);
        #endif
        
        //Get image frametime
        display_data[displayID].frametime = Display_GetShort(&animationFile);
        display_data[displayID].animationFilePosition += 2;

        #ifdef DISPLAY_SERIAL_DEBUG
        Serial.print("Frametime: ");
        Serial.println(display_data[displayID].frametime);
        #endif

        display_data[displayID].drawImage = true;
      }
      
    } else {
      //continue with simple animation
      if(display_data[displayID].animationType == 0){
        display_data[displayID].frameImageID++;
        display_str = "/display/anim/" + String(display_data[displayID].animationID) + "/" + String(display_data[displayID].frameImageID)  + display_fileEnding;
        display_str.toCharArray(display_data[displayID].imageFilename, 100);
        //Check if next frame exists
        if(sd.exists(display_data[displayID].imageFilename)){
          display_data[displayID].drawImage = true;  
        } else {
          //If next frame does not exist but animation should loop, go back to first frame
          if(display_data[displayID].loopAnimation == 1){
            display_data[displayID].frameImageID = 0;
            display_str = "/display/anim/" + String(display_data[displayID].animationID) + "/" + String(display_data[displayID].frameImageID)  + display_fileEnding;
            display_str.toCharArray(display_data[displayID].imageFilename, 100);
            display_data[displayID].drawImage = true;
          }
          //If animation should not loop, stop animation
          else {
            Display_StopAnimation(displayID);
          }
        }
      }
      //continue with complex animation
      else if(display_data[displayID].animationType == 1){
        //display_data[displayID].imageID = Display_GetShort(animationFile);
        
        //Get image path
        display_data[displayID].frameImageID =  Display_GetShort(&animationFile);
        display_data[displayID].animationFilePosition += 2;
        display_str = "/display/anim/" + String(display_data[displayID].animationID) + "/" + String(display_data[displayID].frameImageID) + display_fileEnding;
        display_str.toCharArray(display_data[displayID].imageFilename, 100);

        #ifdef DISPLAY_SERIAL_DEBUG
        Serial.print("Image Path: ");
        Serial.println(display_data[displayID].imageFilename);
        #endif
        
        //Get image frametime
        display_data[displayID].frametime = Display_GetShort(&animationFile);
        display_data[displayID].animationFilePosition += 2;

        #ifdef DISPLAY_SERIAL_DEBUG
        Serial.print("Frametime: ");
        Serial.println(display_data[displayID].frametime);
        #endif

        display_data[displayID].drawImage = true;

        //Complex animation loop check and file position set
        if(animationFile.available()){
          display_data[displayID].animationFilePosition = animationFile.position();
        } else {
          display_data[displayID].animationFilePosition = 0;
          //Check if animation should be looped
          if(display_data[displayID].loopAnimation == 0){
            Display_StopAnimation(displayID);
          }
        }
      }
    }

    #ifdef DISPLAY_SERIAL_DEBUG
    Serial.println("-----");
    #endif  
    
    animationFile.close();
  }
}

void Display_Update(int deltaTime){
  
  for(int i=0; i<DISPLAY_MAX_COUNT; ++i){
    //display_frameLimitCounter[i] += deltaTime;  
    display_data[i].frametimeCounter += deltaTime;

    //Update animation data, draw new image
    if(display_data[i].frametimeCounter >= display_data[i].frametime){
      display_data[i].frametimeCounter = 0;

      //If animation is set, update animation data
      //UPDATE: Playing animations is very slow (display.pushColor takes time ...) . You should only play animations while balls are save and solenoids are off. Otherwise timings will be incorrect.
      _Display_UpdateAnimationData(i);

      //Draw image
      if(display_data[i].drawImage){
        _Display_SetImage(i, display_data[i].imageFilename);
        display_data[i].drawImage = false;
      }
    }

#ifdef DISPLAY_PARTIAL_UPDATE
    //If using partial display update, continue updating the display
    if(display_data[i].continuePartialUpdate == 1){
      _Display_SetImage(i, display_data[i].imageFilename);
    }
#endif
  }
  
  //delay(250);
  //Display_randomMercTalk();
}

