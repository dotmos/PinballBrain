

void Serial_Setup(){
  Serial.begin(57600);
}


short Serial_GetShort(){
  byte shortByte[2];
  shortByte[0] = Serial.read();
  shortByte[1] = Serial.read();

  return read16(shortByte);
}

byte Serial_GetByte(){
  return Serial.read();
}

byte Serial_GetMessageHeader(){
  return Serial_GetByte();
}

short Serial_GetLEDId(){
  short led = Serial_GetShort();
  if(led >= LED_MAX_COUNT){
    led = LED_MAX_COUNT - 1;
  }
  return led;
}

void Serial_Update(int deltaTime){
  if (Serial.available() > 0) {
    byte header = Serial_GetMessageHeader();


    if(header == LED_ACTIVATE){
      short led = Serial_GetLEDId();
      
      byte red = Serial_GetByte();
      byte green = Serial_GetByte();
      byte blue = Serial_GetByte();
      
      LED_SetColor(led, red, green, blue);
      LED_Activate(led);
    }
    else if(header == LED_DEACTIVATE){
      LED_Deactivate(Serial_GetLEDId());
    }
    else if(header == LED_BLINK){
      short led = Serial_GetLEDId();
      byte red = Serial_GetByte();
      byte green = Serial_GetByte();
      byte blue = Serial_GetByte();
      short interval = Serial_GetShort();

      LED_SetColor(led, red, green, blue);
      LED_Blink(led, interval);
    }
    else if(header == LED_BLINK_AMOUNT){
      short led = Serial_GetLEDId();
      byte red = Serial_GetByte();
      byte green = Serial_GetByte();
      byte blue = Serial_GetByte();
      short interval = Serial_GetShort();

      LED_SetColor(led, red, green, blue);
      LED_Blink(led, interval, Serial.read());
    } 
    else if(header == DISPLAY_SET_IMAGE){
      byte display = Serial_GetByte();
      short image = Serial_GetShort();

      Display_SetImage(display, image);
    }
  }
}
