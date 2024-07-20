EFSAdvent v1.2 released 2022-05-02
By JaytheHam
http://www.jaytheham.com/efsa/

Known problems
--------------
- Missing lots of Actor sprites and information.

v1.2 Changes
------------
* .szs files are now re-encoded as Yaz0 on export, so edited layers display correctly on GBA.
* Made some drawing faster.
* Added bigger brushes.
* Increased tile change undo/redo to 500 steps.
* Rarc packing now supports arbitrary levels of folders (not that more than two are needed...)
* Added tile copy/paste
* Boss500 (Shadow Battle map) can now be loaded.
* Window can be resized.
* Added ability to clone an existing actor.
* New "Save as" feature lets you save an existing level with a different name so it can replace some other level.
* Many small bug fixes and usability tweaks.

v1.1 Changes
------------
* If no actor file exists a new one is made. Creation of new rooms with actors is now possible.
* Worked out the first two map variables. X and Y of starting room.
* Undo/Redo system implemented for tile changes. 50 steps.
* Substantialy increased speed at which layers are drawn.
* Layers can now only be edited while the tilesheet tab is selected.
* Actors can now be selected by left clicking on their origin marker while the actors tab is selected, the selected actor can be dragged around with the left mouse button.
* Can now select either a 1x1 or 2x2 tile brush.
* Saves .csv data when exporting
* Supports variable actor sprite sizes