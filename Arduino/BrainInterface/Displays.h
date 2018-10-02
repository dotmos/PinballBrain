const int display_frameLimit = 128; //16ms = 60fps, 32ms = 30fps, 64ms = 15fps, etc.
int display_frameLimitCounter = 0;

Teensy_ST7735 display1 = Teensy_ST7735(TFT_CS, TFT_DC, TFT_RST);

//char helper
char display_imageFilename[100];
String display_str;

//image to draw on the display(s) on next screen refresh.
//TODO: Multiple displays
short display_imageBuffer = -1; 

//Draws an image from a file. Image needs to be converted with "CustomImageConverter" by dotmos.org. (converts .bmp/.jpg to Color565 format)
void _Display_DrawImage(char *filename, uint8_t x, uint8_t y) {
  
  File     bmpFile;
  int      bmpWidth, bmpHeight;   // W+H in pixels
  
  if((x >= display1.width()) || (y >= display1.height())) return;

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
  
  display1.setRotation(1);
  display1.setAddrWindow(x, y, x+bmpWidth-1, y+bmpHeight-1);
  
  
  //Read complete file content at once and store in framebuffer (warning, this will consume up to 41kB of ram!). Then push framebuffer to display. This is very fast, but uses a lot of ram.
  //"Framebuffer"
  uint8_t colorBuffer[2*bmpWidth*bmpHeight];
  bmpFile.read(colorBuffer, sizeof(colorBuffer));
  
  uint16_t result;
  for(int i=0; i<sizeof(colorBuffer); i+=2)
  {
    ((uint8_t *)&result)[0] = colorBuffer[i]; // LSB
    ((uint8_t *)&result)[1] = colorBuffer[i+1]; // MSB
    
    display1.pushColor(result);
  }
  
  
  /*
  //Stream image, pixel by pixel, to display. This is very slow, but uses almost no ram.
  uint16_t result;
  for(int i=0; i<bmpWidth*bmpHeight; ++i)
  {
    ((uint8_t *)&result)[0] = bmpFile.read(); // LSB
    ((uint8_t *)&result)[1] = bmpFile.read(); // MSB
    
    display1.pushColor(result);
  }
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
      display1.pushColor(result);
    }
  }
  */
  
  //Close file
  bmpFile.close();
}

//Draw an image on next display refresh
void Display_SetImage(byte display, short image){
  display_imageBuffer = image; 
}

//Internal call. Directly draws an image
void _Display_SetImage(byte display, short image){
  display_str = "/lcd/" + String(image) + ".bin";
  display_str.toCharArray(display_imageFilename, 100);
  _Display_DrawImage(display_imageFilename, 0, 0);
}

void Display_Setup(){
  // Use this initializer if you're using a 1.8" TFT
  display1.initR(INITR_BLACKTAB);

  // Use this initializer (uncomment) if you're using a 1.44" TFT
  //display1.initR(INITR_144GREENTAB);

  //_Display_DrawImage("/lcd/ml.lcd", 0, 0);
}

void Display_randomMercTalk()
{
  int r = random(10);
  if(r <= 3)
    _Display_DrawImage("/lcd/ml57_791.lcd", 57, 79);
  else if(r <= 6)
    _Display_DrawImage("/lcd/ml57_792.lcd", 57, 79);
  else
    _Display_DrawImage("/lcd/ml57_793.lcd", 57, 79);
}

void Display_Update(int deltaTime){
  display_frameLimitCounter += deltaTime;
  if(display_frameLimitCounter >= display_frameLimit){
    //Update display animation(s)
    display_frameLimitCounter = 0;
    //Display_randomMercTalk();
    if(display_imageBuffer > -1){
      _Display_SetImage(0, display_imageBuffer);
      display_imageBuffer = -1;
    }
  }
}

