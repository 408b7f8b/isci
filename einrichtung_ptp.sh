#!/bin/bash

# Install required packages
sudo apt-get update
sudo apt-get install -y ptpd

# Configure PTP
sudo systemctl stop ptpd.service
sudo nano /etc/ptpd.conf

# /etc/ptpd.conf
global:PTP_DOMAIN_NUMBER=0
global:PTP_DELAY_MECHANISM=E2E

interface:eth0
