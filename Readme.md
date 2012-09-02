
Usb Keys (USB Drives / USB Memory Sticks) are often used to back up data that is on computers, and there are numerous programs 
out there for that purpose.

Another usage mode for these devices, however, is to normally keep files ONLY on the USB stick - so that you can carry around 
the one authoritative version, and potentially to avoid the innformation in question being stored on individual computers that 
others might have or gain access to.

This progra was designed for this second usage scenario - it will back up the contents of a given drive to the local computer,
encrypting on-the-fly, so that the data is safe (from accidental loss/destruction), but still secret.

Changing revisions of a file are stored (up to a time or change count limit), so as long as you run the backup reasonably 
frequently this also serves as a very light revision control system.

In retrospect, this was an ambitious project/system to design from scratch for a week's coding, but it was actually created
for a single person's specific needs, and it seems to have succeeded in that at least.

Some things that probably don't make sense:

* Instead of symmetric encryption, asymmetric encryption could have made more sense, security-wise (at the cost of some usability)
* The "Explore Backups" and "Restore" functionality could be a lot prettier and easier to use; there must be some open-source file-manager UI out there that could be reused...
* ??? (I last touched this a few years ago, memory fails me)
