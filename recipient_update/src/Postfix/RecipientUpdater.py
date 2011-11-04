#!/usr/bin/env python
import sys,os,shutil,smtplib
import email.MIMEText
import subprocess

class RecipientUpdater(object):
    def __init__(self):
        self.basedir = "/etc/postfix/"
        self.logpath = "/tmp/recipient_update.log"
        self.old_recipients_file = "/etc/postfix/virtual.txt"
        self.new_recipients_file = "/etc/postfix/virtual_new.txt"
        try:
            self.logfile = open(self.logpath,"w+")
        except:
            print "Failed opening Logfile"
        #redirect stdout to logfile
        self._stdout = sys.stdout
        sys.stdout = self.logfile
        
    def move_map(self):
        print "Moving Map"
        print "..."
        try:
            shutil.move(self.new_recipients_file, self.old_recipients_file)
        except:
            print "Failed moving map"
            print ""
    
    def make_map(self):
        print "Executing Makefile to generate recipient maps in postfix format"
        print ""
        print "..."
        try:
            os.remove("/etc/postfix/relay_recipients.proto")
        except:pass
        process = subprocess.Popen("sh -c cd /etc/postfix/;make all",shell=True,stdin=subprocess.PIPE)
        retcode = process.wait()
        if retcode != 0:
            print "Something went wrong while executing make subprocess"
            print "returncode = " + retcode
        else: 
            print "Make Successfull"
            self.apply()
        print ""
    
    def apply(self):
        print "Reloading Postfix configuration"
        print ""
        print "..."
        process = subprocess.Popen("postfix reload",shell=True,stdin=subprocess.PIPE)
        retcode = process.wait()
        if retcode != 0:
            print "Something went wrong while reloading postfix"
            print "returncode = " + retcode
        else: print "Reload successful"
        
    def compare_maps(self):
        print "Comparing Recipient list changes:"
        old_recipients = open(self.old_recipients_file).readlines()
        new_recipients = open(self.new_recipients_file).readlines()
        change = False
        for old_recipient in old_recipients:
            if not old_recipient in new_recipients:
                print "removed Recipient: " + old_recipient
                change = True
        for new_recipient in new_recipients:
            if not new_recipient in old_recipients:
                print "added Recipient: " + new_recipient
                change = True
        
        if change:return True
        else: return False
    
    def mail(self):
        self.logfile.flush()
        self.logfile.seek(0)
        msg = email.MIMEText.MIMEText(self.logfile.read())
        msg['Subject'] = 'Empfaenger Update'
        msg['From'] = "from@something"
        msg['To'] = "to@something"

        # Send the message via our own SMTP server, but don't include the
        # envelope header.
        s = smtplib.SMTP("YOUR_MAILSERVER_IP_HERE",25)
        s.sendmail("from@something", ["to@something"], msg.as_string())
        s.quit()
        
    def cleanup(self):
        
        sys.stdout = self._stdout
        self.logfile.close()
        os.remove(self.logpath)
        

if __name__ == '__main__':
    updater = RecipientUpdater()
    change = updater.compare_maps()
    if not change:
        updater.cleanup()
        sys.exit(1)
    updater.move_map()
    updater.make_map()
    updater.mail()
    updater.cleanup()
    
