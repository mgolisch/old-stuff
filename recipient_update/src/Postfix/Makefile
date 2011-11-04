DB=db
all:	relay_recipients.$(DB)
# "all" means to build virtual.db

relay_recipients.proto:	virtual.txt
	awk -F: '{printf("%s\tOK\n",$$1)}' virtual.txt > relay_recipients.proto
# We need virtual.txt to build relay_recipients.proto
# awk will use ":" as field separator and for each line
# that contains "alias: (SMTP|smtp):" it will do:
# print the third row, insert a TAB, insert "OK" and add a newline
# into relay_recipients.proto

%.$(DB):	%.proto
	/usr/sbin/postmap $*.proto && mv $*.proto.$(DB) $*.$(DB)
# Building a *.db requires a *.proto file. If that exists,
# postmap is called to build the map from *.proto. If postmap is successful
# the *.proto map will be renamed to *.db
