<h1>What is dabbit</h2>

dabbit is my attempt at building a simplified, cross-platform IRC client. My goals are to make the interface simple to use (NO clutter), prevent the need to constantly reconfigure when you jump to another machine or a new machine, allow ease of use with BNCs, and make IRC easy to use and fun to look at.

<h2>Roadmap</h2>

What needs to be done:

* Base library
    ** Parse input and make calls to methods
* Each Client UI
    ** Windows 8/Phone 8 Apps
    ** Android App
    ** iOS App (Tablet and phone will share similiar interfaces)
    ** Web HTML App which will be ported to desktops + Chrome
* A Socket Server
    ** The socket server will keep connections open for clients and forward connections on browsers that don't have native TCP sockets. Think mibbit for desktops

The Configuration file will be downloaded via Skydrive/Google Drive/Local Sync option which a user will login with. This way you as a user will have the freedom to know where your config file is going. This lets dabbit have the same settings no matter where you're at.

<h2>Scripting Support</h2>

dabbit will support scripting. The language will most likely be a form of Javascript.

Thanks for checking out dabbit!
