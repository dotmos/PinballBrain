short solenoid_activeTime[SOLENOID_MAX_COUNT];

#define SOLENOID_STATE_IDLE 0
#define SOLENOID_STATE_ACTIVATE 1
#define SOLENOID_STATE_DEACTIVATE 2
#define SOLENOID_STATE_TRIGGER 3
byte solenoid_state[SOLENOID_MAX_COUNT];
byte solenoid_isActive[SOLENOID_MAX_COUNT];

byte solenoids_active = 0; //Currently active solenoids
//------------------------------------------------------------


void Solenoids_Setup(){
  for(int i=0; i<SOLENOID_MAX_COUNT; ++i){
    solenoid_activeTime[i] = -1;
    solenoid_state[i] = SOLENOID_STATE_IDLE;
  }
}


void _Solenoid_Activate(byte solenoid){
  solenoid_isActive[solenoid] = 1;
  // todo
}
void _Solenoid_Deactivate(byte solenoid){
  solenoid_isActive[solenoid] = 0;
  // todo
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
  solenoid_activeTime[solenoid] = ms;
}


//Update playfiel parts logic
void Solenoids_Update(int deltaTime){

  //Update solenoids
  for(int i=0; i<SOLENOID_MAX_COUNT; ++i){
    switch(solenoid_state[i]){
      case SOLENOID_STATE_ACTIVATE:
        if(solenoids_active < SOLENOID_MAX_CONCURRENT && solenoid_isActive[i] == 0){
          _Solenoid_Activate(i);
          if(solenoid_activeTime[i] > 0){
            solenoid_state[i] = SOLENOID_STATE_TRIGGER;
          } else {
            solenoid_state[i] = SOLENOID_STATE_IDLE;
          }
          solenoids_active += 1;
        }
      break;

      case SOLENOID_STATE_DEACTIVATE:
        if(solenoid_isActive[i] == 1){
          _Solenoid_Deactivate(i);
          solenoid_state[i] = SOLENOID_STATE_IDLE;  
          solenoids_active -= 1;
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

