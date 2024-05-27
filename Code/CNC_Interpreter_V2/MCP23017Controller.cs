using System;
using System.Device.I2c;
using System.Device.Gpio;
using Iot.Device.Mcp23xxx;

namespace MCPController
{
    public class MCP23017Controller : IDisposable
    {
        private const byte IOCON = 0x05;
        private const byte IODIRA = 0x00;
        private const byte IODIRB = 0x10;
        private const byte GPIOA = 0x09;
        private const byte GPIOB = 0x19;
        private const byte OLATA = 0x0A;
        private const byte OLATB = 0x1A;

        private I2cDevice _i2cDevice;

        public MCP23017Controller(int i2cBusId = 1, int deviceAddress = 0x20)
        {
            var i2cConnectionSettings = new I2cConnectionSettings(i2cBusId, deviceAddress);
            _i2cDevice = I2cDevice.Create(i2cConnectionSettings);
            WriteRegister(IOCON, 0b10000000); // Set BANK bit to 1

        }

        public void SetPinDirection(int pin, bool isOutput)
        {
            if (pin < 0 || pin > 15)
            {
                throw new ArgumentOutOfRangeException(nameof(pin), "Pin number must be between 0 and 15.");
            }

            byte iodirReg = (byte)(pin < 8 ? IODIRA : IODIRB);
            int pinIndex = pin % 8;
            byte currentDirection = ReadRegister(iodirReg);

            if (isOutput)
            {
                currentDirection &= (byte)~(1 << pinIndex);
            }
            else
            {
                currentDirection |= (byte)(1 << pinIndex);
            }

            WriteRegister(iodirReg, currentDirection);
        }

        public void WritePin(int pin, bool value)
        {
            if (pin < 0 || pin > 15)
            {
                throw new ArgumentOutOfRangeException(nameof(pin), "Pin number must be between 0 and 15.");
            }

            byte gpioReg = (byte)(pin < 8 ? OLATA : OLATB);
            int pinIndex = pin % 8;
            byte currentOutput = ReadRegister(gpioReg);

            if (value)
            {
                currentOutput |= (byte)(1 << pinIndex);
            }
            else
            {
                currentOutput &= (byte)~(1 << pinIndex);
            }

            WriteRegister(gpioReg, currentOutput);
        }

        public bool ReadPin(int pin)
        {
            if (pin < 0 || pin > 15)
            {
                throw new ArgumentOutOfRangeException(nameof(pin), "Pin number must be between 0 and 15.");
            }

            byte gpioReg = (byte)(pin < 8 ? GPIOA : GPIOB);
            int pinIndex = pin % 8;
            byte pinState = ReadRegister(gpioReg);

            return (pinState & (1 << pinIndex)) != 0;
        }

        private byte ReadRegister(byte register)
        {
            _i2cDevice.WriteByte(register);
            return _i2cDevice.ReadByte();
        }

        private void WriteRegister(byte register, byte data)
        {
            _i2cDevice.Write(new byte[] { register, data });
        }

        public void Dispose()
        {
            _i2cDevice?.Dispose();
        }
    }
}