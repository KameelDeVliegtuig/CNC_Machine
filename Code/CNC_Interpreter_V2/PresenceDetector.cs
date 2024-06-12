using CNC_Interpreter_V2;
using System;
using System.IO.Ports;
using System.Numerics;
using UnitsNet;

public class PresenceDetector
{

    struct Location
    {
        public short X;
        public short Y;
    }
    private Location currentLocation = new Location();

    GPIOControl gpioControl = new GPIOControl();
    
    //  Define the data frame structure and settings
    private const int DataLength = 30; // Length of the data frame
    private SerialPort serialPort; // Serial port for communication
    private byte[] buffer = new byte[DataLength * 2]; // Buffer for received data
    private int bufferIndex = 0; 

    // Define the current state and listening thread
    private bool currentState;
    private Thread listeningThread;
    public bool IsDetected { get; private set; }

    //  Define the limits for presence detection when to slow down and when to stop
    private const short brakingLimitX = 452;
    private const short brakingLimitY = 500;
    private const short stopLimitX = 0;
    private const short stopLimitY = 0;


    


    public bool IsPresenceDetected { get; private set; }

    public PresenceDetector(string portName, int baudRate)
    {
        serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
        serialPort.DataReceived += SerialPortDataReceived;
        currentState = false;

        // Start a new thread to listen for data
        listeningThread = new Thread(StartListening);
        listeningThread.Start();

    }

    public void StartListening()
    {
        try
        {
            serialPort.Open();
            while (true)
            {
                if (currentLocation.X < brakingLimitX && currentLocation.Y < brakingLimitY && currentLocation.X > stopLimitX && currentLocation.Y > stopLimitY)
                {
                    gpioControl.ControlSpindel(30);
                    Globals.brake = true;
                    

                }
                else if(currentLocation.X <= stopLimitX && currentLocation.Y <= stopLimitY)
                {
                    Globals.stop = true;
                    gpioControl.ControlSpindel(-1);
                }
                else
                {
                    Globals.brake = false;
                    Globals.stop = false;
                }


            } // Keep the program running (listening for data)
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            serialPort.Close();
        }
    }

    private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        SerialPort serialPort = (SerialPort)sender;

        // Read all available data into the buffer
        int bytesRead = serialPort.Read(buffer, bufferIndex, buffer.Length - bufferIndex);
        bufferIndex += bytesRead;

        // Process the buffer to find valid frames
        ProcessBuffer();
    }

    private void ProcessBuffer()
    {
        int i = 0;
        while (i <= bufferIndex - DataLength)
        {
            // Check for the header and end of frame bytes
            if (buffer[i] == 0xAA && buffer[i + 1] == 0xFF && buffer[i + 2] == 0x03 && buffer[i + 3] == 0x00 &&
                buffer[i + DataLength - 2] == 0x55 && buffer[i + DataLength - 1] == 0xCC)
            {
                // Extract the valid frame
                byte[] frame = new byte[DataLength];
                Array.Copy(buffer, i, frame, 0, DataLength);

                // Process the frame
                ProcessFrame(frame);

                // Move past this frame in the buffer
                i += DataLength;
            }
            else
            {
                i++;
            }
        }

        // Shift any remaining bytes to the beginning of the buffer
        int remainingBytes = bufferIndex - i;
        if (remainingBytes > 0)
        {
            Array.Copy(buffer, i, buffer, 0, remainingBytes);
        }
        bufferIndex = remainingBytes;
    }

    private void ProcessFrame(byte[] frame)
    {
        // Extract target information
        short xCoordinate = BitConverter.ToInt16(frame, 4);
        short yCoordinate = BitConverter.ToInt16(frame, 6);
        yCoordinate = (short)(yCoordinate - 32768);

        if (xCoordinate < 0)
        {
            xCoordinate = 0;
        }
        if (yCoordinate < 0)
        {
            yCoordinate = 0;
        }

        currentLocation.X = xCoordinate;
        currentLocation.Y = yCoordinate;

        // Check if presence is detected within a certain range
        IsPresenceDetected = CheckPresenceDetected(xCoordinate, yCoordinate, 400); // Example range: 1000 mm

        Console.WriteLine($"Presence detected: {IsPresenceDetected}, X: {xCoordinate}, Y: {yCoordinate}");

    }

    private bool CheckPresenceDetected(short xDistance, short yDistance, int range)
    {
        // Check if the presence is within the specified range
        bool isDetected = (xDistance <= 452 && xDistance > 0) && (yDistance <= 500);

        // Flip the current state
        currentState = !currentState;

        // Return the detection status
        return isDetected;
    }
}
