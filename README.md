traymonkey
==========

A small systray app to do things in response to the focused application changing.

The original aim was to have a way to have my system volume maxed when XBMC was launched 
and focused. What TrayMonkey does right now is that, through simple ini configuration, it
can set a volume value (or restore the last value before a set) on focusing / defocusing
any application, as configured with a right-match on the application path (so, for
example, if you have an application at C:\Program Files\SomeApp\Version01\someapp.exe,
you can match it with any of:
someapp.exe
Version01\someapp.exe
01\someapp.exe
(etc).
Also matches are case-insensitive.

When I have the time and need, I'd like to extend this tray app to respond to application
launches / closes, so you could, for instance, close your email application when opening
Steam, or something similar. For now, it's just a handy way to not have to concern myself
with setting the system volume to max when XBMC runs and reducing it afterwards.

Please note that the TrayMonkey repo has a checkin of the CoreAudio API project,
found at 

http://www.codeproject.com/Articles/18520/Vista-Core-Audio-API-Master-Volume-Control

This project is used to manipulate the system volume. All rights and kudos for that code
go to the original author and it is at the discretion of the original author that
I may remove this code. But I really hope I don't have to (:
