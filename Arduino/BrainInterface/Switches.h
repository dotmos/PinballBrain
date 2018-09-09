

Adafruit_MCP23017 mcp;

byte switchState[SWITCH_MAX_COUNT];

void Switch_Setup(){
  //MCP23017
  mcp.begin(0);      // use mcp with address 0
  mcp.pinMode(0, INPUT);
  mcp.pullUp(0, HIGH);  // turn on a 100K pullup internally
  
  for(int i=0; i<SWITCH_MAX_COUNT; ++i){
    switchState[i] = 0;
  }
}

void Switch_Update(int deltaTime){
  //Check switches and send state to unity

  for(int i=0; i<SWITCH_MAX_COUNT; ++i){
    boolean isHigh = mcp.digitalRead(i);
    if(isHigh == false && switchState[i] == 1){
      switchState[i] = 0;
      byte bytes[3];
      bytes[0] = SWITCH_INACTIVE;
      //Switch number
      bytes[1] = lowByte(i);
      bytes[2] = highByte(i);
      Serial.write(bytes, 3);
      //Wait for bytes to be written before continuing
      Serial.flush();
    } else if(isHigh == true && switchState[i] == 0){
      switchState[i] = 1;
      byte bytes[3];
      bytes[0] = SWITCH_ACTIVE;
      //Switch number
      bytes[1] = lowByte(i);
      bytes[2] = highByte(i);
      Serial.write(bytes, 3);
      //Wait for bytes to be written before continuing
      Serial.flush();
    }
  }
}

