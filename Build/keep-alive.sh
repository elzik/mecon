#!/bin/bash

#https://serverfault.com/a/972760

while :;           # Run an endless loop,
do :;              # of do nothing,
done &             # as background task.
kill -STOP $!      # Stop the background task.
wait $!            # Wait forever, because background task process has been stopped.