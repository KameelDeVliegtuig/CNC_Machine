# CNC machine met ingebouwde beveiliging

- 3-assige CNC machine
- Foutherkenning
- Rpi Zero W2

## Documentation

Pi's standard network is Houtenbos.

### Stepper directions

Stepper Direction truth table

| Direction| X | Y | Direction | Z |
| ------------- | ------------- | ----- | ------ | ----- | 
| Left | `NULL` | `NULL` | Up |  `True` |
| Right | `NULL` | `NULL` | Down | `False` |

### Raspberry commands

#### Update,build and run code

To speed up the test process a command is used to combine ``` git pull ``` , ``` dotnet build ``` and ``` dotnet ../-.ddl ```

```bash
./update
```

#### Emergency stop

The emergensy stop is used to immedialty disable spindle and steppers
> [!CAUTION]
> _**Usage may brick PWM signal until reboot!**_

```bash
./stop
```

### [PresenceDetector class](Code/CNC_Interpreter_V2/PresenceDetector.cs)

### [GPIOControl class](Code/CNC_Interpreter_V2/GPIOControl.cs)

```C#
public class GPIOControl
```

In order to use the Gpio pins on the Raspberry pi and MCP23017 multiple functions are used

#### Control functions

##### ControlSpindel

```C#
public boolean ControlSpindel(int speed, bool Dir);
```

This function is used to control the spindle speed and direction.

> [!IMPORTANT]
> Int speed is used as a precentage of the actual speed.

When speed is equal to 0 the spindle will perform a softstop to minimize load to the motor and controller.
When speed is less than 0 (-1), the spindle will perform an emergency stop. With this stop the spindle will stop immediatly. This has a negetive effect on the spindle.
returns true if completed

##### ControlStep _**Not finished**_

```C#
public bool ControlStep(bool dir, enum GPIOControl.StepperAxis);
```

This function controls the steppers. Usage pretty clear.
returns true if completed

##### Code defined emergency stop

```C#
public bool EmergencyStop(void);
```

Stops all active components

#### Read Functions

##### ReadPin

> [!NOTE]
> Only used for non declared pins

```C#
public bool ReadPin(int Pin, bool IsOnExtender);
```

Returns state of pin.

> [!WARNING]
> Some pins have the same number on the extender as well as the pi

##### ReadLimitSwitch

```C#
public bool ReadLimitSwitch(enum GPIOControl.LimitSwitch)
```

Returns value when limit switch is pressed.

- X -> NC
- Y -> NC
- Z -> NO

## Hardware

### Incomplete parts list

- Raspberry Pi Zero 2 W voor CNC besturing
- Stepper motor drivers (A4988)
- Custom PCB
  - MCP23017
  - Shitload JST Connectors
- Stukke Ender 3 V2
- 4 NEMA17
- HLK2450 Human Presence radar

## Useful links

[Default steps ender 3V2](https://www.reddit.com/r/ender3/comments/glbx8b/what_are_the_default_steps_per_mm_on_the_ender_3/)
