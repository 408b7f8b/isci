#!/bin/bash

# Add environment variables to bashrc
echo 'export ISCI_Ressource=$HOSTNAME' >> ~/.bashrc
echo 'export ISCI_Anwendungen=/opt/isci' >> ~/.bashrc
echo 'export ISCI_Datenstrukturen=/var/isci' >> ~/.bashrc

# Update the current shell session
source ~/.bashrc
