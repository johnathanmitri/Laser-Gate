int Thresh = 700; //originally 130
unsigned long startTime;
unsigned long endTime;
unsigned long blockageTime;
byte startByteArray[4];
byte endByteArray[4];
String msg;

void setup()
{
  Serial.begin(57600);
  pinMode(A3, INPUT);
  pinMode(13, OUTPUT);
  digitalWrite(13, HIGH);
}

void loop()
{
  while (Serial.available() == 0)
  {
    delay(10);
  }
  Serial.readString(); //received 'hi'

  digitalWrite(13, LOW);
  Serial.print("helloz");
  delay(20);

  while (Serial.available() == 0)
  {
    while (analogRead(A3) >= Thresh) //&& (Serial.available() == 0))
    { 
      //Serial.println(analogRead(A3));
      if (Serial.available() != 0)
        goto JustOutsideOfLoop;
    }

    //=== At this point, laser is obscured.
    startTime = micros();
    digitalWrite(13, LOW);

    while (analogRead(A3) <= Thresh) //&& (Serial.available() == 0))
    {
      //Serial.println(analogRead(A3));
       if (Serial.available() != 0)
        goto JustOutsideOfLoop;
    }

    //=== At this point, laser is visible again.
    digitalWrite(13, HIGH);
    endTime = micros(); 

    for (int i = 0; i < 4; i++)
    { 
      startByteArray[i] = startTime & 255;
      startTime >>= 8;

      endByteArray[i] = endTime & 255;
      endTime >>= 8;
    }

    Serial.write(endByteArray, 4);    
    Serial.write(startByteArray, 4);   
  }

JustOutsideOfLoop:
  digitalWrite(13, HIGH);
  delay(2000);
  Serial.readString(); //received bye
}
