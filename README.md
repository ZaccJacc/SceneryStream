# SceneryStream
Scenery Streaming for X-Plane Flight Simulators.

How it works
-----------------
XSS relies on platform-native operations for mounting network drives (currently Windows-only: mpr's WNetAddConnection2).
Using the interface, users can specify a target drive letter, a target scenery server location, and their X-Plane installation location.
XSS will then mount the target location to the local machine, and reference that mounted location in the X-Plane installation folder.
This will allow xplane to load any scenery on the XSS server into the sim.

Upcoming features
----------------------
XSS will soon be able to scan for existing installed folders to prevent installation conflicts, and will be able to support server connections for multiple X-Plane installations at once.
XSS notifications and configuration will become available in-sim as an automatiacally installed X-Plane plugin.
