// See https://aka.ms/new-console-template for more information
using CNC_Interpreter_V2;
using Iot.Device.Nmea0183.Ais;
using System.ComponentModel.Design;
using System.Device.Pwm;
using System.Diagnostics;


Interpreter interpreter = new Interpreter();
GPIOControl gpioControl = new GPIOControl();
PresenceDetector presenceDetector = new PresenceDetector("/dev/serial0", 256000);
AxisControl axisControl = new AxisControl(35, null);
Debug.WriteLine("Hello, World!");

Coordinate coordinate = new Coordinate(0, -20, -96, false);
axisControl.Move(coordinate);


//interpreter.Interpret("G1");
//interpreter.Interpret("X0 Y4 Z0.1");
//interpreter.Interpret("M0 P2000");
//interpreter.Interpret("M0 S5");
//interpreter.Interpret("G0 X6 Y-2 Z2");
//interpreter.Interpret("G5 I0 J0 P5 Q2 X10 Y5");
//Console.WriteLine("Interpreter has finished");    