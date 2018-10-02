#define SD_CS    4  // Chip select line for SD card

// File system object.
SdFat sd;

// Test with reduced SPI speed for breadboards.
// Change spiSpeed to SPI_FULL_SPEED for better performance
// Use SPI_QUARTER_SPEED for even slower SPI bus speed
const uint8_t spiSpeed = SPI_HALF_SPEED;

void SDCard_Setup(){
  if (!sd.begin(SD_CS, spiSpeed)) {
    //Serial.println("SDCard failed!");
    return;
  }
  //Serial.println("SDCard OK!");
}

