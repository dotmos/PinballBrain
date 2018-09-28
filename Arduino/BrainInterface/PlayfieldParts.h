// LED -------------------------------------------------------
short led_blinkInterval[LED_MAX_COUNT];
short led_blinkCurrentInterval[LED_MAX_COUNT];
byte led_blinkAmount[LED_MAX_COUNT];
byte led_blinkCurrentAmount[LED_MAX_COUNT];
int led_color[LED_MAX_COUNT];

#define LED_STATE_IDLE 0
#define LED_STATE_ACTIVATE 1
#define LED_STATE_DEACTIVATE 2
#define LED_STATE_BLINK_ON 3
#define LED_STATE_BLINK_OFF 4
byte led_state[LED_MAX_COUNT];

byte drawingMemory[LED_MAX_COUNT*3];         //  3 bytes per LED
DMAMEM byte displayMemory[LED_MAX_COUNT*12]; // 12 bytes per LED
WS2812Serial leds(LED_MAX_COUNT, displayMemory, drawingMemory, ledSerialPin, WS2812_RGB);

//------------------------------------------------------------

// SOLENOID --------------------------------------------------
short solenoid_activeTime[SOLENOID_MAX_COUNT];

#define SOLENOID_STATE_IDLE 0
#define SOLENOID_STATE_ACTIVATE 1
#define SOLENOID_STATE_DEACTIVATE 2
#define SOLENOID_STATE_TRIGGER 3
byte solenoid_state[SOLENOID_MAX_COUNT];
byte solenoid_isActive[SOLENOID_MAX_COUNT];

byte solenoid_active = 0;
//------------------------------------------------------------


void PlayfieldParts_Setup(){
  for(int i=0; i<LED_MAX_COUNT; ++i){
    led_blinkInterval[i] = 0;
    led_blinkAmount[i] = 0;
    led_state[i] = LED_STATE_IDLE;
    led_color[i] = 0x000000;
  }
  for(int i=0; i<SOLENOID_MAX_COUNT; ++i){
    solenoid_activeTime[i] = -1;
    solenoid_state[i] = SOLENOID_STATE_IDLE;
  }

  leds.begin();
}


//-----------------------------------
// LEDs
//-----------------------------------

void _LED_Activate(short led){
  //digitalWrite(led, HIGH);
  leds.setPixel(led, led_color[led]);
}
void _LED_Deactivate(short led){
  leds.setPixel(led, 0x000000);
  //digitalWrite(led, LOW);
}

void LED_SetColor(short led, byte red, byte green, byte blue){
  int color = ((int)red << 16) | ((int)green << 8) | blue;
  led_color[led] = color;
}

//Mark an LED to activate
void LED_Activate(short led){
  led_state[led] = LED_STATE_ACTIVATE;
}

//Mark an LED to deactivate
void LED_Deactivate(short led){
  led_state[led] = LED_STATE_DEACTIVATE;
}

//Mark an LED to blink
void LED_Blink(short led, short interval){
  led_blinkCurrentInterval[led] = 0;
  led_blinkCurrentAmount[led] = 0;
  led_blinkInterval[led] = interval;
  led_blinkAmount[led] = -1;
  led_state[led] = LED_STATE_BLINK_ON;
  _LED_Activate(led);
}

//Mark an LED to blink for a fixed amount of times
void LED_Blink(short led, short interval, byte amount){
  LED_Blink(led, interval);
  led_blinkAmount[led] = amount;
}



//-----------------------------------
// Solenoids
//-----------------------------------

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
void PlayfieldParts_Update(int deltaTime){

  //Update solenoids
  for(int i=0; i<SOLENOID_MAX_COUNT; ++i){
    switch(solenoid_state[i]){
      case SOLENOID_STATE_ACTIVATE:
        if(solenoid_active < SOLENOID_MAX_CONCURRENT && solenoid_isActive[i] == 0){
          _Solenoid_Activate(i);
          if(solenoid_activeTime[i] > 0){
            solenoid_state[i] = SOLENOID_STATE_TRIGGER;
          } else {
            solenoid_state[i] = SOLENOID_STATE_IDLE;
          }
          solenoid_active += 1;
        }
      break;

      case SOLENOID_STATE_DEACTIVATE:
        if(solenoid_isActive[i] == 1){
          _Solenoid_Deactivate(i);
          solenoid_state[i] = SOLENOID_STATE_IDLE;  
          solenoid_active -= 1;
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

  //Update displays
  // todo
  
  //Update lights
  for(int i=0; i<LED_MAX_COUNT; ++i){
    switch(led_state[i]){
      case LED_STATE_ACTIVATE:
        _LED_Activate(i);
        led_state[i] = LED_STATE_IDLE;
      break;
      
      case LED_STATE_DEACTIVATE:
        _LED_Deactivate(i);
        led_state[i] = LED_STATE_IDLE;
      break;

      case LED_STATE_BLINK_ON:
        led_blinkCurrentInterval[i] += deltaTime;
        if(led_blinkCurrentInterval[i] >= led_blinkInterval[i]){
          led_blinkCurrentInterval[i] = 0;
          _LED_Deactivate(i);

          if(led_blinkAmount[i] > 0){
            led_blinkAmount[i] -= 1;
            if(led_blinkAmount[i] == 0){
              led_state[i] = LED_STATE_IDLE;
            } else {
              led_state[i] = LED_STATE_BLINK_OFF;
            }
          } else {
            led_state[i] = LED_STATE_BLINK_OFF;  
          }
          
        }
      break;

      case LED_STATE_BLINK_OFF:
        led_blinkCurrentInterval[i] += deltaTime;
        if(led_blinkCurrentInterval[i] >= led_blinkInterval[i]){
          led_blinkCurrentInterval[i] = 0;
          _LED_Activate(i);
         led_state[i] = LED_STATE_BLINK_ON;
        }
      break;
      
      default:
      break;
    }
  }
  leds.show();
}

