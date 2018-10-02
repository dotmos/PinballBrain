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

void LEDs_Setup(){
  for(int i=0; i<LED_MAX_COUNT; ++i){
    led_blinkInterval[i] = 0;
    led_blinkAmount[i] = 0;
    led_state[i] = LED_STATE_IDLE;
    led_color[i] = 0x000000;
  }

  leds.begin();
}


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



//Update led logic
void LEDs_Update(int deltaTime){

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

