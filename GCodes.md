# G and M Codes

## G Codes

| Code    | Description                                         | Status                   |
|---------|-----------------------------------------------------|--------------------------|
| G0 / G1 | Linear Move                                         | $${\color{green}TESTED}$$|
| G2 / G3 | Arc or Circle Move                                  |$${\color{orange}NOT TESTED}$$|
| G4      | Dwell                                               |$${\color{orange}NOT TESTED}$$|
| G5      | Bézier cubic spline                                 |$${\color{orange}NOT TESTED}$$|
| G6      | Direct Stepper Move                                 |$${\color{orange}NOT TESTED}$$|
| G17     | CNC Workspace Plane XY                              |$${\color{orange}NOT TESTED}$$|
| G18     | CNC Workspace Plane ZX                              |$${\color{orange}NOT TESTED}$$|
| G19     | CNC Workspace Plane YZ                              |$${\color{orange}NOT TESTED}$$|
| G21     | Units mm                                            | $${\color{green}TESTED}$$|
| G27     | Park Toolhead                                       |$${\color{orange}NOT TESTED}$$|
| G28     | Auto Home                                           | $${\color{green}TESTED}$$|
| G42     | Move to mesh coordinate                             |$${\color{red}NOT IMPLEMENTED}$$|
| G53     | Move in Machine coordinates (fast move)             |$${\color{orange}NOT TESTED}$$|
| G54     | Workspace Coordinate System Slot 1                  |                          |
| G55     | Workspace Coordinate System Slot 2                  |                          |
| G56     | Workspace Coordinate System Slot 3                  |                          |
| G57     | Workspace Coordinate System Slot 4                  |                          |
| G58     | Workspace Coordinate System Slot 5                  |                          |
| G59     | Workspace Coordinate System Slot 6                  |                          |
| G59.1   | Workspace Coordinate System Slot 7                  |                          |
| G59.2   | Workspace Coordinate System Slot 8                  |                          |
| G59.3   | Workspace Coordinate System Slot 9                  |                          |
| G60     | Save Current position                               |                          |
| G61     | Return to saved position                            |                          |
| G91     | Relative Positioning (default)                      |$${\color{orange}NOT TESTED}$$|
| G92     | Set Position (Disable <0 values)                    |$${\color{orange}NOT TESTED}$$|

## M Codes

| Code    | Description                                         | Status                   |
|---------|-----------------------------------------------------|--------------------------|
| M0 / M1 | Unconditional STOP                                  |$${\color{orange}NOT TESTED}$$|
| M3      | Turn on Spindle CW                                  | $${\color{green}TESTED}$$|
| M4      | Turn on Spindle CCW                                 | $${\color{green}TESTED}$$|
| M5      | Turn off Spindle                                    | $${\color{green}TESTED}$$|
| M16     | Device checker (Always returns true, no printer check) | $${\color{green}TESTED}$$|
| M18 / M84 | Disable all steppers                              |$${\color{orange}NOT TESTED}$$|
| M20     | List Items SD card (Only when using SD Card)        |                          |
| M21     | Initialize SD card                                  |                          |
| M22     | Release SD card                                     |                          |
| M23     | Select file on SD                                   |                          |
| M24     | Start/Resume SD file                                |                          |
| M25     | Pause SD file                                       |                          |
| M27     | Report SD Execution Status                          |                          |
| M28 / M29 | Start/Stop Write to SD Card (May be discontinued) |                          |
| M30     | Delete file from SD                                 |                          |
| M31     | Print Time                                          |$${\color{orange}NOT TESTED}$$|
| M32     | Begin from SD File                                  |                          |
| M42     | Set pin state (Digital pins on extender)            |$${\color{orange}NOT TESTED}$$|
| M73     | Manually set progress (LCD)                         |                          |
| M75     | Start Print Job Timer (LCD)                         |                          |
| M76     | Pause Print Job Timer (LCD)                         |                          |
| M77     | Stop Print Job Timer (LCD)                          |                          |
| M100    | Free Memory                                         | $${\color{green}TESTED}$$|
| M105    | Temperature Report                                  |$${\color{orange}NOT TESTED}$$|
| M111    | Set debug level                                     |                          |
| M112    | Full Shutdown                                       | $${\color{green}TESTED}$$|
| M114    | Get Current Position                                |$${\color{orange}NOT TESTED}$$|
| M115    | Firmware Info                                       |$${\color{orange}NOT TESTED}$$|
| M117    | Set LCD Message                                     |$${\color{orange}NOT TESTED}$$|
| M119    | Endstop States                                      | $${\color{green}TESTED}$$|
| M149    | Set Temperature Units (C/F/K) (Fixed at C)          | $${\color{green}TESTED}$$|
| M150    | Set LED(strip) RGB(W) Color                         |                          |
| M201    | Set MAX Acceleration                                |                          |
| M205    | Advanced axis Settings                              |                          |
| M206    | Home Offset                                         |                          |
| M226    | Wait for pin state (0/1/-1)                         |$${\color{orange}NOT TESTED}$$|
| M256    | Set/Get LCD Brightness                              |                          |
| M300    | Play Tone                                           |                          |
| M305    | Set MicroStepping (Fixed at 1/16)                   |$${\color{orange}NOT TESTED}$$|
| M400    | Wait for moves to finish                            |$${\color{orange}NOT TESTED}$$|
| M401    | Deploy Bed Probe                                    |$${\color{orange}NOT TESTED}$$|
| M402    | Stow Bed Probe                                      |$${\color{orange}NOT TESTED}$$|
| M410    | Stop all stepper instantly                          |$${\color{orange}NOT TESTED}$$|
| M420    | Get/Set Bed Leveling State                          |                          |
| M428    | Set Home at Current Position                        |$${\color{orange}NOT TESTED}$$|
| M510    | Lock Machine                                        |$${\color{orange}NOT TESTED}$$|
| M511    | Unlock Machine                                      |$${\color{orange}NOT TESTED}$$|
| M512    | Set Passcode                                        |$${\color{orange}NOT TESTED}$$|
| M524    | Abort Print in Progress                             |                          |
| M575    | Change Serial BaudRate                              |                          |
| M928    | Start Logging to SD                                 |                          |
| M995    | Calibrate Touchscreen                               |                          |

## Custom Codes

| Code | Description                      | Status                   |
|------|----------------------------------|--------------------------|
| X0   | Selective Stop                   |$${\color{orange}NOT TESTED}$$|
