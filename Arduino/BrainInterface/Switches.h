//Do not change
#define SWITCH_MCP_COUNT ((int)ceil(SWITCH_MAX_COUNT/MCP_IOCOUNT)) //MCP23017 count needed for switches

Adafruit_MCP23017 mcp[SWITCH_MCP_COUNT];

byte switchState[SWITCH_MAX_COUNT];

void Switch_Setup(){
  for(int i=0; i<SWITCH_MCP_COUNT; ++i){
    //MCP23017
    mcp[i].begin(i);      // use mcp with address 0
    //Setup all ports as input
    for(int p=0; p<MCP_IOCOUNT; ++p){
      mcp[i].pinMode(p, INPUT);
      mcp[i].pullUp(p, HIGH);  // turn on a 100K pullup internally  
    }
  }
  
  for(int i=0; i<SWITCH_MAX_COUNT; ++i){
    switchState[i] = 0;
  }
}

void Switch_Update(int deltaTime){
  //Check switches and send state to unity

  for(int i=0; i<SWITCH_MAX_COUNT; ++i){
    int mcpID = (i/MCP_IOCOUNT);
    
    boolean pressed = !mcp[mcpID].digitalRead(i); //Negate the input, as pullUP resitors are used and mcp will trigger on GND
    if(pressed == false && switchState[i] == 1){
      switchState[i] = 0;
      byte bytes[3];
      bytes[0] = SWITCH_INACTIVE;
      //Switch number
      bytes[1] = lowByte(i);
      bytes[2] = highByte(i);
      Serial.write(bytes, 3);
      //Wait for bytes to be written before continuing
      Serial.flush();
    } else if(pressed == true && switchState[i] == 0){
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

