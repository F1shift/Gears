var platform;
if (navigator.appVersion.includes("Windows")) {
    platform = "UWP";
}
else if (navigator.appVersion.includes("Linux")) {
    platform = "Android";
}
else if (navigator.appVersion.includes("iOS")) {
    platform = "iOS";
}

function InvokeCS(data) {
    switch (platform) {
        case "UWP":
            window.external.notify(data);
            break;
        case "Android":
            jsBridge.Notify(data);
            break;
        default:
            console.log("Platform of " + platform + " is not implement InvokeCS method!");
            break;
    }
}

var performanceSetting;
function SceneInit(targetCanvas) {
	var timeMeasurer = CreateTimeMeasurer();
    if (platform == "UWP") //On computer
	{
		performanceSetting = {
			antialias: window.devicePixelRatio > 1.5 ? false : true,
			shadowEnabled: true,
			shadowMapType: THREE.PCFSoftShadowMap,
			shadowMapSize: 2048,
			envMapEnabled: true,
		};
	}
	else // On mobile
	{
		performanceSetting = {
            antialias: false,
            shadowEnabled: false,
			shadowMapType: THREE.BasicShadowMap,
            shadowMapSize: 1024,
            envMapEnabled: true,
		};
	}
	
	//#region レンダラーを作成
	var renderer = new THREE.WebGLRenderer({
		canvas: targetCanvas,
		alpha: true, //透明背景を作るため
		antialias: performanceSetting.antialias,
	});
	renderer.setClearColor(0x000000, 0);//透明背景を作るため
	renderer.setPixelRatio(window.devicePixelRatio);
	renderer.setSize(window.innerWidth, window.innerHeight);
	//renderer.shadowMap.enabled = navigator.appVersion.includes("Windows")? true : false;
	renderer.shadowMap.enabled = performanceSetting.shadowEnabled;
    renderer.shadowMap.type = performanceSetting.shadowMapType;
    renderer.toneMapping = THREE.CineonToneMapping;
	//#endregion

	//#region シーンを作成
	var scene = new THREE.Scene();
	//scene.background = new THREE.Color(0x000000);

	
	//#endregion

	//#region load texture 
	var evnTexture;
	var DefaultMeshMaterial;
	var DefaultLineMaterial = new THREE.LineBasicMaterial({ color:0xFF0000 });
	var textureLoaded = false;
	var SceneInitFinished = false;
	if (performanceSetting.envMapEnabled)
	{
		new THREE.RGBELoader()
			.setDataType(THREE.FloatType) // alt: FloatType, HalfFloatType
			.load('textures/autoshop_01_1k.hdr', function (texture, textureData) {

				texture.minFilter = THREE.NearestFilter;
				// texture.magFilter = THREE.NearestFilter;
				texture.encoding = THREE.LinearEncoding;

				var cubemapGenerator = new THREE.EquirectangularToCubeGenerator(texture, { resolution: 512, type: THREE.HalfFloatType });
				var exrBackground = cubemapGenerator.renderTarget.texture;
				var cubeMapTexture = cubemapGenerator.update(renderer);

				var pmremGenerator = new THREE.PMREMGenerator(cubeMapTexture);
				pmremGenerator.update(renderer);

				var pmremCubeUVPacker = new THREE.PMREMCubeUVPacker(pmremGenerator.cubeLods);
				pmremCubeUVPacker.update(renderer);

				var exrCubeRenderTarget = pmremCubeUVPacker.CubeUVRenderTarget;

				texture.dispose();
				pmremGenerator.dispose();
				pmremCubeUVPacker.dispose();

				evnTexture = exrBackground;
				DefaultMeshMaterial = new THREE.MeshStandardMaterial({
					color: 0x555555,
					metalness: 1,
					roughness: 0.5,
					envMapIntensity: 1,
					envMap: evnTexture
					});
				textureLoaded = true;
				if(SceneInitFinished)
					CSInitial();
			});
	}
	//#endregion

	//#region カメラを作成
	var camera = new THREE.PerspectiveCamera(45, window.innerWidth / window.innerHeight, 1, 10000);
    camera.position.set(1.0386, 40.733, 161.9);
    camera.rotation.set(-0.33667,0.02830, 0.00990);
	//#endregion

	//#region 光源
	var ambientLight = new THREE.AmbientLight(0xbbeeff);
	ambientLight.intensity = 0.1;
	scene.add(ambientLight);

	// var pointLight = new THREE.PointLight(0xffeebb, 1, 100000);
	// pointLight.position.set(-300, 0, 300);
	// pointLight.castShadow = true; // default false

	// //Set up shadow properties for the light;
	// pointLight.shadow.mapSize.width = performanceSetting.shadowMapSize; // default 1024
	// pointLight.shadow.mapSize.height = performanceSetting.shadowMapSize; // default 1024
	// pointLight.shadow.camera.near = 1; // default
	// pointLight.distance = 3000;
	// pointLight.shadow.camera.far = 3000;
	// pointLight.decay = 2;
	// pointLight.power = Math.PI * 4;
	// pointLight.shadow.bias = - 0.0005; // reduces self-shadowing on double-sided objects
	// scene.add(pointLight);

	var directionalLight = new THREE.DirectionalLight(0xddddbb, 4);
	directionalLight.position.set(-1000, 500, 1000);
	directionalLight.castShadow = true; // default false

	//Set up shadow properties for the light;
	directionalLight.shadow.mapSize.width = 4096; // default 1024
    directionalLight.shadow.mapSize.height = 4096; // default 1024
	directionalLight.shadow.camera.right = 1000;
	directionalLight.shadow.camera.left = -1000;
	directionalLight.shadow.camera.top = -1000;
	directionalLight.shadow.camera.bottom = 1000;
	directionalLight.shadow.camera.near = 1; // default
	directionalLight.distance = 3000;
	directionalLight.shadow.camera.far = 3000;
	directionalLight.power = Math.PI * 4;
	directionalLight.shadow.bias = - 0.0005; // reduces self-shadowing on double-sided objects
	scene.add(directionalLight);

    var directionalLight2 = new THREE.DirectionalLight(0xddddbb, 4);
    directionalLight2.position.set(1000, 500, -1000);
    directionalLight2.castShadow = true; // default false

    //Set up shadow properties for the light;
    directionalLight2.shadow.mapSize.width = 4096; // default 1024
    directionalLight2.shadow.mapSize.height = 4096; // default 1024
    directionalLight2.shadow.camera.right = 1000;
    directionalLight2.shadow.camera.left = -1000;
    directionalLight2.shadow.camera.top = -1000;
    directionalLight2.shadow.camera.bottom = 1000;
    directionalLight2.shadow.camera.near = 1; // default
    directionalLight2.distance = 3000;
    directionalLight2.shadow.camera.far = 3000;
    directionalLight2.power = Math.PI * 4;
    directionalLight2.shadow.bias = - 0.0005; // reduces self-shadowing on double-sided objects
    scene.add(directionalLight2);

	var pLightGeometry = new THREE.SphereBufferGeometry(10, 16, 8);
	// var pLightMaterial = new THREE.MeshStandardMaterial({
	// 	emissive: 0xffffee,
	// 	emissiveIntensity: 1,
	// 	color: 0x000000
	// });
	//pointLight.add(new THREE.Mesh(pLightGeometry, pLightMaterial));

	var pointLight2 = new THREE.PointLight(0xffeebb, 1, 100000);
	pointLight2.position.set(-100, -100, -800);
	pointLight2.castShadow = true; // default false

	pointLight2.shadow.mapSize.width = performanceSetting.shadowMapSize; // default 1024
	pointLight2.shadow.mapSize.height = performanceSetting.shadowMapSize; // default 1024
	pointLight2.shadow.camera.near = 1; // default
	pointLight2.distance = 3000;
	pointLight2.shadow.camera.far = 3000;
	pointLight2.penumbra = 0.2;
	pointLight2.decay = 2;
	pointLight2.power = Math.PI * 4;
	pointLight2.shadow.bias = - 0.0005; // reduces self-shadowing on double-sided objects
	scene.add(pointLight2);

	var pLightMaterial2 = new THREE.MeshStandardMaterial({
		emissive: 0xffeebb,
		emissiveIntensity: 1,
		color: 0x000000
	});
	pointLight2.add(new THREE.Mesh(pLightGeometry, pLightMaterial2));
	//#endregion

	//#region グラウンド
	// var planeGeometry = new THREE.PlaneGeometry(2000, 2000);
	// planeGeometry.rotateX(- Math.PI / 2);
	// var planeMaterial = new THREE.MeshStandardMaterial({ color: 0x555555, roughness: 0.2 });
	// var plane = new THREE.Mesh(planeGeometry, planeMaterial);
	// plane.position.y = -200;
	// plane.receiveShadow = true;
	// scene.add(plane);

	// var planeGeometry2 = new THREE.PlaneGeometry(2000, 1000);
	// var planeMaterial2 = new THREE.MeshStandardMaterial({ color: 0x555555, roughness: 1 });
	// var plane2 = new THREE.Mesh(planeGeometry2, planeMaterial2);
	// plane2.position.z = -1000;
	// plane2.position.y = 300;
	// plane2.receiveShadow = true;
	// scene.add(plane2);

	var helperXY = new THREE.GridHelper(2000, 100);
	helperXY.rotation.x = Math.PI / 2;
	helperXY.material.opacity = 0.25;
	helperXY.material.transparent = true;
	helperXY.castShadow = false;
	helperXY.receiveShadow = false;
	scene.add(helperXY);
	//#endregion

	//#region show FPS
	var stats = new Stats();
	stats.showPanel(0); // 0: fps, 1: ms, 2: mb, 3+: custom
	document.body.appendChild(stats.dom);
	//#endregion


	//#region コントロール
	//#region マウスの指しているものを判別する
	var raycaster = new THREE.Raycaster();
	var touchStartPosition = new THREE.Vector2();
	var touchEndPosition = new THREE.Vector2();
	var mouseOverMaterial = new THREE.MeshPhongMaterial({ color: 0x000000, specular: 0x666666, emissive: 0x00ff00, shininess: 10, opacity: 0.9, transparent: true });
	var selectedObjects = [];
	var selectedObjAndMaterials = [];
	
	function addSelectedObject(object) {
		selectedObjAndMaterials.push([object, object.material]);
		object.material = mouseOverMaterial;
	}

	function ClearSelectedObject() {
		selectedObjAndMaterials.forEach(function (item) {
			item[0].material = item[1];
		});
		selectedObjAndMaterials = [];
	}

	function checkIntersection(mouse) {
		raycaster.setFromCamera(mouse, camera);

		var intersects = raycaster.intersectObjects([scene], true);

		if (intersects.length > 0) {
			var selected = false;
			for (var i = 0; i < intersects.length; i++) {
				if (intersects[i].object.type == "Mesh") {
					var selectedObject = intersects[i].object;
					selected = true;
					ClearSelectedObject();
					addSelectedObject(selectedObject);
					break;
				}
			}
			if (!selected) {
				ClearSelectedObject();
			}

		} else {
			ClearSelectedObject();
		}
	}

	var touchStartTime;
	var touchEndTime;
	function onTouchStart(event) {
		touchStartTime = new Date().getTime();
		var x, y;
		if (event.changedTouches) {

			x = event.changedTouches[0].pageX;
			y = event.changedTouches[0].pageY;

		} else {

			x = event.clientX;
			y = event.clientY;

		}
		touchStartPosition.x = x;
		touchStartPosition.y = y;
	}

	function onTouchEnd(event) {
		touchEndTime = new Date().getTime();
		var timeSpan = touchEndTime - touchStartTime;
		if (timeSpan < 200) {
			var x, y;
			if (event.changedTouches) {

				x = event.changedTouches[0].pageX;
				y = event.changedTouches[0].pageY;

			} else {

				x = event.clientX;
				y = event.clientY;

			}
			
			touchEndPosition.x = x;
			touchEndPosition.y = y;

			var mouse = new THREE.Vector2();
			mouse.x = (x / window.innerWidth) * 2 - 1;
			mouse.y = - (y / window.innerHeight) * 2 + 1;

			var deltaX = touchEndPosition.x - touchStartPosition.x;
			var deltaY= touchEndPosition.y - touchStartPosition.y;
			if(Math.sqrt(deltaX * deltaX + deltaY * deltaY) < 20){
			mouse.y = - (y / window.innerHeight) * 2 + 1;
				checkIntersection(mouse);
			}
		}
	}

	$(window).on('touchstart mousedown', onTouchStart);
	$(window).on('touchend mouseup', onTouchEnd);
	//#endregion
	// var controls = new THREE.OrbitControls(camera, renderer.domElement);
	// controls.minDistance = 20;
	// controls.maxDistance = 2000;
	// controls.enablePan = false;
	var controls = new THREE.TrackballControls(camera, renderer.domElement);
	controls.rotateSpeed = 3.0;
	controls.zoomSpeed = 1.2;
	controls.panSpeed = 0.8;
	controls.noZoom = false;
	controls.noPan = false;
	controls.staticMoving = true;
	controls.dynamicDampingFactor = 0.3;
	controls.keys = [ 65, 83, 68 ];
	//#endregion
	
	//#region
	var Meshs = [];

	var Clear = function(){
		Meshs.forEach((value, index, array) => {
			scene.remove(value);
			if (value.geometry != null) {
				value.geometry.dispose();
			}
			if (value.material != null) {
				value.material.dispose();
			}
		});
		Meshs = [];
	};
	var AddBufferGeometryMesh = function(data){
		var position = data.position;
		var index = data.index;
		var color = data.color;
		var meshtype = data.type;
		var normal = data.normal;
		var geometry = new THREE.BufferGeometry();
		geometry.addAttribute('position', new THREE.Float32BufferAttribute(position, 3));
		
		if(index != null && index !== void(0))
			geometry.setIndex(index);
		else
			throw "THREE.BufferGeometry has no index information";

		if(normal == null && normal !== void(0))
			geometry.computeVertexNormals();
		else
			geometry.addAttribute('normal', new THREE.Float32BufferAttribute(normal, 3));
		
		

		var material;
		switch(meshtype){
			case "line":
				if(color != null && color !== void(0))
				{
					if(Array.isArray(color))
					{
						geometry.addAttribute('color', new THREE.Float32BufferAttribute(color, 3));
						material = new THREE.LineBasicMaterial({ vertexColors: THREE.VertexColors });
					}
					else
					{
						material = new THREE.LineBasicMaterial({ color:color });
					}
				}
				else{
					material = DefaultLineMaterial;
				}
				var linemesh = new THREE.LineSegments(geometry, material);
				Meshs.push(linemesh);
				scene.add(linemesh);
				break;
			case "mesh":
				if(color != null)
				{
					if(Array.isArray(color))
					{
						geometry.addAttribute('color', new THREE.Float32BufferAttribute(color, 3));
						material = new THREE.MeshStandardMaterial({ vertexColors: THREE.VertexColors });
					}
					else
					{
						material = new THREE.MeshStandardMaterial( {
							color:color,
							metalness: 1,
							roughness: 0.5,
							envMapIntensity: 1,
							envMap: evnTexture
						} );
					}
				}
				else{
					material = DefaultMeshMaterial;
				}
				var mesh = new THREE.Mesh(geometry, material);
				mesh.name = data.name;
				mesh.castShadow = data.castShadow;
				mesh.receiveShadow = data.receiveShadow;
				if (data.matrix != null) {
					mesh.applyMatrix(data.matrix);
				}
				Meshs.push(mesh);
				scene.add(mesh);
				break;
			default:
				Console.log("Unsupported mesh type of '" + meshtype + "'");
				break;
		}
		
	};
	var CopyMesh = function(name, newMatrix, copyedObjectName = null){
		var orgMesh = Meshs.find((value) => value.name == name);
		if(orgMesh == null)
		{
			console.log("Mesh called '" + name + "' is not found when copy mesh");
			return;
		}
		var newMesh;
		switch(orgMesh.type){
			case "Line":
				newMesh = new THREE.LineSegments(orgMesh.geometry, orgMesh.material);
				break;
			case "Mesh":
				newMesh = new THREE.Mesh(orgMesh.geometry, orgMesh.material);
		}
		if(newMatrix != null)
			newMesh.applyMatrix(newMatrix);
		newMesh.name = copyedObjectName;
		newMesh.castShadow = orgMesh.castShadow;
		newMesh.receiveShadow = orgMesh.receiveShadow;
		if(newMesh != null){
			Meshs.push(newMesh);
			scene.add(newMesh);
		}
	};
	var CreateGear = function (data) {
		var geometry = new THREE.BufferGeometry();
		geometry.addAttribute('position', new THREE.Float32BufferAttribute(data.position, 3));

		if (data.index != null && data.index !== void (0))
			geometry.setIndex(data.index);
		else
			throw "CreateGear : data has no index information";

		if (data.normal == null && data.normal !== void (0))
			geometry.computeVertexNormals();
		else
			geometry.addAttribute('normal', new THREE.Float32BufferAttribute(data.normal, 3));

		var material;
		if (data.color != null) {
			if (Array.isArray(data.color)) {
				geometry.addAttribute('color', new THREE.Float32BufferAttribute(data.color, 3));
				material = new THREE.MeshStandardMaterial({ vertexColors: THREE.VertexColors });
			}
			else {
				material = new THREE.MeshStandardMaterial({
					color: data.color,
					metalness: 1,
					roughness: 0.5,
					envMapIntensity: 1,
					envMap: evnTexture
				});
			}
		}
		else {
			material = DefaultMeshMaterial;
		}
		var mesh = new THREE.Mesh(geometry, material);
		mesh.name = "tooth";
		mesh.castShadow = data.castShadow;
		mesh.receiveShadow = data.receiveShadow;

		var group = new THREE.Group();
		group.name = data.name;
		if (data.matrix != null) {
			group.position.set(0, 0, 0);
			group.rotation.set(0, 0, 0);
			group.applyMatrix(new THREE.Matrix4().fromArray(data.matrix));
		}
		group.add(mesh);

		for (var i = 1; i < data.z; i++) {
			var newMesh = new THREE.Mesh(geometry, material);
			newMesh.applyMatrix(new THREE.Matrix4().makeRotationZ(2 * Math.PI / data.z * i));
			newMesh.name = mesh.name + i;
			newMesh.castShadow = mesh.castShadow;
			newMesh.receiveShadow = mesh.receiveShadow;
			group.add(newMesh);
		}

		Meshs.push(group);
		scene.add(group);
	};
	var UpdatreGear = function (data, orgGroup = null) {
		timeMeasurer.StartOrReset();
		if (orgGroup == null) {
			orgGroup = Meshs.find((item) => item.name === data.name);
		}
		var orgMesh = orgGroup.children.find(item => item.name == "tooth");
		var geometry = orgMesh.geometry;
		//Set attribute dynamic
		if (geometry.attributes.position.usage !== THREE.DynamicDrawUsage) {
			geometry.attributes.position.usage = THREE.DynamicDrawUsage;
		}
		if (geometry.attributes.normal &&
			geometry.attributes.normal.usage !== THREE.DynamicDrawUsage) {
			geometry.attributes.normal.usage = THREE.DynamicDrawUsage;
		}
		timeMeasurer.Report("get orgMesh and set to DynamicDrawUsage");

		//update attributes
		if (data.position != null
			&& data.position !== void (0)
			&& data.normal != null
			&& data.normal !== void (0)
		) {
			for (var i = 0; i < data.position.length; i++) {
				geometry.attributes.position.array[i] = data.position[i];
				geometry.attributes.normal.array[i] = data.normal[i];
			}
			geometry.attributes.position.needsUpdate = true;
			geometry.attributes.normal.needsUpdate = true;
		}
		timeMeasurer.Report("Set position and normal");

		//update index
		if (data.index != null && data.index !== void (0))
			geometry.setIndex(data.index);
		timeMeasurer.Report("Set index:");

		var material;
		if (data.color != null && orgMesh.material.color != data.color) {
			
			if (Array.isArray(data.color)) {
				geometry.addAttribute('color', new THREE.Float32BufferAttribute(data.color, 3));
				material = new THREE.MeshStandardMaterial({ vertexColors: THREE.VertexColors });
			}
			else {
				material = new THREE.MeshStandardMaterial({
					color: data.color,
					metalness: 1,
					roughness: 0.5,
					envMapIntensity: 1,
					envMap: evnTexture
				});
			}
			orgMesh.material = material;
		}
		else
		{
			material = orgMesh.material;
		}
		orgMesh.castShadow = data.castShadow;
		orgMesh.receiveShadow = data.receiveShadow;
		timeMeasurer.Report("Set Material and shadow settings.");

		if (orgGroup.children.length != data.z) {
			var orgMeshCopys = orgGroup.children.filter(item => item != orgMesh);
			orgMeshCopys.forEach((item) => {
				orgGroup.remove(item);
			});
			
			for (var i = 1; i < data.z; i++) {
				var newMesh = new THREE.Mesh(geometry, material);
				newMesh.applyMatrix(new THREE.Matrix4().makeRotationZ(2 * Math.PI / data.z * i));
				newMesh.name = orgMesh.name + i;
				newMesh.castShadow = orgMesh.castShadow;
				newMesh.receiveShadow = orgMesh.receiveShadow;
				orgGroup.add(newMesh);
			}
		}
		timeMeasurer.Report("Copy Single tooth Mesh");

		if (data.matrix != null) {
			orgGroup.position.set(0, 0, 0);
			orgGroup.rotation.set(0, 0, 0);
			orgGroup.applyMatrix(new THREE.Matrix4().fromArray(data.matrix));
		}
		timeMeasurer.Report("Set Gear position");
	};
	var UpdateOrCreateGear = function (data) {
		var orgGroup = Meshs.find((item) => item.name === data.name);
		if (orgGroup != null
			&& data.position.length == orgGroup.children[0].geometry.attributes.position.count * 3) {
			UpdatreGear(data, orgGroup);
		}
		else {
			CreateGear(data);
		}
	};
	

	//#region Animation
	function render() {
		stats.update();
		// レンダリング
		var timer = 0.0001 * Date.now();
		mouseOverMaterial.emissive.setHSL(0.54, 1, 0.35 * (0.5 + 0.5 * Math.cos(35 * timer)));
		renderer.render(scene, camera);
	}

	
	function tick() {
		//回転させる
		if (pointLight2 != null) {
			var time = performance.now() * 0.001;
			pointLight2.position.x = 900 * Math.sin( time * 0.6 );
			pointLight2.position.z = 900 * Math.cos( time * 0.6 );
		}

		// レンダリング
		render();
		controls.update();

		requestAnimationFrame(tick);
	}
	
	function OnResize() {
		var width = window.innerWidth;
		var height = window.innerHeight;

		renderer.setSize(width, height);

		controls.handleResize();

		camera.aspect = width / height;
		camera.updateProjectionMatrix();
	}
	controls.addEventListener( 'change', render );
	$(window).on('resize', OnResize);
	requestAnimationFrame(tick);
	//#endregion

	var SceneController = {};
	Object.defineProperties(SceneController, {
		DefaultMeshMaterial:{
			get: () => DefaultMeshMaterial
		},
		DefaultLineMaterial:{
			get: () => DefaultLineMaterial
		},
		Clear: {
			get: () => Clear
		},
		AddBufferGeometryMesh:{
			get: () => AddBufferGeometryMesh
		},
		CopyMesh : {
			get: () => CopyMesh
		},
		CreateGear : {
			get: () => CreateGear
		},
		UpdatreGear : {
			get:() =>UpdatreGear
		},
		UpdateOrCreateGear : {
			get: () => UpdateOrCreateGear
		}
	});
	SceneInitFinished = true;


	if(textureLoaded)
		CSInitial();
	return SceneController;
}

var CSInitial = () => {
	InvokeCS("StartAutoUpdate");
	InvokeCS("UpdateWebSide");
};