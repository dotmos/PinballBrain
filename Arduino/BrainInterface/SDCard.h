// File system object.
SdFat sd;

void SDCard_Setup(){
  if (!sd.begin(SD_CS, spiSpeed)) {
    //Serial.println("SDCard failed!");
    return;
  }
  //Serial.println("SDCard OK!");
}

