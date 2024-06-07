# G and M Codes

## G Codes

| Code    | Description                                         | Status                   |
|---------|-----------------------------------------------------|--------------------------|
| G0 / G1 | Linear Move                                         |<code style="color : Green">TESTED</code>|
| G2 / G3 | Arc or Circle Move                                  |                          |
| G4      | Dwell                                               |                          |
| G5      | Bézier cubic spline                                 |                          |
| G6      | Direct Stepper Move                                 |                          |
| G17     | CNC Workspace Plane XY                              |                          |
| G18     | CNC Workspace Plane ZX                              |                          |
| G19     | CNC Workspace Plane YZ                              |                          |
| G21     | Units mm                                            |                          |
| G27     | Park Toolhead                                       |                          |
| G28     | Auto Home                                           |                          |
| G42     | Move to mesh coordinate                             |                          |
| G53     | Move in Machine coordinates (fast move)             |                          |
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
| G91     | Relative Positioning (default)                      |                          |
| G92     | Set Position (Disable <0 values)                    |                          |

## M Codes

| Code    | Description                                         | Status                   |
|---------|-----------------------------------------------------|--------------------------|
| M0 / M1 | Unconditional STOP                                  |                          |
| M3      | Turn on Spindle CW                                  |                          |
| M4      | Turn on Spindle CCW                                 |                          |
| M5      | Turn off Spindle                                    |                          |
| M16     | Device checker (Always returns true, no printer check) |                          |
| M18 / M84 | Disable all steppers                              |                          |
| M20     | List Items SD card (Only when using SD Card)        |                          |
| M21     | Initialize SD card                                  |                          |
| M22     | Release SD card                                     |                          |
| M23     | Select file on SD                                   |                          |
| M24     | Start/Resume SD file                                |                          |
| M25     | Pause SD file                                       |                          |
| M27     | Report SD Execution Status                          |                          |
| M28 / M29 | Start/Stop Write to SD Card (May be discontinued) |                          |
| M30     | Delete file from SD                                 |                          |
| M31     | Print Time                                          |                          |
| M32     | Begin from SD File                                  |                          |
| M42     | Set pin state (Digital pins on extender)            |                          |
| M73     | Manually set progress (LCD)                         |                          |
| M75     | Start Print Job Timer (LCD)                         |                          |
| M76     | Pause Print Job Timer (LCD)                         |                          |
| M77     | Stop Print Job Timer (LCD)                          |                          |
| M100    | Free Memory                                         |                          |
| M105    | Temperature Report                                  |                          |
| M111    | Set debug level                                     |                          |
| M112    | Full Shutdown                                       |                          |
| M114    | Get Current Position                                |                          |
| M115    | Firmware Info                                       |                          |
| M117    | Set LCD Message                                     |                          |
| M119    | Endstop States                                      |                          |
| M149    | Set Temperature Units (C/F/K) (Fixed at C)          |                          |
| M150    | Set LED(strip) RGB(W) Color                         |                          |
| M201    | Set MAX Acceleration                                |                          |
| M205    | Advanced axis Settings                              |                          |
| M206    | Home Offset                                         |                          |
| M226    | Wait for pin state (0/1/-1)                         |                          |
| M256    | Set/Get LCD Brightness                              |                          |
| M300    | Play Tone                                           |                          |
| M305    | Set MicroStepping (Fixed at 1/16)                   |                          |
| M400    | Wait for moves to finish                            |                          |
| M401    | Deploy Bed Probe                                    |                          |
| M402    | Stow Bed Probe                                      |                          |
| M410    | Stop all stepper instantly                          |                          |
| M420    | Get/Set Bed Leveling State                          |                          |
| M428    | Set Home at Current Position                        |                          |
| M510    | Lock Machine                                        |                          |
| M511    | Unlock Machine                                      |                          |
| M512    | Set Passcode                                        |                          |
| M524    | Abort Print in Progress                             |                          |
| M575    | Change Serial BaudRate                              |                          |
| M928    | Start Logging to SD                                 |                          |
| M995    | Calibrate Touchscreen                               |                          |

## Custom Codes

| Code | Description                      | Status                   |
|------|----------------------------------|--------------------------|
| X0   | Selective Stop                   |                          |
