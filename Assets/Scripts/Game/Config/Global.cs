using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global
{
    public struct LayerType
    {
        public const string Default = "Default";
        public const string TransparentFX = "TransparentFX";

		public const string UI = "UI";
        public const string UIModel3D = "UIModel3D";
    }

    public struct Tag
    {
        public const string UILayer3D = "3DUI";
        public const string UILayer2D = "2DUI";
		public const string UISplash = "UISplash";
		public const string UICamera2D = "UICamera2D";

        public const string Terrain = "Terrain";
    }

    public struct SceneName
    {

    }
}
