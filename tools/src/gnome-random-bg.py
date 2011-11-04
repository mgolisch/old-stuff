import os
import random
import time

def set_bg_gnome(file,mode="fill"):
    """sets the Wallpaper to file with Options set to mode.
        valid values for mode are centered,fill,stretch,zoom,none
    """
    os.system("gconftool -t string -s /desktop/gnome/background/picture_options " + mode)
    os.system("gconftool -t string -s /desktop/gnome/background/picture_filename " + file)

def combine_path(path1,path2):
    return os.path.join(path1,path2)

while(True):
    wallpaper_path = "/home/michi/Bilder/wallpapers/"
    wallpaper = os.listdir(wallpaper_path)
    new_background = combine_path(wallpaper_path, wallpaper[random.randrange(0,len(wallpaper))])
    set_bg_gnome(new_background)
    time.sleep(1800)
