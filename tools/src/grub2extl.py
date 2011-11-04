#!/usr/bin/env python
#coding=utf-8
def get_value(line):
    return line.split("\t")[2].strip()

def split_kernel(line):
    kern_opts = []
    first_space = line.find(" ")
    kern_opts.append(line[0:first_space])
    kern_opts.append(line[first_space+1:])
    return kern_opts

def write_config():
    extlinux = open("/boot/extlinux.conf","w")
    label_prefix = "ununtu"
    extlinux.write("DEFAULT menu.c32\n")
    extlinux.write("TIMEOUT 100\n")
    extlinux.write("PROMPT 0\n")
    extlinux.write("\n")
    for i in range(len(entries["Initrd"])):
        extlinux.write("LABEL "+label_prefix+str(i)+"\n")
        extlinux.write("MENU LABEL "+entries["Title"][i]+"\n")
        kernel = split_kernel(entries["Kernel"][i])[0]
        append = split_kernel(entries["Kernel"][i])[1]
        initrd = entries["Initrd"][i]
        append = append + " initrd="+initrd
        extlinux.write("KERNEL "+kernel+"\n")
        extlinux.write("APPEND " +append+"\n")
        extlinux.write("\n")
        extlinux.flush()
    extlinux.close()

entries = {"Title":[],"Kernel":[],"Initrd":[]}
file = open('/boot/grub/menu.lst','r')
for line in file.readlines():
    if line.startswith('#'): 
        continue
    if line.startswith('title'):
        entries["Title"].append(get_value(line))
    if line.startswith("kernel"):
        entries["Kernel"].append(get_value(line))
    if line.startswith("initrd"):
        entries["Initrd"].append(get_value(line))
file.close()
write_config()
