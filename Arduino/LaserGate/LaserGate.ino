int Thresh = 130; //originally 130
unsigned long startTime;
unsigned long endTime;
unsigned long blockageTime;
byte startByteArray[4];
byte endByteArray[4];
String msg;

void setup()
{
  Serial.begin(57600);
  pinMode(A4, INPUT);
  pinMode(13, OUTPUT);
  digitalWrite(13, HIGH);
}

void loop()
{
  while (Serial.available() == 0)
  {
  }
  Serial.readString(); //received 'yo'

  digitalWrite(13, LOW);
  Serial.print("supz");
  delay(20);

  while (Serial.available() == 0)
  {
    while (analogRead(A4) >= Thresh) //&& (Serial.available() == 0))
    { if (Serial.available() != 0)
        goto JustOutsideOfLoop;
    }

    //=== At this point, laser is obscured.
    startTime = micros();
    digitalWrite(13, LOW);

    while (analogRead(A4) <= Thresh) //&& (Serial.available() == 0))
    { if (Serial.available() != 0)
        goto JustOutsideOfLoop;
    }

    //=== At this point, laser is visible again.
    digitalWrite(13, HIGH);
    endTime = micros();   //i would combine this, but its  buggy, so leave as is.

    for (int i = 0; i < 4; i++)
    { 
      startByteArray[i] = startTime & 255;
      startTime >>= 8;

      endByteArray[i] = endTime & 255;
      endTime >>= 8;
    }

    Serial.write(endByteArray, 4);     //a slight obfuscation here by sending the end value first.
    Serial.write(startByteArray, 4);   
  }

JustOutsideOfLoop:
  digitalWrite(13, HIGH);
  delay(2000);
  Serial.readString(); //received bye
}
