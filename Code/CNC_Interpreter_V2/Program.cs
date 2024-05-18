// See https://aka.ms/new-console-template for more information
using CNC_Interpreter_V2;
using System.Diagnostics;

Interpreter interpreter = new Interpreter();
GPIOControl gpioControl = new GPIOControl();


Debug.WriteLine("Hello, World!");

gpioControl.Test();

interpreter.Interpret("G1");
interpreter.Interpret("X0 Y4 Z0.1");
interpreter.Interpret("M0 P2000");
interpreter.Interpret("M0 S5");
interpreter.Interpret("G0 X6 Y-2 Z2");
interpreter.Interpret("G5 I0 J0 P5 Q2 X10 Y5");
Console.WriteLine("Interpreter has finished");

for (int i = 0; i < interpreter.Moves.Count; i++)
{
    Debug.Write("Moves " + i + ": ");
    Debug.WriteLine(interpreter.Moves[i]);
}

Debug.WriteLine("GoodBye");