@echo off
@cd c:\scripts
@echo "Starting recipient export"
@C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -PSConsoleFile "C:\Program Files\Microsoft\Exchange Server\bin\exshell.psc1" -command ". 'C:\scripts\create_recipient_map.ps1'" > c:\scripts\virtual.txt
@pscp -l root -pw rootpasswd_here virtual.txt 192.168.0.2:/etc/postfix/virtual_new.txt
@plink -l root -pw rootpasswd_here postfix_ip_here /etc/postfix/update_recipients.sh
