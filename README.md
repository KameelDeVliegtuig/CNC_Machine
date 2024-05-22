# CNC machine met ingebouwde beveiliging

- 3-assige CNC machine
- Foutherkenning
- Rpi Zero W2

## Documentation

Pi's standard network is Houtenbos.

### Steppermotors, Spindle, Buttons & Switches

#### Stepper Steps per mm

| Axis | Steps | mm |
| ---- | ----- | -- |
| X | 0 | 0 |
| Y | 0 | 0 |
| Z | 400 | 1 |

#### Directions

| Direction| X | Y | Direction | Z | Direction | Spindle |
| -------- | - | - | --------- | - | --------- | ------- |
| Left | `NULL` | `NULL` | Up |  `true` | Clockwise *Right* | `true` |
| Right | `NULL` | `NULL` | Down | `false` | Counter clockwise *Left* | `false` |

> [!CAUTION]
> **Only change direction of motors when idle or at low RPM!**

#### Stepper Spindle-offset

| Axis | Offset (mm) |
| ---- | ----------- |
| X | 0 |
| Y | 0 |
| Z | 0 |

#### Contact type buttons & switches

| Switch/Button | Contact type | Usage |
| ------------- | ------------ | ----- |
| Limit switch X | NC | Used for zero-ing spindle's X-axis |
| Limit switch Y | NC | Used for zero-ing spindle's Y-axis |
| Limit switch Z | NO | Used for zero-ing spindle's Z-axis, is connected via magnets |
| E-Stop | NO | Used to stop all moving parts |
| Emergency button| NC | Takes all power off machine |

>[!NOTE]
> NC (Normally Closed)
> NO (Normally Open)

### Raspberry commands

| Command   | Description |
| --------- | ----------- |
| `./update` | To speed up the test process a command is used to combine ``` git pull ``` , ``` dotnet build ``` and ``` dotnet ../-.ddl ``` |
| `./stop` | The emergensy stop is used to immedialty disable spindle and steppers |
| `./run`| Runs the .ddl file without compiling |

> [!CAUTION]
> **Usage of `./stop` may brick PWM signal until reboot!**

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

##### ControlStep

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
