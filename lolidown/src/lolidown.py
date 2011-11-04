#!/usr/bin/env python
from BeautifulSoup import BeautifulSoup
from optparse import OptionParser
import urllib,urllib2
import sys
import os
import cookielib
import time
import shutil

loadkontext = {}
#Progressbar Class from aspn cookbook,license unkown
class progressBar:
    def __init__(self, minValue = 0, maxValue = 10, totalWidth=12):
        self.progBar = "[]"   # This holds the progress bar string
        self.min = minValue
        self.max = maxValue
        self.span = maxValue - minValue
        self.width = totalWidth
        self.amount = 0       # When amount == max, we are 100% done 
        self.updateAmount(0)  # Build progress bar string

    def updateAmount(self, newAmount = 0):
        if newAmount < self.min: newAmount = self.min
        if newAmount > self.max: newAmount = self.max
        self.amount = newAmount

        # Figure out the new percent done, round to an integer
        diffFromMin = float(self.amount - self.min)
        percentDone = (diffFromMin / float(self.span)) * 100.0
        percentDone = round(percentDone)
        percentDone = int(percentDone)

        # Figure out how many hash bars the percentage should be
        allFull = self.width - 2
        numHashes = (percentDone / 100.0) * allFull
        numHashes = int(round(numHashes))

        # build a progress bar with hashes and spaces
        self.progBar = "[" + '#'*numHashes + ' '*(allFull-numHashes) + "]"

        # figure out where to put the percentage, roughly centered
        percentPlace = (len(self.progBar) / 2) - len(str(percentDone)) 
        percentString = str(percentDone) + "%"

        # slice the percentage into the bar
        self.progBar = self.progBar[0:percentPlace] + percentString + self.progBar[percentPlace+len(percentString):]

    def __str__(self):
        return str(self.progBar)

class job(object):
    def __init__(self,url,numeps):
        self.url = url
        self.numeps = int(numeps)
        self.curep = 0
        self.curerr = ""
        self.error = False
        self.curfilename = ""
        self.numepdown = 0
        self.prog = ""
    

class loli(object):
    def __init__(self,config):
        self.config = config
        self.curjob = None
        self.jobs = []
        #self.parser = BeautifulSoup()
    
    def add_job(self,url,numeps):
        self.jobs.append(job(url,numeps))
    
    def myReportHook(self,count, blockSize, totalSize):
        if self.curjob.prog == "":
            self.curjob.prog = progressBar(0,totalSize,50)
        #print count, blockSize, totalSize
        #prog = progressBar(0, totalSize, 77)
        self.curjob.prog.updateAmount(count*blockSize)
        status = "%d kb of %d kb downloaded" % (count * (blockSize/1024) , (totalSize/1024) )
        sys.stdout.write('\033]2;'+status+'\007')
        sys.stdout.write ("\r")
        sys.stdout.write (str(self.curjob.prog))
        sys.stdout.write ("\r")
    
    def download_file(self,url):       
        self.curjob.error = False
        print ""
        print "Downloading file: " , self.curjob.curfilename
        try:
            res = urllib.urlretrieve(url,reporthook=self.myReportHook)
        except IOError:
            print "Error downloading file"
            print "sleeping %d seconds before retry" %(self.config.error_sleep)
            self.curjob.prog = ""
            self.curjob.error = True
            return
        
        if not "Content-Disposition" in res[1]:
            self.curjob.error = True
            self.curjob.prog = ""
            return
        
        dis = res[1]["Content-Disposition"]
        move_loc = os.path.join(self.config.download_dir , dis[dis.find('"')+1:dis.rfind('"')])  
        shutil.move(res[0].strip(), move_loc.strip())
        self.curjob.curep +=1
        self.curjob.numepdown +=1
        self.curjob.prog = ""       
    
    def get_download_url(self):
        self.dprint ("retrieving Listing From Url: " + self.curjob.url)
        print ""
        cookie = cookielib.CookieJar()
        opener = urllib2.build_opener(urllib2.HTTPCookieProcessor(cookie))
        urllib2.install_opener(opener)
        urllib2.urlopen("http://www.lolipower.org/");
        html = urllib2.urlopen(self.curjob.url)
        p = BeautifulSoup(html.read())
        data_list = p.findAll('div', "filedata")
        self.curjob.curfilename = data_list[self.curjob.curep].find("span","filename").contents[0]
        link = data_list[self.curjob.curep].find('a', href=True)['href']
        self.dprint("extracted values:")
        self.dprint( "filename: " + self.curjob.curfilename )
        self.dprint( "downloadLink: " + link)
        return link
        
    def process_job(self):
        while self.curjob.curep <= self.curjob.numeps -1:
            download_url = self.get_download_url()
            self.download_file(download_url)
            self.print_status()
            if self.curjob.error:
                time.sleep(self.config.error_sleep)
            
        
        del self.jobs[0]
        self.curjob = None
    
    def print_status(self):
        print ""
        print "***Status***"
        if self.curjob.error: print "error occoured while downloading " , self.curjob.curfilename 
        print "downloaded %d of %d eps" % (self.curjob.numepdown , self.curjob.numeps)
        
    
    def dprint(self, message):
        if self.config.debug :
            print message
            
def load_config():
    conf = open(os.path.expanduser('~/.loliconf'),'r')
    exec conf.read() in loadkontext
    conf.close()

def create_config():
    print "you seem to use lolidown for the first time, please setup your profile now.."
    print ''
    downdir = raw_input("what should the download directory be?(it must exists):")
    if not os.path.exists(os.path.expanduser(downdir)):
        print 'dir does not exist..exiting'
        exit(1)
    print ''
    debug = raw_input("Should debug mode be enabled?(True/False):")
    print ''
    errorwait = raw_input("How many seconds should we wait when we encounter a download error?:")
    if os.path.exists(os.path.expanduser('~/.loliconf')):
        os.remove(os.path.expanduser('~/.loliconf'))
    conf = open(os.path.expanduser('~/.loliconf'), 'w')
    conf.write("class config(object):\n")
    conf.write("\t def __init__(self):\n")
    conf.write("\t\tself.download_dir = '"+downdir+"'\n")
    conf.write("\t\tself.debug ="+ debug+"\n")
    conf.write("\t\tself.error_sleep ="+errorwait+"\n")
    conf.flush()
    conf.close()
    print 'Please restart lolidown'
    
        
def main():
    usage = "%prog -l/--listing URL -n/--numep NUMBER"
    parser = OptionParser(usage)
    parser.add_option("-l","--listing",type="string",dest="listing",help="use URL as the listing url",metavar="URL")
    parser.add_option("-n","--numep",type="int",dest="numep",help="download all eps up to NUMBER",metavar="NUMBER")
    parser.add_option("-f","--from",type="int",dest="start",help="start from ep NUMBER",metavar="NUMBER")
    (optionen,args) = parser.parse_args()
    if len(sys.argv) < 3 :
        print parser.get_usage()
        exit(1)
    url = optionen.listing
    numeps = optionen.numep
    if not os.path.exists(os.path.expanduser('~/.loliconf')):
        create_config()
        exit(1)
    else:
        load_config()
    mylolidown = loli(loadkontext['config']())
    mylolidown.add_job(url, numeps)
    mylolidown.curjob = mylolidown.jobs[0]
    if optionen.start:
        mylolidown.curjob.curep = optionen.start -1
    mylolidown.process_job()
    
if __name__ == '__main__':
    main()
    
    
