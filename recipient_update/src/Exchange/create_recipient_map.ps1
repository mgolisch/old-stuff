Get-Mailbox -OrganizationalUnit domain.tld/UserOU | sort -property displayname | select EmailAddresses|foreach{$_.EmailAddresses|foreach{write-output $_.SmtpAddress}}
Get-DistributionGroup -OrganizationalUnit domain.tld/Groups/External-Distribution-Groups | sort -property displayname | select EmailAddresses|foreach{$_.EmailAddresses|foreach{write-output $_.SmtpAddress}}
