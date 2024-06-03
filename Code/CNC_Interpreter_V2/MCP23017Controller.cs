using System;
using System.Device.Gpio;
using System.Device.I2c;
using Iot.Device.Mcp23xxx;

public class MCP23017Controller : Mcp23017
{
    public MCP23017Controller(I2cDevice i2cDevice) : base(i2cDevice)
    {
    }

    public void SetPinDirection(int pin, bool isOutput)
    {
        SetPinMode(pin, isOutput ? PinMode.Output : PinMode.Input);
    }

    public void WritePin(int pin, bool value)
    {
        Write(pin, value);
    }

    public bool ReadPin(int pin)
    {   
        return Read(pin) == PinValue.High;
    }
}
