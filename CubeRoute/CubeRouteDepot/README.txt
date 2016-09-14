1) Download Unity version 5.3.1p4 at: https://unity3d.com/unity/qa/patch-releases

2) Download the "Cube Route" folder that is located inside of this directory (it should have 2 and only 2 sub-folders: "Assets" and "ProjectSettings")

3) Open \Cube Route\Assets\Cookie Dough Games\Scenes\Temp.unity and Unity will automatically create the remaining needed project files locally



Notes:

- Third party assets are stored in the root "Assets" folder so that they can be more easily updated (Unity automatically unpackages assets into the root folder). All of our work should be contained in the "Cookie Dough Games" folder so that it can be cleanly separated.

- Everyone has a personal workspace where they can experiment/develop new scripts and assets. Try to move your asset into a non-workspace folder when it is ready for team-wide use.

- There are CodeMaid and ReSharper settings in the root "Assets" folder that can be imported for each plugin if you are using Visual Studio.
	- http://www.codemaid.net/
	- https://www.jetbrains.com/resharper/
	
- When pushing your work to Perforce, be careful to only submit files contained within the "Assets" and "ProjectSettings" folders -- the rest aren't needed in version control