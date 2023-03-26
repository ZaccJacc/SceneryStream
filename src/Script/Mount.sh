#!/bin/sh
sudo mkdir ~/mnt/$2
sudo mount -t smbfs //$1 ~/mnt/$2