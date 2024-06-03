using Iot.Device.CharacterLcd;
using Iot.Device.Pn532;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;

namespace CNC_Interpreter_V2
{
    internal class Interpreter
    {
        Parser parser = new Parser();
        Settings settings = new Settings(0.0, 0.0, 0.0);
        //GPIOControl gpio = new GPIOControl();
        AxisControl axisControl = new AxisControl(0.1, null);

        const string firmwareInfo = "CNC Machine V0.1 by Lasse Houtenbos and Kamiel Groot ©2024";

        private System.Timers.Timer StopTimer = new System.Timers.Timer();
        private bool consoleInput;

        private List<Coordinate> moves = new List<Coordinate>();
        private string lastCommand;
        private int passcode = 0000;

        private long startTime;

        public List<Coordinate> Moves { get { return moves; } }

        public void Interpret(string GCODE)
        {
            string[] splitted = GCODE.Split(' ');
            Value value = createValue(splitted);
            //value.Print();
            switch (value.Command)
            {
                // G Codes
                case "G0":
                case "G1":
                //Debug.WriteLine("Linear Move");
                case "G2":
                case "G3":
                //Debug.WriteLine("Arc or Circle Move");
                case "G4":
                //Debug.WriteLine("Dwell");
                case "G5":
                //Debug.WriteLine("Bézier cubic spline");
                case "G6":
                    //Debug.WriteLine("Direct Stepper Move");
                    Debug.WriteLine("To Parser to calculate coordinates");
                    try
                    {
                        moves.AddRange(parser.Parse(new[] { settings.X, settings.Y, settings.Z }, value));
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }
                    break;

                // WorkPlane Select
                case "G17": // XY
                    Debug.WriteLine("CNC Workspace Planes");
                    settings.Workplane = (Settings.workplanes)0;
                    Debug.WriteLine("Workplane: " + settings.Workplane);
                    break;
                case "G18": // ZX
                    Debug.WriteLine("CNC Workspace Planes");
                    settings.Workplane = (Settings.workplanes)1;
                    Debug.WriteLine("Workplane: " + settings.Workplane);
                    break;
                case "G19": // YZ
                    Debug.WriteLine("CNC Workspace Planes");
                    settings.Workplane = (Settings.workplanes)2;
                    Debug.WriteLine("Workplane: " + settings.Workplane);
                    break;
                case "G20":
                    Debug.WriteLine("Units inch");
                    settings.MM = false;
                    break;
                case "G21":
                    Debug.WriteLine("Units mm");
                    settings.MM = true;
                    break;
                case "G27":
                    Debug.WriteLine("Park Toolhead");
                    try
                    {
                        moves.Add(settings.ParkTool(value.P));
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }
                    break;
                case "G28":
                    Debug.WriteLine("Auto Home");
                    AutoHome(GPIOControl.StepperAxis.Z, GPIOControl.LimitSwitch.Z);
                    AutoHome(GPIOControl.StepperAxis.X, GPIOControl.LimitSwitch.X);
                    AutoHome(GPIOControl.StepperAxis.Y, GPIOControl.LimitSwitch.Y);
                    Debug.WriteLine("Auto Home Complete");
                    break;
                //case "G34": // Checks if Z-rods are at the same position
                //    Debug.WriteLine("Z-axis Gantry Calibration");
                //    break;
                case "G42":
                    Debug.WriteLine("Move to mesh coordinate");
                    break;
                case "G53":
                    Debug.WriteLine("Move in Machine coordinates (fast move)");
                    try
                    {
                        moves.Add(new Coordinate(value.X, value.Y, value.Z, false));
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }
                    break;
                case "G54": // Slot 1
                case "G55": // Slot 2
                case "G56": // Slot 3
                case "G57": // Slot 4
                case "G58": // Slot 5
                case "G59": // Slot 6
                case "G59.1": // Slot 7
                case "G59.2": // Slot 8
                case "G59.3": // Slot 9
                    // Use G92 to set offset in current workspace
                    Debug.WriteLine("Workspace Coordinate System");
                    break;
                case "G60":
                    Debug.WriteLine("Save Current position");

                    break;
                case "G61":
                    Debug.WriteLine("Return to saved position");
                    break;
                case "G90":
                    Debug.WriteLine("Absolute Positioning");
                    break;
                case "G91":
                    Debug.WriteLine("Relative Positioning");
                    break;
                case "G92": // Disable <0 values
                    Debug.WriteLine("Set Position");
                    break;

                // M codes
                case "M0":
                case "M1":
                    Debug.WriteLine("Unconditional STOP");
                    consoleInput = false;
                    if (value.P > 0)
                    {
                        Debug.WriteLine("StopTime (ms): " + value.P);
                        StopTimer = new System.Timers.Timer(value.P); // milliseconds
                    }
                    else if (value.S > 0)
                    {
                        Debug.WriteLine("StopTime (s):" + value.S);
                        StopTimer = new System.Timers.Timer(value.S * 1000); // seconds * 1000 = milliseconds
                    }
                    else
                    {
                        Debug.WriteLine("Waiting for console input: ");
                        consoleInput = true;
                        StopTimer = new System.Timers.Timer(500); // Half a second
                    }
                    Debug.WriteLine("Timer Started at " + DateTime.Now.ToString("HH:mm:ss.fff"));
                    StopTimer.Enabled = true;
                    StopTimer.Elapsed += StopTimer_Elapsed;
                    if (consoleInput)
                    {
                        Console.ReadKey();
                        StopTimer.Stop();
                    }
                    while (StopTimer.Enabled) continue;
                    StopTimer.Dispose();
                    break;
                case "M3":
                    Debug.WriteLine("Turn on Spindel CW");
                    settings.SpindelDir = true;
                    settings.Spindel = true;
                    break;
                case "M4":
                    Debug.WriteLine("Turn on Spindel CCW");
                    settings.SpindelDir = false;
                    settings.Spindel = true;
                    break;
                case "M5":
                    Debug.WriteLine("Turn off Spindel");
                    settings.Spindel = false;
                    break;
                case "M16":
                    // Always return true (no printer check)
                    Debug.WriteLine("Device checker");
                    break;
                case "M18":
                case "M84":
                    Debug.WriteLine("Disable all steppers");
                    settings.StepperEnable = false;
                    axisControl.DisableStepper();
                    break;

                // Only when using SD Card
                case "M20":
                    Debug.WriteLine("List Items SD card");
                    break;
                case "M21":
                    Debug.WriteLine("Initialize SD card");
                    break;
                case "M22":
                    Debug.WriteLine("Release SD card");
                    break;
                case "M23":
                    Debug.WriteLine("Select file on SD");
                    break;
                case "M24":
                    Debug.WriteLine("Start/Resume SD file");
                    break;
                case "M25":
                    Debug.WriteLine("Pause SD file");
                    break;
                case "M27":
                    Debug.WriteLine("Report SD Execution Status");
                    break;
                case "M28": // May be discontinued
                    Debug.WriteLine("Start Write to SD Card");
                    break;
                case "M29": // May be discontinued
                    Debug.WriteLine("Stop Write to SD Card");
                    break;
                case "M30":
                    Debug.WriteLine("Delete file from SD");
                    break;

                case "M31":
                    Debug.WriteLine("Print Time");
                    if (startTime != null)
                    {
                        Console.WriteLine("Time Elapsed: " + Stopwatch.GetElapsedTime(startTime));
                    }
                    break;
                case "M32":
                    Debug.WriteLine("Begin from SD File");
                    startTime = Stopwatch.GetTimestamp();
                    break;
                // End of SD Card Section

                case "M42": // Digital pins
                    Debug.WriteLine("Set pin state");
                    bool sValue;
                    if (value.S > 0)
                    {
                        sValue = true;
                    }
                    else
                    {
                        sValue = false;
                    }
                    if (axisControl.SetPin((int)value.P, sValue))
                    {
                        Console.WriteLine("Pin " + value.P + " set to " + sValue);
                    }
                    else
                    {
                        Console.WriteLine("Could not set pin " + value.P);
                    }
                    break;

                // LCD
                case "M73":
                    Debug.WriteLine("Mannually set progress");
                    break;
                case "M75":
                    Debug.WriteLine("Start Print Job Timer");
                    break;
                case "M76":
                    Debug.WriteLine("Pause Print Job Timer");
                    break;
                case "M77":
                    Debug.WriteLine("Stop Print Job Timer");
                    break;
                // End LCD

                case "M92":
                    Debug.WriteLine("Set Axis Steps per unit (mm/in)");

                    if (settings.MM)
                    {
                        //if(value.E)
                    }
                    break;
                case "M100":
                    GC.Collect();
                    //return Process.GetCurrentProcess().PrivateMemorySize64;
                    break;
                case "M105":
                    Debug.WriteLine("Temperature Report");
                    break;
                case "M111":
                    Debug.WriteLine("Set debug level");
                    break;
                case "M112":
                    Debug.WriteLine("Full Shutdown");
                    Console.WriteLine("Shutting down within 1 minute...");
                    System.Diagnostics.Process.Start(new ProcessStartInfo() { FileName = "sudo", Arguments = "shutdown" });
                    break;
                case "M114":
                    Debug.WriteLine("Get Current Position");
                    Coordinate currentPos = new Coordinate(settings.X, settings.Y, settings.Z, settings.Spindel);
                    Console.WriteLine("Current Position: ");
                    currentPos.Print();
                    break;
                case "M115":
                    Debug.WriteLine("Firmware Info");
                    Console.WriteLine(firmwareInfo);
                    break;
                case "M117":
                    Debug.WriteLine("Set LCD Message");
                    Console.WriteLine("LCD Message");
                    break;
                case "M119":
                    Debug.WriteLine("Endstop States");
                    Console.WriteLine("Endstop States:");
                    Console.Write("X: " + axisControl.ReadLimitSwitch(GPIOControl.LimitSwitch.X));
                    Console.Write(", Y: " + axisControl.ReadLimitSwitch(GPIOControl.LimitSwitch.Y));
                    Console.WriteLine(", Z: " + axisControl.ReadLimitSwitch(GPIOControl.LimitSwitch.Z));
                    break;
                case "M149":
                    Debug.WriteLine("Set Temperature Units (C/F/K)");
                    Console.WriteLine("FUCK YOU!!!! It's Celsius, take it or leave it!");
                    break;
                case "M150":
                    Debug.WriteLine("Set LED(strip) RGB(W) Color");
                    break;

                case "M201":
                    Debug.WriteLine("Set MAX Acceleration");
                    break;
                case "M205":
                    Debug.WriteLine("Advanced axis Settings");
                    break;
                case "M206":
                    Debug.WriteLine("Home Offset");
                    break;
                case "M226":
                    Debug.WriteLine("Wait for pin state (0/1/-1)");
                    bool wanted = false;
                    bool[] currentState;
                    switch (value.S)
                    {
                        case 0:
                            wanted = false;
                            break;
                        case 1:
                            wanted = true;
                            break;
                        case -1:
                            currentState = axisControl.ReadPin((int)value.P);
                            if (currentState[0])
                            {
                                wanted = !currentState[1];
                            }
                            else
                            {
                                Console.WriteLine("Unable to read pin, check pin number");
                                break;
                            }
                            break;
                    }
                    currentState = axisControl.ReadPin((int)value.P);
                    while (currentState[0] && currentState[1] != wanted)
                    {
                        currentState = axisControl.ReadPin((int)value.P);
                    }
                    break;
                case "M256":
                    Debug.WriteLine("Set/Get LCD Brightness");
                    break;

                case "M300":
                    Debug.WriteLine("Play Tone");
                    break;
                case "M305":
                    Debug.WriteLine("Set MicroStepping");
                    Console.WriteLine("Microstepping fixed at 1/16");
                    break;
                case "M355":
                    Debug.WriteLine("Case Light Control");
                    break;

                case "M400":
                    Debug.WriteLine("Wait for moves to finish");
                    if (moves.Count != 0)
                    {
                        Console.WriteLine("Moves is not empty");
                    }
                    break;
                case "M401":
                    Debug.WriteLine("Deploy Bed Probe");
                    Console.WriteLine("Place Z-axis limit switch");
                    break;
                case "M402":
                    Debug.WriteLine("Stow Bed Probe");
                    Console.WriteLine("Remove Z-axis limit switch");
                    break;
                case "M410":
                    Debug.WriteLine("Stop all stepper instantly");
                    axisControl.DisableStepper();
                    break;
                case "M420":
                    Debug.WriteLine("Get/Set Bed Leveling State");
                    break;
                case "M428":
                    Debug.WriteLine("Set Home at Current Position");
                    break;

                case "M510":
                    Debug.WriteLine("Lock Machine");
                    settings.Lock(passcode);
                    break;
                case "M511":
                    Debug.WriteLine("Unlock Machine");
                    settings.Unlock((int)value.P);
                    break;
                case "M512":
                    Debug.WriteLine("Set Passcode");
                    if ((int)value.P == passcode)
                    {
                        passcode = (int)value.S;
                    }
                    break;
                case "M524":
                    Debug.WriteLine("Abort Print in Progress");
                    break;
                case "M575":
                    Debug.WriteLine("Change Serial BaudRate");
                    break;

                case "M928":
                    Debug.WriteLine("Start Logging to SD");
                    break;
                case "M995":
                    Debug.WriteLine("Calibrate Touchscreen");
                    break;
                // Custom
                case "X0":
                    Debug.WriteLine("Selective Stop");
                    settings.EmergencyStop = true;
                    axisControl.EmergencyStop();
                    break;
                default:
                    Debug.WriteLine("Code not Found");
                    break;
            }
        }

        private void StopTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            Debug.WriteLine("Timer Elapsed at {0:HH:mm:ss.fff}", e.SignalTime);
            if (!consoleInput) { StopTimer.Stop(); }
            return;
        }

        private Value createValue(string[] Input)
        {
            Value value = new Value();
            value.Workplane = settings.Workplane;
            if (Input == null)
            {
                return null;
            }
            if (Input[0][0] == 'G' || Input[0][0] == 'M' || Input[0][0] == 'X')
            {
                value.Command = Input[0];
                if (Input[0] == "G0" || Input[0] == "G1")
                {
                    lastCommand = Input[0];
                }
            }
            else
            {
                value.Command = lastCommand;
            }
            if (Input.Length > 1)
            {
                for (int i = 1; i < Input.Length; i++)
                {
                    switch (Input[i][0])
                    {

                        case 'E': // Extruder
                        case 'F': // FlowRate
                        case 'S': // Spindel
                                  // Combine all in case of wrong configuration
                            value.S = getDouble(Input[i]);
                            Debug.WriteLine("S: " + value.S);
                            break;
                        case 'I':
                            value.I = getDouble(Input[i]);
                            Debug.WriteLine("I: " + value.I);
                            break;
                        case 'J':
                            value.J = getDouble(Input[i]);
                            Debug.WriteLine("J: " + value.J);
                            break;
                        case 'R':
                            value.R = getDouble(Input[i]);
                            Debug.WriteLine("R: " + value.R);
                            break;
                        case 'P':
                            value.P = getDouble(Input[i]);
                            Debug.WriteLine("P: " + value.P);
                            break;
                        case 'Q':
                            value.Q = getDouble(Input[i]);
                            Debug.WriteLine("Q: " + value.Q);
                            break;
                        case 'X':
                            value.X = getDouble(Input[i]);
                            Debug.WriteLine("X: " + value.X);
                            break;
                        case 'Y':
                            value.Y = getDouble(Input[i]);
                            Debug.WriteLine("Y: " + value.Y);
                            break;
                        case 'Z':
                            value.Z = getDouble(Input[i]);
                            Debug.WriteLine("Z: " + value.Z);
                            break;
                        default:
                            break;
                    }
                }
            }
            return value;
        }


        private double getDouble(string Input)
        {
            string number = new string("");
            Input.Substring(1);
            for (int i = 1; i < Input.Length; i++)
            {
                if (Input[i] == '.')
                {
                    number += ',';
                }
                else
                {
                    number += Input[i];
                }
            }
            return double.Parse(number.ToString());
        }

        private bool AutoHome(GPIOControl.StepperAxis Axis, GPIOControl.LimitSwitch Switch)
        {

            // Make sure everything is off and stationary
            axisControl.Move(new Coordinate(0, 0, 0, false));

            double backDistance = 10.0;
            Coordinate Up = new Coordinate(0, 0, 0, false);
            Coordinate Down = new Coordinate(0, 0, 0, false);


            if (Axis == GPIOControl.StepperAxis.X)
            {
                Up = new Coordinate(-backDistance, 0, 0, false);
                Down = new Coordinate(0.1, 0, 0, false);
            }

            if (Axis == GPIOControl.StepperAxis.Y)
            {
                Up = new Coordinate(0, -backDistance, 0, false);
                Down = new Coordinate(0, 0.1, 0, false);
            }

            if (Axis == GPIOControl.StepperAxis.Z)
            {
                // Connect and press limitswitch manually
                Console.WriteLine("Connect Z-axis limit switch and press manually");
                while (axisControl.ReadLimitSwitch(GPIOControl.LimitSwitch.Z)) continue;
                while (!axisControl.ReadLimitSwitch(GPIOControl.LimitSwitch.Z)) continue;
                Thread.Sleep(2000);
                Up = new Coordinate(0, 0, backDistance, false);
                Down = new Coordinate(0, 0, (-0.1), false);
            }


            Console.WriteLine("Starting Auto Home");
            // Move axis until it is touching limitswitch
            Console.WriteLine("Move axis until it is touching limitswitch");
            while (axisControl.ReadLimitSwitch(Switch))
            {
                axisControl.Move(Down);
            }
            Console.WriteLine("Press detected");

            // Move back up
            Console.WriteLine("Moving back up " + backDistance + "mm");
            axisControl.Move(Up);

            // Move down slower
            Console.WriteLine("Moving down slower");
            while (axisControl.ReadLimitSwitch(Switch))
            {
                axisControl.Move(Down);
                Thread.Sleep(0);
            }
            Console.WriteLine("Press detected");

            // Set position to 0
            Console.WriteLine("Setting " + Axis + " to 0.0mm");
            if (Axis == GPIOControl.StepperAxis.X) settings.X = 0.0 + settings.SpindelToProbe[0];
            if (Axis == GPIOControl.StepperAxis.Y) settings.Y = 0.0 + settings.SpindelToProbe[1];
            if (Axis == GPIOControl.StepperAxis.Z) settings.Z = 0.0 + settings.SpindelToProbe[2];

            switch (Axis)
            {
                case GPIOControl.StepperAxis.Y:
                    Up = new Coordinate(0, settings.SpindelToProbe[1], 0, false);
                    axisControl.Move(Up);
                    break;
                case GPIOControl.StepperAxis.Z:
                    axisControl.Move(Up);
                    settings.Z = backDistance + settings.SpindelToProbe[2];
                    Console.WriteLine("Remove Z-axis Switch");
                    // Wait for switch to be removed
                    Thread.Sleep(5000);
                    break;
                default:
                    break;
            }
            return true;
        }

    }
}
