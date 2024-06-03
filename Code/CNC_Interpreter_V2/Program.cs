﻿// See https://aka.ms/new-console-template for more information
using CNC_Interpreter_V2;
using Iot.Device.Nmea0183.Ais;
using System.ComponentModel.Design;
using System.Device.Pwm;
using System.Diagnostics;
using (Process p = Process.GetCurrentProcess()) p.PriorityClass = ProcessPriorityClass.RealTime; // Hoogste prioriteit



Interpreter interpreter = new Interpreter();
GPIOControl gpioControl = new GPIOControl();
PresenceDetector presenceDetector = new PresenceDetector("/dev/ttyUSB0", 256000);
Coordinate coordinate = new Coordinate(10, 10, 10, true);

AxisControl axisControl = new AxisControl(100000, null);
Debug.WriteLine("Hello, World!");
 
for(int i = 0; i < 800; i++)
{
    gpioControl.ControlStep(true, GPIOControl.StepperAxis.X);
}
Console.WriteLine("Done Only Gpio Control");
Thread.Sleep(1000);

axisControl.Move(coordinate);

// Auto Home
interpreter.Interpret("G28");

Coordinate zerood = new Coordinate(0, 0, -21.5, false);

axisControl.Move(zerood);

gpioControl.ControlSpindel(10, true);

presenceDetector.StartListening();

while (true)
{
    string? GCode = Console.ReadLine();
    if(GCode == null) break;
    interpreter.Interpret(GCode);
    while(interpreter.Moves.Count > 0)
    {
        interpreter.Moves[0].Print();
        axisControl.Move(interpreter.Moves[0]);
    }
}


//interpreter.Interpret("G1");
//interpreter.Interpret("X0 Y4 Z0.1");
//interpreter.Interpret("M0 P2000");
//interpreter.Interpret("M0 S5");
//interpreter.Interpret("G0 X6 Y-2 Z2");
//interpreter.Interpret("G5 I0 J0 P5 Q2 X10 Y5");
//Console.WriteLine("Interpreter has finished");    