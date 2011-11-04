#!/bin/bash
COUNT=0
CMD_ZENITY='zenity --list --column=ID --column=Auflösung'
CMD_XRANDR="xrandr -s "
for line in $(xrandr|tail -n+3|awk {'print $1'})
do
RES[$COUNT]="$line"
COUNT=$((COUNT+1))
done
for i in $(seq 0 "${#RES[@]}")
do
CMD_ZENITY="$CMD_ZENITY $i ${RES[$i]}"
done
RES=$($CMD_ZENITY)
$CMD_XRANDR "$RES"
