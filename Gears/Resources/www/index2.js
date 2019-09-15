
window.addEventListener('DOMContentLoaded', init);

var mesh;
var mesh2;
var scene;
var camera;
var renderer;
var stats;
var composer, renderPass, outlinePass;
var raycaster = new THREE.Raycaster();
var mouse = new THREE.Vector2();
var mouseOverTexture, earthTexture, moonTexture;
var mouseOverMaterial;
var selectedObjects = [];
var selectedObjAndMaterials = [];
var globalClippingPlane;

function init() {

    //#region レンダラーを作成
    renderer = new THREE.WebGLRenderer({
        canvas: document.querySelector('#myCanvas')
    });
    renderer.setPixelRatio(window.devicePixelRatio);
    renderer.setSize(window.innerWidth, window.innerHeight);
    renderer.shadowMap.enabled = true;
    //renderer.shadowMap.type = THREE.PCFSoftShadowMap;
    renderer.shadowMap.renderReverseSided = false;
    renderer.localClippingEnabled = true;
    //#endregion

    //#region シーンを作成
    scene = new THREE.Scene();
    scene.background = new THREE.Color(0x000000);
    //#endregion

    //#region カメラを作成
    camera = new THREE.PerspectiveCamera(45, window.innerWidth / window.innerHeight, 1, 10000);
    camera.position.set(0, 0, 1000);
    //#endregion

    //#region コントロール
    var controls = new THREE.OrbitControls(camera, renderer.domElement);
    controls.addEventListener('change', render);
    controls.minDistance = 20;
    controls.maxDistance = 2000;
    controls.enablePan = false;
    //#endregion

    //#region postprocessing(OutLine effect)
    // composer = new THREE.EffectComposer( renderer );

    // renderPass = new THREE.RenderPass( scene, camera );
    // composer.addPass( renderPass );

    // outlinePass = new THREE.OutlinePass( new THREE.Vector2( window.innerWidth, window.innerHeight ), scene, camera );
    // outlinePass.renderToScreen = true;
    // outlinePass.visibleEdgeColor.set('#ff6f00');
    // outlinePass.hiddenEdgeColor.set('#ffffff');
    // outlinePass.pulsePeriod = 2;
    // outlinePass.usePatternTexture = false;
    // composer.addPass( outlinePass );

    // var onLoad = function (texture) {

    //     //outlinePass.patternTexture = texture;
    //     texture.wrapS = THREE.RepeatWrapping;
    //     texture.wrapT = THREE.RepeatWrapping;

    // };

    // var loader = new THREE.TextureLoader();

    // //Read from textureData.js to prevent CROS error.
    // mouseOverTexture = loader.load(greyWhiteCheckeredData, onLoad);
    // //mouseOverMaterial = new THREE.MeshPhongMaterial( { map: mouseOverTexture, shininess : 0, refractionRatio: 0, reflectivity: 0} );
    // mouseOverMaterial = new THREE.MeshStandardMaterial({ map: mouseOverTexture, roughness: 1 });
    //#endregion

    //#region Texture
    var onLoad = function (texture) {
        texture.wrapS = THREE.RepeatWrapping;
        texture.wrapT = THREE.RepeatWrapping;
    };

    var loader = new THREE.TextureLoader();

    //Read from textureData.js to prevent CROS error.
    //mouseOverTexture = loader.load(greyWhiteCheckeredData, onLoad);
    //earthTexture = loader.load("examples/textures/planets/earth_atmos_4096.jpg", onLoad);
    //moonTexture = loader.load("examples/textures/planets/moon_1024.jpg",onLoad);
    //earthTexture.rotation = Math.PI / 2;
    //moonTexture.rotation = Math.PI / 2;
    //mouseOverMaterial = new THREE.MeshStandardMaterial({ map: mouseOverTexture, roughness: 1 });
    //#endregion

    //#region window.addEventListener
    window.addEventListener('mousemove', onTouchMove);
    window.addEventListener('touchmove', onTouchMove);
    window.addEventListener('resize', OnResize, false);
    //#endregion

    //#region 環境オブジェクト

    //#region 光源
    // var spotLight = new THREE.SpotLight( 0xffffff, 1.5 );
    // spotLight.position.set( 0, 500, 0 );
    // spotLight.angle = Math.PI / 4;
    // spotLight.penumbra = 0.05;
    // spotLight.decay = 0;
    // spotLight.distance = 100000;

    // spotLight.castShadow = true;
    // spotLight.shadow.mapSize.width = 2048;
    // spotLight.shadow.mapSize.height = 2048;
    // spotLight.shadow.camera.near = 1;
    // spotLight.shadow.camera.far = 100000;
    //scene.add( spotLight );

    // lightHelper = new THREE.SpotLightHelper( spotLight, 0x99CC00 );
    // scene.add( lightHelper );
    // shadowCameraHelper = new THREE.CameraHelper( spotLight.shadow.camera );
    // scene.add( shadowCameraHelper );

    var ambientLight = new THREE.AmbientLight(0xffffff);
    ambientLight.intensity = 0.5;
    scene.add(ambientLight);

    var pointLight = new THREE.PointLight(0xFFFFFF, 1, 100000);
    pointLight.position.set(30, 0, 300);
    pointLight.castShadow = true; // default false

    //Set up shadow properties for the light;
    pointLight.shadow.mapSize.width = 2048; // default 1024
    pointLight.shadow.mapSize.height = 2048; // default 1024
    pointLight.shadow.camera.near = 1; // default
    pointLight.shadow.camera.far = 10000
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

    pointLight2.shadow.mapSize.width = 2048; // default 1024
    pointLight2.shadow.mapSize.height = 2048; // default 1024
    pointLight2.shadow.camera.near = 1; // default
    pointLight2.shadow.camera.far = 10000
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

    var helper = new THREE.GridHelper(2000, 100);
    helper.position.y = - 199;
    helper.material.opacity = 0.25;
    helper.material.transparent = true;
    helper.castShadow = false;
    helper.receiveShadow = false;
    scene.add(helper);

    var axes = new THREE.AxesHelper(1000);
    // axes.position.set( 0, 0, 0 );
    //axes.position.set( -500, - 500, - 500 );
    //scene.add( axes );
    //#endregion


    //#endregion

    //Clipping Setting
    globalClippingPlane = new THREE.Plane(new THREE.Vector3(- 1, 0, 0), 0);//for Global Clipping

    //#region WebSocketコネクションを作る
    var sock = new WebSocket("ws://localhost:8080");
    sock.onopen = function (event) {
        console.log("Socket connected");
    }
    sock.onmessage = SocketOnMessage;
    //#endregion

    //#region show FPS
    stats = new Stats();
    stats.showPanel(0); // 0: fps, 1: ms, 2: mb, 3+: custom
    document.body.appendChild(stats.dom);
    //#endregion

    //#region デフォルト球体を加入する
    mesh = GetPresetMesh2(50, 100, 150, 0, 0, 0, 0xffffff, false, 75);
    mesh2 = GetPresetMesh2(50, 100, 50, -400, -100, 0, 0xFFFFFF, false, -100);
    mesh.material.map = earthTexture;
    mesh2.material.map = moonTexture;
    mesh.material.overdraw = 0.5;
    mesh2.material.overdraw = 0.5;
    mesh.material.needsUpdate = true;
    mesh2.material.needsUpdate = true;
    mesh.castShadow = true;
    mesh.receiveShadow = true;
    mesh2.castShadow = true;
    mesh2.receiveShadow = true;
    scene.add(mesh);
    scene.add(mesh2);
    //#endregion

    //Animationl0-op;o99-
    requestAnimationFrame(tick);
}

function render() {
    stats.update();
    // レンダリング
    renderer.render(scene, camera);
    //composer.render(scene, camera);
}

function tick() {

    //箱を回転させる
    if (mesh != null) {
        mesh.rotation.x += 0.01;
        mesh.rotation.y += 0.01;
        mesh2.rotation.y += 0.02;
    }

    // レンダリング
    render();

    requestAnimationFrame(tick);
}

function SocketOnMessage(event) {
    try {
        var geometry = parseGeometry(event.data, true);
        const material = new THREE.MeshStandardMaterial({ color: 0x0000FF });
        if (mesh != null) {
            scene.remove(mesh);
        }
        mesh = new THREE.Mesh(geometry, material);
        mesh.castShadow = true;
        scene.add(mesh);
    }
    catch (e) {
        console.log("recieve massege : " + event.data);
    }

}

function parseGeometry(varr, invert) {
    var vertexs = JSON.parse(varr);

    // Initialise threejs geometry
    var geometry = new THREE.Geometry();

    // Add grid vertices to geometry
    var n = vertexs.length;
    var m = vertexs[0].length;
    for (var i = 0; i < n; i++) {
        for (var j = 0; j < m; j++) {
            var newvert = new THREE.Vector3(vertexs[i][j][0], vertexs[i][j][1], vertexs[i][j][2]);
            geometry.vertices.push(newvert);
        }
    }

    // Add face to geometry
    for (var i = 0; i < n - 1; i++) {
        for (var j = 0; j < m - 1; j++) {
            var n0 = i * m + j;
            var n1 = n0 + 1;
            var n2 = (i + 1) * m + j + 1;
            var n3 = n2 - 1;
            if (invert) {
                face1 = new THREE.Face3(n2, n1, n0);
                face2 = new THREE.Face3(n0, n3, n2);
            }
            else {
                face1 = new THREE.Face3(n0, n1, n2);
                face2 = new THREE.Face3(n2, n3, n0);
            }
            geometry.faces.push(face1);
            geometry.faces.push(face2);
        }
    }

    geometry.computeFaceNormals();

    return geometry;
}

//using Geomatry
function GetPresetMesh(vres, hres, r, x, y, z, color, clip, clipDis) {
    var vertexs = new Array(vres);
    for (var i = 0; i < vres; i++) {
        vertexs[i] = new Array(hres);
    }

    for (var i = 0; i < vres; i++) {
        for (var j = 0; j < hres; j++) {
            vertexs[i][j] = [0, 0, 0];
            var theta_z = Math.PI / 2 - Math.PI / (vres - 1) * i;
            var theta_h = 2 * Math.PI / (hres - 1) * j;
            var r_h = r * Math.cos(theta_z);
            vertexs[i][j][0] = r_h * Math.cos(theta_h) + x;
            vertexs[i][j][1] = r_h * Math.sin(theta_h) + y;
            vertexs[i][j][2] = r * Math.sin(theta_z) + z;
        }
    }
    // Initialise threejs geometry
    var geometry = new THREE.Geometry();

    //// Add grid vertices to geometry
    var n = vertexs.length;
    var m = vertexs[0].length;
    for (var i = 0; i < vres; i++) {
        for (var j = 0; j < hres; j++) {
            var newvert = new THREE.Vector3(vertexs[i][j][0], vertexs[i][j][1], vertexs[i][j][2]);
            geometry.vertices.push(newvert);
        }
    }

    // Add cell faces (2 traingles per cell) to geometry
    for (var i = 0; i < n - 1; i++) {
        for (var j = 0; j < m - 1; j++) {
            var n0 = i * m + j;
            var n1 = n0 + 1;
            var n2 = (i + 1) * m + j + 1;
            var n3 = n2 - 1;
            var uv0 = new THREE.Vector2(i / (n - 1.0), j / (m - 1.0));
            var uv1 = new THREE.Vector2(i / (n - 1.0), (j + 1) / (m - 1.0));
            var uv2 = new THREE.Vector2((i + 1) / (n - 1.0), (j + 1) / (m - 1.0));
            var uv3 = new THREE.Vector2((i + 1) / (n - 1.0), j / (m - 1.0));
            face1 = new THREE.Face3(n2, n1, n0);
            face2 = new THREE.Face3(n0, n3, n2);
            geometry.faces.push(face1);
            geometry.faceVertexUvs[0].push([uv2, uv1, uv0]);
            geometry.faces.push(face2);
            geometry.faceVertexUvs[0].push([uv0, uv3, uv2]);
        }
    }

    // Compute normals for shading
    geometry.computeFaceNormals();
    geometry.uvsNeedUpdate = true;

    ////geometry.computeVertexNormals();
    var mesh;
    if (clip) {
        var plane = new THREE.Plane(new THREE.Vector3(0, -1, 0), clipDis);
        geometry.clearGroups();
        geometry.addGroup(0, Infinity, 0);
        geometry.addGroup(0, Infinity, 1);

        const material = new THREE.MeshStandardMaterial({
            color: color,
            roughness: 0,
            opacity: 0.5,
            transparent: true,
            visible: true,
            // clippingPlanes:[plane],
            // clipShadows: true,
            side: THREE.FrontSide
        });
        const material2 = new THREE.MeshStandardMaterial({
            color: 0xFF0000,
            roughness: 0,
            opacity: 0.5,
            transparent: true,
            visible: true,
            // clippingPlanes:[plane],
            // clipShadows: true,
            side: THREE.FrontSide
        });
        mesh = new THREE.Mesh(geometry, [material, material2]);


        // const material2 = new THREE.MeshStandardMaterial({ 
        //     color: color, 
        //     roughness : 0,
        //     clippingPlanes:[plane],
        //     clipShadows: true
        //  });
        //  var shadowMesh = new THREE.Mesh(geometry, material2);
        //  shadowMesh.scale.set(1.1, 1.1, 1.1);
        //mesh = new THREE.Group();

        //mesh.add(Mesh)
        //mesh.add(shadowMesh)
    }
    else {
        const material = new THREE.MeshStandardMaterial({ color: color, roughness: 0 });
        mesh = new THREE.Mesh(geometry, material);
    }

    return mesh;
}

//using BufferGeomatry
function GetPresetMesh2(vres, hres, r, x, y, z, color, clip, clipDis) {
    var position = [];
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
    geometry.setIndex( indices );
    geometry.addAttribute( 'position', new THREE.Float32BufferAttribute( position, 3 ) );
    geometry.addAttribute('uv', new THREE.Float32BufferAttribute( uv, 2) )
    

    // Compute normals for shading
    geometry.computeVertexNormals();

    ////geometry.computeVertexNormals();
    var mesh;
    if (clip) {
        var plane = new THREE.Plane(new THREE.Vector3(0, -1, 0), clipDis);
        

        const material = new THREE.MeshStandardMaterial({
            color: 0x000000,
            roughness: 0,
            opacity: 1,
            transparent: true,
            visible: true,
            clippingPlanes:[plane],
            clipShadows: true,
            //side: THREE.DoubleSide,
            wireframe: true
        });

        const material2 = new THREE.MeshStandardMaterial({
            color: color,
            roughness: 0,
            opacity: 1,
            transparent: true,
            visible: true,
            clippingPlanes:[plane],
            clipShadows: true,
            side: THREE.DoubleSide
        });
        geometry.clearGroups();
        geometry.addGroup(0, indices.length, 0);
        geometry.addGroup(0, indices.length, 1);
        mesh = new THREE.Mesh(geometry, [material, material2]);
        // const material2 = new THREE.MeshStandardMaterial({
        //     color: color,
        //     roughness: 0,
        //     opacity: 1,
        //     transparent: true,
        //     visible: true,
        //     clippingPlanes:[plane],
        //     clipShadows: true,
        //     side: THREE.BackSide,
        //     shadowSide: THREE.FrontSide
        // });
        // geometry.clearGroups();
        // geometry.addGroup(0, indices.length, 0);
        // geometry.addGroup(0, indices.length, 1);
        // mesh = new THREE.Mesh(geometry, [material, material2]);


        // const material2 = new THREE.MeshStandardMaterial({ 
        //     color: color, 
        //     roughness : 0,
        //     clippingPlanes:[plane],
        //     clipShadows: true
        //  });
        //  var shadowMesh = new THREE.Mesh(geometry, material2);
        //  shadowMesh.scale.set(1.1, 1.1, 1.1);
        //mesh = new THREE.Group();

        //mesh.add(Mesh)
        //mesh.add(shadowMesh)
    }
    else {
        const material = new THREE.MeshStandardMaterial({ color: color, roughness: 0 });
        mesh = new THREE.Mesh(geometry, material);
    }

    return mesh;
}

function addSelectedObject(object) {
    selectedObjAndMaterials.push([object, object.material])
    if(Array.isArray(object.material))
    {
        mouseOverMaterial.copy(object.material[0]);
    }
    else
    {
        mouseOverMaterial.copy(object.material);
    }
    mouseOverMaterial.map = mouseOverTexture;
    object.material = mouseOverMaterial;
    // outlinePass.selectedObjects = [];
}

function ClearSelectedObject() {
    // selectedObjects = [];
    

    selectedObjAndMaterials.forEach(function (item) {
        item[0].material = item[1];
    });
    selectedObjAndMaterials = [];
    // outlinePass.selectedObjects = [];
}

function checkIntersection() {

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

function onTouchMove(event) {

    var x, y;

    if (event.changedTouches) {

        x = event.changedTouches[0].pageX;
        y = event.changedTouches[0].pageY;

    } else {

        x = event.clientX;
        y = event.clientY;

    }

    mouse.x = (x / window.innerWidth) * 2 - 1;
    mouse.y = - (y / window.innerHeight) * 2 + 1;

    checkIntersection();

}

function OnResize() {
    var width = window.innerWidth;
    var height = window.innerHeight;

    renderer.setSize(width, height);
    //composer.setSize( width, height );

    camera.aspect = width / height;
    camera.updateProjectionMatrix();
}

