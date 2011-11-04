#cd $HOME/encode
mkfifo stream.y4m
mkfifo stream.wav 
FILENAME=$(basename "$1")
OUTPURDIR="/data/xbox_encodes/"
#Extract FPS to guarantie syncronized playback
FPS=$(mplayer -identify -frames 0 -vc null -vo null -ao null "$1"|grep FPS|cut -f2 -d =)
#Pipe audio stream to pipe for further processing by encoder(mencoder,ffmpeg)
mplayer -vc null -vo null -ao pcm:file=stream.wav "$1" -quiet -benchmark </dev/null &
#Pipe video stream with onscreen subtitle drawing to encoder
mplayer -sid 0 "$1" -vo yuv4mpeg:file=stream.y4m -nosound  -noframedrop -quiet -benchmark  </dev/null &
#wait for processes to start(guarantie availibility of pipes before starting encoder
sleep 10
#starting encoder
#ffmpeg -f yuv4mpegpipe -i stream.y4m -i stream.wav -acodec libfaac -ac 2 -ab 64k -ar 44100 -vcodec libx264 -r "$FPS" -vpre hq -threads 0 -f mp4 "$2"
ffmpeg -f yuv4mpegpipe -i stream.y4m -i stream.wav -acodec libfaac -ac 2 -ab 64k -ar 44100 -vcodec libx264 -r "$FPS" -vpre default -b 1024k -coder 0 -level 30 -threads 0 -f mp4 "$OUTPUTDIR$FILENAME.mp4"
#deleting pipes
rm stream.y4m
rm stream.wav
