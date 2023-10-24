#!/bin/bash

# Add environment variables to bashrc
echo 'export AUTOMATISIERUNG_RESSOURCE=$HOSTNAME' >> ~/.bashrc
echo 'export AUTOMATISIERUNG_ANWENDUNG=Entwicklungsanwendung' >> ~/.bashrc
echo 'export AUTOMATISIERUNG_ANWENDUNGEN=/home/pi/isci/Development/Anwendungen' >> ~/.bashrc
echo 'export AUTOMATISIERUNG_DATENSTRUKTUREN=/media/ramdisk' >> ~/.bashrc

# Update the current shell session
source ~/.bashrc
