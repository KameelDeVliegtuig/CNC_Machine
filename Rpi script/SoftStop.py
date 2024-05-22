#!/usr/bin/env python2.7  
# https://raspi.tv/2013/how-to-use-interrupts-with-python-on-the-raspberry-pi-and-rpi-gpio  
import RPi.GPIO as GPIO  
import os as os
GPIO.setmode(GPIO.BCM)  
# GPIO 23 set up as input. It is pulled up to stop false signals  
GPIO.setup(20, GPIO.IN)  
# now the program will do nothing until the signal on port 23   
try:  
    GPIO.wait_for_edge(23, GPIO.FALLING)  
    print "Stopping the system"
    os.system('cd')
    os.system('./stop')


except KeyboardInterrupt:  
GPIO.cleanup()       
GPIO.cleanup

"""
The above code is a python script that will be run on the Raspberry Pi. This script listens for a signal on GPIO pin 20. When a signal is detected, the script will run a bash script that will stop the system. The bash script is as follows:
The stop script will stop the system when run. The system will then need to be manually powered back on in order to restart it.
"""