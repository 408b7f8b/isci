#!/bin/bash

# Define the mount point and size for the tmpfs filesystem
mount_point="/media/ramdisk"
size="100M"

# Create the mount point directory if it doesn't exist
if [ ! -d "$mount_point" ]; then
    sudo mkdir "$mount_point"
fi

# Add the tmpfs entry to /etc/fstab
echo "tmpfs $mount_point tmpfs defaults,size=$size 0 0" | sudo tee -a /etc/fstab

# Mount the tmpfs filesystem
sudo mount -a

# Verify the mount
mount | grep "$mount_point"
