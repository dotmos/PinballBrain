short solenoid_activeTime[SOLENOID_MAX_COUNT];

#define SOLENOID_STATE_IDLE 0
#define SOLENOID_STATE_ACTIVATE 1
#define SOLENOID_STATE_DEACTIVATE 2
#define SOLENOID_STATE_TRIGGER 3
byte solenoid_state[SOLENOID_MAX_COUNT];
byte solenoid_isActive[SOLENOID_MAX_COUNT];

byte solenoids_active = 0; //Currently active solenoids
//------------------------------------------------------------

//TODO: More than one mcp for solenoids
Adafruit_MCP23017 mcp_solenoid;


void Solenoids_Setup(){
  for(int i=0; i<SOLENOID_MAX_COUNT; ++i){
    solenoid_activeTime[i] = -1;
    solenoid_state[i] = SOLENOID_STATE_IDLE;
  }

  //MCP23017
  mcp_solenoid.begin(SOLENOID_MCPID);      // use mcp with address 0
  //Setup all ports as output
  for(int p=0; p<MCP_IOCOUNT; ++p){
    mcp_solenoid.pinMode(p, OUTPUT);
    //mcp_solenoid.pullUp(p, LOW);  // turn off internal 100K pullup. We do NOT want this for the solenoids
    mcp_solenoid.digitalWrite(p, LOW);
  }
}


void _Solenoid_Activate(byte solenoid){  
  if(solenoid_isActive[solenoid] == 0 && solenoids_active < SOLENOID_MAX_CONCURRENT){
    mcp_solenoid.digitalWrite(solenoid, HIGH);
    solenoid_isActive[solenoid] = 1;
    solenoids_active += 1;
  }
}
void _Solenoid_Deactivate(byte solenoid){
  if(solenoid_isActive[solenoid] == 1){
    mcp_solenoid.digitalWrite(solenoid, LOW);
    solenoid_isActive[solenoid] = 0;
    solenoids_active -= 1;  
  }
}

//Mark a solenoid for activation
void Solenoid_Activate(byte solenoid){
  solenoid_state[solenoid] = SOLENOID_STATE_ACTIVATE;
  solenoid_activeTime[solenoid] = 0;
}

//Mark a solenoid for deactivation
void Solenoid_Deactivate(byte solenoid){
  solenoid_state[solenoid] = SOLENOID_STATE_DEACTIVATE;
  solenoid_activeTime[solenoid] = 0;
}

//Trigger a solenoid for a specific amount of time (in ms)
void Solenoid_Trigger(byte solenoid, short ms){
  Solenoid_Activate(solenoid);
  solenoid_activeTime[solenoid] = min(SOLENOID_MAX_ACTIVE_MS, ms);
}


//Update playfiel parts logic
void Solenoids_Update(int deltaTime){
  //Update solenoids
  for(int i=0; i<SOLENOID_MAX_COUNT; ++i){
    switch(solenoid_state[i]){
      case SOLENOID_STATE_ACTIVATE:
        _Solenoid_Activate(i);
        if(solenoid_isActive[i] == 1){
          if(solenoid_activeTime[i] > 0){
            solenoid_state[i] = SOLENOID_STATE_TRIGGER;
          } else {
            solenoid_state[i] = SOLENOID_STATE_IDLE;
          }
        }
      break;

      case SOLENOID_STATE_DEACTIVATE:
          _Solenoid_Deactivate(i);
          if(solenoid_isActive[i] == 0){
            solenoid_state[i] = SOLENOID_STATE_IDLE;  
          }
      break;

      case SOLENOID_STATE_TRIGGER:
        solenoid_activeTime[i] -= deltaTime;
        if(solenoid_activeTime[i] <= 0){
          solenoid_activeTime[i] = 0;
          solenoid_state[i] = SOLENOID_STATE_DEACTIVATE;
        }
      break;
      
      default:
      break;
    }
  }
}

