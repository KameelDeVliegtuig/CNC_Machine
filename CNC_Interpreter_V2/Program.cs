// See https://aka.ms/new-console-template for more information
using CNC_Interpreter_V2;
using System.Diagnostics;

Interpreter interpreter = new Interpreter();

Debug.WriteLine("Hello, World!");
interpreter.Interpret("M0 P2000");
interpreter.Interpret("M0 S5");
interpreter.Interpret("M0");

for (int i = 0; i < interpreter.Moves.Count; i++)
{
    Debug.WriteLine(interpreter.Moves[i]);
}
Debug.WriteLine("GoodBye");