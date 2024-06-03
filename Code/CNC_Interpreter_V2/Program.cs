﻿// See https://aka.ms/new-console-template for more information
using CNC_Interpreter_V2;
using Iot.Device.Nmea0183.Ais;
using System.ComponentModel.Design;
using System.Device.Pwm;
using System.Diagnostics;
using (Process p = Process.GetCurrentProcess()) p.PriorityClass = ProcessPriorityClass.RealTime; // Hoogste prioriteit



//static void Main(string[] args)
//{
Interpreter interpreter = new Interpreter();
GPIOControl gpioControl = new GPIOControl();
PresenceDetector presenceDetector = new PresenceDetector("/dev/ttyUSB0", 256000);
Coordinate coordinate = new Coordinate(10, 10, 10, true);

AxisControl axisControl = new AxisControl(25, null);



//if (args.Length == 0)
//{
//    Console.WriteLine("No arguments while starting program");
//}
//else
//{
//    for (int i = 0; i < args.Length; i++)
//    {
//        Console.WriteLine("Argument " + i + 1 + ": " + args[i]);
//    }
//}

Console.WriteLine("Hello, World!");


while (true)
{
    string? GCode = Console.ReadLine();
    if (GCode == null) break;
    try
    {
        interpreter.Interpret(GCode);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
    Console.WriteLine("Moves length: " + interpreter.Moves.Count());
    for (int i = 0; i < interpreter.Moves.Count; i++)
    {
        interpreter.Moves[0].Print();
        axisControl.Move(interpreter.Moves[0]);
        interpreter.Moves.RemoveAt(0);
    }
    interpreter.Interpret("M5"); // Stop spindel at the end
}

//}
//interpreter.Interpret("G1");
//interpreter.Interpret("X0 Y4 Z0.1");
//interpreter.Interpret("M0 P2000");
//interpreter.Interpret("M0 S5");
//interpreter.Interpret("G0 X6 Y-2 Z2");
//interpreter.Interpret("G5 I0 J0 P5 Q2 X10 Y5");
//Console.WriteLine("Interpreter has finished");    