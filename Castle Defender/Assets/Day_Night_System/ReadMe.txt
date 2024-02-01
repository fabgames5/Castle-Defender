******************************Setting Up Day & Night System*********************************************************

Drag and drop the preFab "DayNightSystem" into the scene and zero out the position.
Drag and drop the preFab "Time display" into the UI Canvas and move it to a preferred location.


Next add a new layer , Something like "DayNight".
Drag and drop the preFab "DayNightCam" onto the main Camera as a child object, then reset all transforms so that it aligns with the "Main Camera".
Now change the "DayNightCam" Culling mask to "DayNight".
Set Depth to 0.


Now change the "main camera" Culling mask to everything except "DayNight".
Also change Clear flags to "Depth Only".
Set Depth to 1.
Turn off Flare Layer.


Its Best to disable/remove the original directional lights in the scene, as the system has its own.
Most of the important References in the scripts have a mouse over to help.
Now go to Windows > Lighting  set Skybox to Sky_Dynamic , and set Sun to the Sunlight Directional light in the scene.


Now go to the DayNightSystem GameObject and add all needed / Missing references to the DayNightSystem.cs script.
Set the Layer of this object to "DayNight" also change all childern as well.


Go to DayNightSystem > SunLight > Moon > MoonSprite MoonPhase.cs script and add all needed references.


 

********Known Issues: ********************************************************************************************
Moon does not Cull / hide stars behind it.
Moon flips incorrectly (Left-right) as it moves through the sky.





/////////////Next update will hopefully add weather elements to the game////////////////////////////////////////// 



