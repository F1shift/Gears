function SceneInit(targetCanvas) {
	var platform = navigator.appVersion.includes("Windows") ? "PC" : "mobile";
	var performanceSetting;
	if (platform == "PC") //On computer
	{
		performanceSetting = {
			antialias: window.devicePixelRatio > 1.5 ? false : true,
			shadowEnabled: true,
			shadowMapType: THREE.PCFSoftShadowMap,
			shadowMapSize: 2048,
		};
	}
	else // On mobile
	{
		performanceSetting = {
			antialias: false,
			shadowEnabled: true,
			shadowMapType: THREE.BasicShadowMap,
			shadowMapSize: 1024,
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
	//#endregion

	//#region シーンを作成
	var scene = new THREE.Scene();
	scene.background = new THREE.Color(0x000000);
	//#endregion

	//#region カメラを作成
	var camera = new THREE.PerspectiveCamera(45, window.innerWidth / window.innerHeight, 1, 10000);
	camera.position.set(0, 0, 1000);
	//#endregion

	//#region 光源
	var ambientLight = new THREE.AmbientLight(0xffffff);
	ambientLight.intensity = 0.5;
	scene.add(ambientLight);

	var pointLight = new THREE.PointLight(0xFFFFFF, 1, 100000);
	pointLight.position.set(-300, 0, 300);
	pointLight.castShadow = true; // default false

	//Set up shadow properties for the light;
	pointLight.shadow.mapSize.width = performanceSetting.shadowMapSize; // default 1024
	pointLight.shadow.mapSize.height = performanceSetting.shadowMapSize; // default 1024
	pointLight.shadow.camera.near = 1; // default
	pointLight.distance = 3000;
	pointLight.shadow.camera.far = 3000;
	pointLight.decay = 2;
	pointLight.power = Math.PI * 4;
	pointLight.shadow.bias = - 0.0005; // reduces self-shadowing on double-sided objects
	scene.add(pointLight);

	var pLightGeometry = new THREE.SphereBufferGeometry(10, 16, 8);
	var pLightMaterial = new THREE.MeshStandardMaterial({
		emissive: 0xffffee,
		emissiveIntensity: 1,
		color: 0x000000
	});
	pointLight.add(new THREE.Mesh(pLightGeometry, pLightMaterial));

	var pointLight2 = new THREE.PointLight(0xffeebb, 1, 100000);
	pointLight2.position.set(0, -100, 800);
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
	var planeGeometry = new THREE.PlaneGeometry(2000, 2000);
	planeGeometry.rotateX(- Math.PI / 2);
	var planeMaterial = new THREE.MeshStandardMaterial({ color: 0x555555, roughness: 0.2 });
	var plane = new THREE.Mesh(planeGeometry, planeMaterial);
	plane.position.y = -200;
	plane.receiveShadow = true;
	scene.add(plane);

	var planeGeometry2 = new THREE.PlaneGeometry(2000, 1000);
	var planeMaterial2 = new THREE.MeshStandardMaterial({ color: 0x555555, roughness: 1 });
	var plane2 = new THREE.Mesh(planeGeometry2, planeMaterial2);
	plane2.position.z = -1000;
	plane2.position.y = 300;
	plane2.receiveShadow = true;
	scene.add(plane2);

	// var helper = new THREE.GridHelper(2000, 100);
	// helper.position.y = - 199;
	// helper.material.opacity = 0.25;
	// helper.material.transparent = true;
	// helper.castShadow = false;
	// helper.receiveShadow = false;
	//scene.add(helper);

	var helperXY = new THREE.GridHelper(2000, 100);
	helperXY.rotation.x = Math.PI / 2;
	helperXY.material.opacity = 0.25;
	helperXY.material.transparent = true;
	helperXY.castShadow = false;
	helperXY.receiveShadow = false;
	scene.add(helperXY);

	var helperXZ = new THREE.GridHelper(2000, 100);
	helperXZ.material.opacity = 0.25;
	helperXZ.material.transparent = true;
	helperXZ.castShadow = false;
	helperXZ.receiveShadow = false;
	scene.add(helperXZ);
	//#endregion

	//#region show FPS
	var stats = new Stats();
	stats.showPanel(0); // 0: fps, 1: ms, 2: mb, 3+: custom
	document.body.appendChild(stats.dom);
	//#endregion

	//#region デフォルト球体を加入する
	var mesh = GetPresetMesh2(100, 200, 150, 0, 0, 0, 0xffffff);
	var mesh2 = GetPresetMesh2(100, 200, 50, -400, -100, 0, 0xFFFFFF);
	mesh.material.overdraw = 0.5;
	mesh2.material.overdraw = 0.5;
	mesh.material.needsUpdate = true;
	mesh2.material.needsUpdate = true;
	mesh.castShadow = true;
	mesh.receiveShadow = true;
	mesh2.castShadow = true;
	mesh2.receiveShadow = true;
	//scene.add(mesh);
	scene.add(mesh2);
	//#endregion

	function GetPresetMesh2(vres, hres, r, x, y, z, color) {
		var position = [];
		var normal = [];
		var uv = [];
		for (var i = 0; i < vres; i++) {
			for (var j = 0; j < hres; j++) {
				var theta_z = Math.PI / 2 - Math.PI / (vres - 1) * i;
				var theta_h = 2 * Math.PI / (hres - 1) * j;
				var r_h = r * Math.cos(theta_z);
				position.push(
					r_h * Math.cos(theta_h) + x,
					r_h * Math.sin(theta_h) + y,
					r * Math.sin(theta_z) + z);
				normal.push(
					Math.cos(theta_z) * Math.cos(theta_h),
					Math.cos(theta_z) * Math.sin(theta_h),
					Math.sin(theta_z));

				var u = i / (vres - 1.0);
				var v = j / (hres - 1.0);
				uv.push(u, v);
			}
		}

		// Add cell faces (2 traingles per cell) to geometry
		var indices = [];
		var n = vres;
		var m = hres;
		for (var i = 0; i < n - 1; i++) {
			for (var j = 0; j < m - 1; j++) {
				var n0 = i * m + j;
				var n1 = n0 + 1;
				var n2 = (i + 1) * m + j + 1;
				var n3 = n2 - 1;
				indices.push(n2, n1, n0);
				indices.push(n0, n3, n2);
			}
		}
		// Initialise threejs geometry
		var geometry = new THREE.BufferGeometry();
		geometry.setIndex(indices);
		geometry.addAttribute('position', new THREE.Float32BufferAttribute(position, 3));
		geometry.addAttribute('normal', new THREE.Float32BufferAttribute(normal, 3));
		geometry.addAttribute('uv', new THREE.Float32BufferAttribute(uv, 2));

		var mesh;
		var material = new THREE.MeshStandardMaterial({ color: color, roughness: 0 });
		mesh = new THREE.Mesh(geometry, material);

		return mesh;
	}

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
	var SceneController = {
		AddBufferGeometryMesh:function(vertices, indices, color, meshtype, normal = null){
			var geometry = new THREE.BufferGeometry();
			geometry.addAttribute('position', new THREE.Float32BufferAttribute(vertices, 3));
			
			if(indices != null)
				geometry.setIndex(indices);
			else
				throw "THREE.BufferGeometry has no index information";

			if(normal == null)
				geometry.computeVertexNormals();
			else
				geometry.addAttribute('normal', new THREE.Float32BufferAttribute(normal, 3));
			
			

			var material;
			switch(meshtype){
				case "line":
					if(color != null)
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
						material = new THREE.LineBasicMaterial({ color:0xFF0000 });
					}
					var linemesh = new THREE.LineSegments(geometry, material);
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
							material = new THREE.MeshStandardMaterial({ color:color });
						}
					}
					else{
						material = new THREE.MeshStandardMaterial({ color:0xFF0000 });
					}
					var mesh = new THREE.Mesh(geometry, material);
					scene.add(mesh);
					break;
				default:
					break;
			}
			
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
		if (mesh != null) {
			mesh.rotation.x += 0.01;
			mesh.rotation.y += 0.01;
			mesh2.rotation.y += 0.02;
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

	return SceneController;
}