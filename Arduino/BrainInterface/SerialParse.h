

void Serial_Setup(){
  Serial.begin(9600);
}


short Serial_GetShort(){
  byte shortByte[2];
  shortByte[0] = Serial.read();
  shortByte[1] = Serial.read();

  return read16(shortByte);
}

void Serial_Update(int deltaTime){
  if (Serial.available() > 0) {
    byte header = Serial.read();


    if(header == LED_ACTIVATE){
      LED_Activate(Serial_GetShort());
    } else if(header == LED_DEACTIVATE){
      LED_Deactivate(Serial_GetShort());
    } else if(header == LED_BLINK){
      short led = Serial_GetShort();
      short interval = Serial_GetShort();
     
      LED_Blink(led, interval);
    } else if(header == LED_BLINK_AMOUNT){
      short led = Serial_GetShort();
      short interval = Serial_GetShort();
      
      LED_Blink(led, interval, Serial.read());
    } 
  }
}
