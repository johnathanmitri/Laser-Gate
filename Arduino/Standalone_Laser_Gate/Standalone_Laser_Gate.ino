int Sense;
double BlockLength;
double BlockLengthMeters;
double BlockCount;
double BlockTotal;
String tempRead;
double intTime;
double finTime;
double expTime;
bool BlockStat;
double Velocity;
String BlockLenthText;
int Thresh;

void setup()
{
  Thresh = 130;
  Serial.begin(9600);
  pinMode(A0, INPUT);
  pinMode(51, OUTPUT);
  Serial.println("Please Enter The Length of Blockage In Milimeters");
  while (0 == 0)
  {
    if (Serial.available())
    {
      tempRead = Serial.readStringUntil('\n');
      break;
    }
  }
  BlockLength = tempRead.toDouble();
  BlockLengthMeters = BlockLength / 1000;
  Serial.print("Length Of Blockage In Meters: ");
  Serial.println(BlockLengthMeters, 5);
  Serial.println();
  Serial.println("Please Enter The Number of Blockages: ");
  while (0 == 0)
  {
    if (Serial.available())
    {
      tempRead = Serial.readStringUntil('\n');
      break;
    }
  }
  BlockTotal = tempRead.toInt();
  Serial.print("Number of Blockages: ");
  Serial.println(BlockTotal);
}

void loop()
{
  BlockStat = false;
  tempRead = "";
  Serial.println("Enter RUN To Begin Recording Data");
  while (tempRead != "RUN")
  {
    tempRead = Serial.readStringUntil('\n');
    if (analogRead(A0) >= Thresh)
    {
      digitalWrite(51, HIGH);
    }
    else
    {
      digitalWrite(51, LOW);
    }
  }
  Serial.println("Recording Data...");
  while (BlockCount < BlockTotal)
  {
    Sense = analogRead(A0);
    intTime = micros();
    digitalWrite(51, HIGH);
    while (Sense <= Thresh)
    {
      BlockStat = true;
      digitalWrite(51, LOW);
      Sense = analogRead(A0);
    }
    digitalWrite(51, HIGH);
    if (BlockStat == true)
    {
      BlockStat = false;
      BlockCount ++;
      finTime = micros();
      expTime = (finTime - intTime) / double(1000);
      Serial.print("The Following Values Are For Blockage Number: ");
      Serial.println(BlockCount);
      Velocity = BlockLengthMeters / (expTime*1000);
      Serial.print("Duration of Blockage in MilliSeconds: ");
      Serial.println(expTime, 5);
      Serial.print("Velocity (m/s): ");
      Serial.println(Velocity, 5);
      Serial.print("Velocity (mph): ");
      Serial.println(Velocity * 2.23694, 5);
      Serial.println("----------------------------------------------------------------");
    }
  }
  BlockCount = 0;
}
