© 2022 - Leonardo Vieira
© 2022 - Willian Freitas
============================


QUICK START
-----------
The default rendering version on this package is HDRP. If you want to upgrade for either URP or Standard rendering pipeline, you may do so by importing the provided unity packages inside the folder "Assets\Nokdef_VFXPack\Elemental Mesh FX\Upgrades"

If you get script errors on the camera or post process volume, you can either delete them, or get the official unity post process package. (Regarding the preview scene only)

Once you have the right rendering pipeline selected, you can simply choose a particle system inside of the "Assets\Nokdef_VFXPack\Elemental Mesh FX\MeshFX_Prefabs" folder and drag it as a child of whatever model you want to apply the effect on, then set the shape' mesh render field to the parent model.

Make sure you have the Core RP installed if using HDRP or URP, and Unninstalled if you use Standard. (You can check that on the package manager.)

If your model does not use unity's default measurement scale, the particles may look too small or too big. You can adjust that on the particle system itself, but ideally all of your models should have a baseline scale of 1.

CONTACT
-------
Questions, suggestions, help needed?
Contact me at:

leonardo.v.alvarez@hotmail.com


RELEASE NOTES
-------------
1.0 - Initial release.
1.5 - 3 New Elements. (TECH, GUMDROP, SAND)