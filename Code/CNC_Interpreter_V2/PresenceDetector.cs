using System;
using System.IO.Ports;

public class PresenceDetector
{
    private const int DataLength = 24; // Length of the data frame
    private SerialPort serialPort;
    private bool currentState;

    public bool IsPresenceDetected { get; private set; }

    public PresenceDetector(string portName, int baudRate)
    {
        serialPort = new SerialPort(portName, baudRate);
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

        // Check if enough bytes are available in the input buffer
        if (serialPort.BytesToRead < DataLength)
        {
            return;
        }

        // Read the data into the buffer
        byte[] buffer = new byte[DataLength];
        serialPort.Read(buffer, 0, DataLength);

        // Check if the received data matches the expected format
        if (buffer[0] == 0xAA && buffer[1] == 0xFF && buffer[2] == 0x03 && buffer[3] == 0x00 && buffer[DataLength - 2] == 0x55 && buffer[DataLength - 1] == 0xCC)
        {
            // Extract target information
            short xCoordinate = BitConverter.ToInt16(buffer, 4);
            short yCoordinate = BitConverter.ToInt16(buffer, 6);

            // Check if presence is detected within a certain range
            IsPresenceDetected = CheckPresenceDetected(xCoordinate, yCoordinate, 1000); // Example range: 1000 mm

            Console.WriteLine($"Presence detected: {IsPresenceDetected}, X: {xCoordinate}, Y: {yCoordinate}");
        }
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

