// See https://aka.ms/new-console-template for more information
using CNC_Interpreter_V2;
using Iot.Device.Display;
using Iot.Device.Nmea0183.Ais;
using System.ComponentModel.Design;
using System.Device.Pwm;
using System.Diagnostics;


//static void Main(string[] args)
//{

Interpreter interpreter = new Interpreter();
GPIOControl gpioControl = new GPIOControl();
PresenceDetector presenceDetector = new PresenceDetector("/dev/ttyUSB0", 115200);
Coordinate coordinate = new Coordinate(10, 10, 10, true);
AxisControl axisControl = new AxisControl(25, null);

Thread GCodeThread = new(RunGCodes);


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

Console.WriteLine("Program started!");
GCodeThread.Start();

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
    Console.WriteLine("Moves length: " + interpreter.Moves.Count);
}

void RunGCodes()
{
    while (interpreter.Moves.Count == 0 || Globals.brake || Globals.stop) continue;
    interpreter.Moves[0].Print();
    axisControl.Move(interpreter.Moves[0]);
    interpreter.Moves.RemoveAt(0);
}


static class Globals
{
    public static bool stop = false;
    public static bool brake = false;
    public static int currentSpindelSpeed = 0;
}