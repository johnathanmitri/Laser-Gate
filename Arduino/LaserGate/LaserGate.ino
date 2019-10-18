int Thresh = 130; //originally 130
unsigned long intTime;
unsigned long endTime;
unsigned long blockageTime;
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
  byte ByteArray[4];
  
  while (Serial.available() == 0)
  {
    while (analogRead(A4) >= Thresh) //&& (Serial.available() == 0))
    { if (Serial.available() != 0)
        goto JustOutsideOfLoop;
    }
    
//=== At this point, laser is obscured.
    intTime = micros();
    digitalWrite(13, LOW);

    while (analogRead(A4) <= Thresh) //&& (Serial.available() == 0))
    { if (Serial.available() != 0)
        goto JustOutsideOfLoop;
    }

//=== At this point, laser is visible again.    
    digitalWrite(13, HIGH);
    endTime = micros();   //i would combine this, but its  buggy, so leave as is.
    blockageTime = endTime - intTime;

for (int i = 0; i < 4; i++)
{ ByteArray[i] = blockageTime & 255;
  blockageTime >>= 8;
}
    
    Serial.write(ByteArray, 4);
  }

JustOutsideOfLoop:
  digitalWrite(13, HIGH);
  delay(2000);
  Serial.readString(); //received bye
}
