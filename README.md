githere
=======
short git status in [branch +~-] format right in the bottom of your IDE

![scr](http://visualstudiogallery.msdn.microsoft.com/0344701c-45e4-4b3b-8d2b-92e5c8ad5bbe/image/file/136883/1/w3lk4jD.png?Id=136883)

the format is borrowed from a must-have [posh-git](https://github.com/dahlbyk/posh-git) PowerShell extension

installation
============
grab it from [VS gallery](http://visualstudiogallery.msdn.microsoft.com/0344701c-45e4-4b3b-8d2b-92e5c8ad5bbe)

status
======
**NOT TESTED AT ALL**, it will steal your soul and set your machine on fire, use at your own risk.

known issues
============
I still didn't figure out how to load the exact LibGit2Sharp DLL version I need, so there may be problems when using this extension alongside other LibGit2Sharp-dependant extensions ([GitDiffMargin](https://github.com/laurentkempe/GitDiffMargin), [SaveAllTheTime](https://github.com/paulcbetts/SaveAllTheTime)).

building
========
*I'm building it on Windows 8.1 x64, VS 2013 Premium. You also need to have [VS 2013 SDK](http://www.microsoft.com/en-us/download/details.aspx?id=40758) installed.*
