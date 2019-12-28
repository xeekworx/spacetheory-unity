Creating star in edit mode
	To create custom star use Sun prefab from StarGen/Prefabs folder. Press "Make Unique" button in the inspector, and choose folder and name for your star materials.
	After that you can edit parameters.
	Use playmode to see particles, prominence and shader animations. 
	However, you will require to save component settings before leaving into edit mode (cog icon->"Copy component"), then you should paste component values and press "Force Update" button.

Generating stars at runtime
	Instaniate Sun prefab from StarGen/Prefabs folder, then call MakeUniqueRunTime() from his HU_Star component.
		GameObject star = Instaniate(starPrefab);
		HU_Star starComponent = star.GetComponent<HU_Star>();
		starComponent.MakeUniqueRunTime();
		//Now you can set up parameters
		starComponent._color=new Color(0,0,1);
		starComponent._rim=1;
		...

Use Post Processing Stack: https://assetstore.unity.com/packages/essentials/post-processing-stack-83912 with included profile (StarGen/Post-Processing Profile) to get results like in demo.

Parameters
	_starMaterial - refence to star matrial.
	_coronaMateral - refence to corona matrial. You probably dont need to touch them.
	_size - size of the star. Note that this will only change the scale of details on the star. To change physical size use transform.
	_color - star color. If you not using color grading or HDR range on camera than you probably want to set it little bit darker to get more details.
	_rim - rim brightness.
	_flowSpeed - shader animation speed.
	_spots - amount and size of spots. Darker areas of the star.
	_coronaBrightness - brightness of prominence, sprite corona and particles corona.
	_coronaSpeed
	_flare - enable flare sprite?
	_flareSize
	_flareContrast
	_coronaSprites - enable corona sprites?
	_simpleCoronaSprites - more simple and much faster version of corona sprites.
	_coronaDensity - amount of corona sprites. May cause slowdown on high values (more than 1).
	_coronaSize
	_coronaWaves - corona size variation.
	_coronaRipple
	_coronaParticles - enable corona particles?
	_coronaTrails - change particles to their trails.
	_trailsResolution - amount of trails segments.
	_coronaParticlesSize
	_particleDensity
	_particleSpread
	_particleWavesAmplitude - noise amount.
	_particleWavesFreq - noise size.
	prominenceAmount - probably of spawning prominence.
	prominenceSize - max height of prominence.
	_accretionDisk - enable accretion disk?
	_accretionSegments - density of accretion disk. Adds volume.
	_accretionColor1
	_accretionColor2
	_accretionSpeed - speed of rotation and flow.
	_accretionSize
	_accretionTwist
	_accretionInnerRadius - center hole in disk.
	_jets - enable jets?
	_jetHeight
	_jetWidth
	_jetSmear - streechiness of the texture.
	_jetSpread
	_jetSpeed
	_jetDensity - number of segments. Adds volume.

	
