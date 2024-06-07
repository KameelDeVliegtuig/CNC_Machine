# G and M Codes

## G Codes

| Code    | Description                                         | Status                   |
|---------|-----------------------------------------------------|--------------------------|
| G0 / G1 | Linear Move                                         | $${\color{green}STATE}$$ |
| G2 / G3 | Arc or Circle Move                                  | $${\color{orange}STATE}$$|
| G4      | Dwell                                               | $${\color{orange}STATE}$$|
| G5      | Bézier cubic spline                                 | $${\color{orange}STATE}$$|
| G6      | Direct Stepper Move                                 | $${\color{orange}STATE}$$|
| G17     | CNC Workspace Plane XY                              | $${\color{orange}STATE}$$|
| G18     | CNC Workspace Plane ZX                              | $${\color{orange}STATE}$$|
| G19     | CNC Workspace Plane YZ                              | $${\color{orange}STATE}$$|
| G21     | Units mm                                            | $${\color{green}STATE}$$ |
| G27     | Park Toolhead                                       | $${\color{orange}STATE}$$|
| G28     | Auto Home                                           | $${\color{green}STATE}$$ |
| G42     | Move to mesh coordinate                             |   $${\color{red}STATE}$$ |
| G53     | Move in Machine coordinates (fast move)             | $${\color{orange}STATE}$$|
| G54     | Workspace Coordinate System Slot 1                  |   $${\color{red}STATE}$$ |
| G55     | Workspace Coordinate System Slot 2                  |   $${\color{red}STATE}$$ |
| G56     | Workspace Coordinate System Slot 3                  |   $${\color{red}STATE}$$ |
| G57     | Workspace Coordinate System Slot 4                  |   $${\color{red}STATE}$$ |
| G58     | Workspace Coordinate System Slot 5                  |   $${\color{red}STATE}$$ |
| G59     | Workspace Coordinate System Slot 6                  |   $${\color{red}STATE}$$ |
| G59.1   | Workspace Coordinate System Slot 7                  |   $${\color{red}STATE}$$ |
| G59.2   | Workspace Coordinate System Slot 8                  |   $${\color{red}STATE}$$ |
| G59.3   | Workspace Coordinate System Slot 9                  |   $${\color{red}STATE}$$ |
| G60     | Save Current position                               |   $${\color{red}STATE}$$ |
| G61     | Return to saved position                            |   $${\color{red}STATE}$$ |
| G91     | Relative Positioning (default)                      |$${\color{orange}STATE}$$|
| G92     | Set Position (Disable <0 values)                    |$${\color{orange}STATE}$$|
--------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------
| Code    | Description                                         | Status                   |
|---------|-----------------------------------------------------|--------------------------|
| M0 / M1 | Unconditional STOP                                  |$${\color{orange}STATE}$$|
| M3      | Turn on Spindle CW                                  | $${\color{green}STATE}$$|
| M4      | Turn on Spindle CCW                                 | $${\color{green}STATE}$$|
| M5      | Turn off Spindle                                    | $${\color{green}STATE}$$|
| M16     | Device checker                                      | $${\color{green}STATE}$$|
| M18 / M84 | Disable all steppers                              |$${\color{orange}STATE}$$|
| M20     | List Items                                          |   $${\color{red}STATE}$$ |
| M23     | Select file                                         |   $${\color{red}STATE}$$ |
| M24     | Start/Resume                                        |   $${\color{red}STATE}$$ |
| M25     | Pause                                               |   $${\color{red}STATE}$$ |
| M27     | Report Execution Status                             |   $${\color{red}STATE}$$ |
| M28 / M29 | Start/Stop Write to SD Card                       |   $${\color{red}STATE}$$ |
| M30     | Delete file from SD                                 |   $${\color{red}STATE}$$ |
| M31     | Print Time                                          |$${\color{orange}STATE}$$|
| M32     | Begin from SD File                                  |   $${\color{red}STATE}$$ |
| M42     | Set pin state (Digital pins on extender)            |$${\color{orange}STATE}$$|
| M73     | Manually set progress (LCD)                         |   $${\color{red}STATE}$$ |
| M75     | Start Print Job Timer (LCD)                         |   $${\color{red}STATE}$$ |
| M76     | Pause Print Job Timer (LCD)                         |   $${\color{red}STATE}$$ |
| M77     | Stop Print Job Timer (LCD)                          |   $${\color{red}STATE}$$ |
| M100    | Free Memory                                         | $${\color{green}STATE}$$|
| M105    | Temperature Report                                  |$${\color{orange}STATE}$$|
| M111    | Set debug level                                     |   $${\color{red}STATE}$$ |
| M112    | Full Shutdown                                       | $${\color{green}STATE}$$|
| M114    | Get Current Position                                |$${\color{orange}STATE}$$|
| M115    | Firmware Info                                       |$${\color{orange}STATE}$$|
| M117    | Set LCD Message                                     |$${\color{orange}STATE}$$|
| M119    | Endstop States                                      | $${\color{green}STATE}$$|
| M149    | Set Temperature Units (C/F/K) (Fixed at C)          | $${\color{green}STATE}$$|
| M150    | Set LED(strip) RGB(W) Color                         |   $${\color{red}STATE}$$ |
| M201    | Set MAX Acceleration                                |   $${\color{red}STATE}$$ |
| M205    | Advanced axis Settings                              |   $${\color{red}STATE}$$ |
| M206    | Home Offset                                         |   $${\color{red}STATE}$$ |
| M226    | Wait for pin state (0/1/-1)                         |$${\color{orange}STATE}$$|
| M256    | Set/Get LCD Brightness                              |   $${\color{red}STATE}$$ |
| M300    | Play Tone                                           |   $${\color{red}STATE}$$ |
| M305    | Set MicroStepping (Fixed at 1/16)                   |$${\color{orange}STATE}$$|
| M400    | Wait for moves to finish                            |$${\color{orange}STATE}$$|
| M401    | Deploy Bed Probe                                    |$${\color{orange}STATE}$$|
| M402    | Stow Bed Probe                                      |$${\color{orange}STATE}$$|
| M410    | Stop all stepper instantly                          |$${\color{orange}STATE}$$|
| M420    | Get/Set Bed Leveling State                          |   $${\color{red}STATE}$$ |
| M428    | Set Home at Current Position                        |$${\color{orange}STATE}$$|
| M510    | Lock Machine                                        |$${\color{orange}STATE}$$|
| M511    | Unlock Machine                                      |$${\color{orange}STATE}$$|
| M512    | Set Passcode                                        |$${\color{orange}STATE}$$|
| M524    | Abort Print in Progress                             |   $${\color{red}STATE}$$ |
| M575    | Change Serial BaudRate                              |   $${\color{red}STATE}$$ |
| M928    | Start Logging to SD                                 |   $${\color{red}STATE}$$ |
| M995    | Calibrate Touchscreen                               |   $${\color{red}STATE}$$ |

## Custom Codes

| Code | Description                      | Status                   |
|------|----------------------------------|--------------------------|
| X0   | Selective Stop                   |$${\color{orange}STATE}$$|
