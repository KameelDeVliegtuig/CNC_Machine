// See https://aka.ms/new-console-template for more information
using CNC_Interpreter_V2;
using System.Diagnostics;

Interpreter interpreter = new Interpreter();

Debug.WriteLine("Hello, World!");
interpreter.Interpret("G17");
interpreter.Interpret("G18");
interpreter.Interpret("G19");
interpreter.Interpret("G0");
interpreter.Interpret("X5 Y-2");

for (int i = 0; i < interpreter.Moves.Count; i++)
{
    Debug.WriteLine(interpreter.Moves[i]);
}
Debug.WriteLine("GoodBye");