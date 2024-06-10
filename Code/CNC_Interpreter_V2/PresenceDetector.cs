using System;
using System.ComponentModel;
using System.IO.Ports;

public class PresenceDetector
{
    private const int DataLength = 24; // Length of the data frame
    private SerialPort serialPort;
    private bool currentState;

    public bool IsPresenceDetected { get; private set; }

    public PresenceDetector(string portName, int baudRate)
    {
        serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
        serialPort.DataReceived += SerialPortDataReceived;
        currentState = false;
    }

    public void StartListening()
    {
        try
        {
            serialPort.Open();
            Console.WriteLine("Listening for radar data. Press Enter to exit.");
            Console.ReadLine(); // Keep the program running until Enter is pressed
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

        // Read all available data into a temporary buffer
        byte[] tempBuffer = new byte[serialPort.BytesToRead];
        serialPort.Read(tempBuffer, 0, tempBuffer.Length);
        
        // Process the buffer to find valid frames
        ProcessBuffer(tempBuffer);
    }

    private void ProcessBuffer(byte[] buffer)
    {
        int bufferLength = buffer.Length;
        for (int i = 0; i <= bufferLength - DataLength; i++)
        {
            // Check for the header and end of frame bytes
            if (buffer[i] == 0xAA && buffer[i + 1] == 0xFF)
            {
                // Extract the valid frame
                byte[] frame = new byte[DataLength];
                Array.Copy(buffer, i, frame, 0, DataLength);
                Console.WriteLine(BitConverter.ToString(frame));
                // Process the frame
                ProcessFrame(frame);

                // Skip to the next potential frame start
                i += DataLength - 1;
            }
        }        
    }

    private void ProcessFrame(byte[] frame)
    {
        // Extract target information
        short xCoordinate = BitConverter.ToInt16(frame, 4);
        short yCoordinate = BitConverter.ToInt16(frame, 6);

        // Check if presence is detected within a certain range
        IsPresenceDetected = CheckPresenceDetected(xCoordinate, yCoordinate, 1000); // Example range: 1000 mm

        

        Console.WriteLine($"Presence detected: {IsPresenceDetected}, X: {xCoordinate}, Y: {yCoordinate}");
    }

    private bool CheckPresenceDetected(short xDistance, short yDistance, int range)
    {
        // Check if the presence is within the specified range
        bool isDetected = (xDistance >= -range && xDistance <= range) && (yDistance >= -range && yDistance <= range);

        // Flip the current state
        currentState = !currentState;

        // Return the detection status
        return isDetected;
    }
}
