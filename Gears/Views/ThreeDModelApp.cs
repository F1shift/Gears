//using System;
//using System.Collections.Generic;
//using System.Text;

//using Urho;
//using Urho.Forms;
//using Urho.Gui;

//namespace Gears.Views
//{
//    class ThreeDModelApp : Urho.Application
//    {
//        public ThreeDModelApp(ApplicationOptions options) : base(options)
//        {
            
//        }

//        protected override void Start()
//        {
//            base.Start();
//            var scene = new Scene();
//            scene.CreateComponent<Octree>();

//            var lightNode = scene.CreateChild("lightNode");
//            lightNode.SetDirection(new Vector3(1, -1, 1));
//            var light = lightNode.CreateComponent<Light>();
//            light.LightType = LightType.Directional;

//            var cameraNode = scene.CreateChild("cameraNode");
//            cameraNode.Position = new Vector3(0, 50, 0);
//            cameraNode.SetDirection(new Vector3(0, -1, 1));
//            var camera = cameraNode.CreateComponent<Camera>();
//            camera.Orthographic = false;
//            camera.Fov = 108;

//            var zoneNode = scene.CreateChild("Zone");
//            var zone = zoneNode.CreateComponent<Zone>();
//            zone.SetBoundingBox(new BoundingBox(-3000.0f, 3000.0f));
//            zone.AmbientColor = new Color(0.15f, 0.15f, 0.15f);
//            zone.FogColor = new Color(1, 1, 1);
//            zone.FogStart = 100;
//            zone.FogEnd = 300;

//            var groundNode = scene.CreateChild("groundNode");
//            groundNode.Scale = new Vector3(1000, 1, 1000);
//            var ground = groundNode.CreateComponent<StaticModel>();
//            ground.Model = CoreAssets.Models.Plane;
//            var groundMaterial = new Material();
//            groundMaterial.SetTechnique(0, CoreAssets.Techniques.NoTexture, 0, 0);
//            groundMaterial.SetShaderParameter("MatDiffColor", new Color(1f, 1f, 1f));
//            ground.Material = groundMaterial;

//            var gridNode = scene.CreateChild("gridNode");
//            gridNode.Scale = new Vector3(1000, 1, 1000);
//            var grid = gridNode.CreateComponent<CustomGeometry>();
//            var gridMaterial = new Material(Context);
//            gridMaterial.SetTechnique(0, CoreAssets.Techniques.NoTexture, 0, 0);
//            grid.SetMaterial(gridMaterial);
//            grid.BeginGeometry(0, PrimitiveType.LineList);
//            grid.DefineVertex(new Vector3(1, 0.1f, 1));
//            grid.DefineColor(Color.Red);
//            grid.DefineVertex(new Vector3(0, 0.1f, -1));
//            grid.DefineColor(Color.Red);
//            grid.Commit();

//            var viewport = new Viewport(Context, scene, camera, null);
//            Renderer.SetViewport(0, viewport);
//            viewport.SetClearColor(Color.White);
//        }
//    }
//}
