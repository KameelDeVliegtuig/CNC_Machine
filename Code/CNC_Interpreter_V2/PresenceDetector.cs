using System;
using System.IO.Ports;
using System.Numerics;

public class PresenceDetector
{
    private const int DataLength = 30; // Length of the data frame
    private SerialPort serialPort;
    private byte[] buffer = new byte[DataLength * 2]; // Buffer for received data
    private int bufferIndex = 0; // Current index in the buffer
    private bool currentState;
    private Thread listeningThread;
    public bool IsDetected { get; private set; }

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
            while (true) { } // Keep the program running (listening for data)
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
