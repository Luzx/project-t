﻿Shader "Custom/TerrainColor" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Parallax ("Height", Range (0.005, 0.2)) = 0.02
		_BumpMap ("Normalmap", 2D) = "bump" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard 
		//fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpMap;
		uniform float _waterThreshold, _sandThreshold, _grassThreshold, _rockThreshold, _perlinShadowBias, _elevation, _mapCoefficient, _heightVariance, _normalHeight;
		uniform fixed3 _waterColor, _sandColor, _grassColor, _rockColor, _iceColor;

		float _Parallax;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 viewDir;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		
		float3 getNormal(sampler2D heightmap, float2 pos, float strength)
		{
			//Compute normal
			//This assumes heightmap is encoded into the red channel of the heightmap
			
			//Get four surrounding heightvalues
			float up = tex2D (heightmap, pos + float2(0, 0.005)).r;
			float down = tex2D (heightmap, pos + float2(0, -0.005)).r;
			float left = tex2D (heightmap, pos + float2(-0.005, 0)).r;
			float right = tex2D (heightmap, pos + float2(0.005, 0)).r;
			
			//Calculate tangent plane
			//Plane is defined as z(x, y) = dRight * x + dUp * y
			float dUp = (up - down) * strength;
			float dRight = (right - left) * strength;
			
			//Get normal to tangent plane
			float3 norm = normalize(cross(float3(1, 0, dRight), float3(0, 1, dUp)));
			
			//Make sure normal goes up and not into the terrain
			if (norm.z < 0) norm = -norm;
			
			return norm;
		}


		void surf (Input IN, inout SurfaceOutputStandard o) {

			half h = tex2D (_MainTex, IN.uv_MainTex).r;

			float2 offset = ParallaxOffset (h, _Parallax, IN.viewDir);
			IN.uv_MainTex += offset;
			IN.uv_BumpMap += offset;


			//Capture heightmap pixel
			fixed4 mapPixel = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			
			//Get height from red channel
			float height = mapPixel.r;
			
			//Get biomeHeightVariance from green channel
			float biomeHeightVariance = mapPixel.g;
			
			//Get biomeElevation from blue channel
			float biomeElevation = mapPixel.b;

			
			height *= _heightVariance * biomeHeightVariance;
				
			//Islands/Lakes
			height = height - _mapCoefficient*height*height + _mapCoefficient*height*height*height;
			
			//Brightness
			_perlinShadowBias += _heightVariance;
			
			
			height = height + _elevation + biomeElevation;
			
			
			o.Normal = getNormal(_MainTex, IN.uv_MainTex, 10);
			
			
			
			
			//o.Albedo.rgb = fixed4(slope , slope, slope, 1);
			

			
			
			//Assign color zones
			//o.Albedo (fixed3) is rgb color of output pixel
			//See "http://docs.unity3d.com/Manual/SL-SurfaceShaders.html" for other parameters
			if (height < _waterThreshold) 
			{
				if (height < 0) height = 0;
				o.Albedo = (height - _waterThreshold) + _perlinShadowBias * _waterColor;
				o.Normal = o.Normal * height + float3(0, 0, 1) * (1 - height);
			}

			else if (height * _heightVariance < _sandThreshold) 
				o.Albedo = ((height - _sandThreshold) / 30) + _perlinShadowBias * _sandColor;

					
			else if (height * _heightVariance < _grassThreshold) 
				o.Albedo = (height - _grassThreshold) + _perlinShadowBias * _grassColor;
	
					
			else if (height * _heightVariance < _rockThreshold) 
				o.Albedo = (height - _rockThreshold) + _perlinShadowBias * _rockColor;


			else o.Albedo = _perlinShadowBias * _iceColor;//height + _perlinShadowBias * _iceColor;

			
			//Other parameters
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = 1;//c.a;
			
			
			
			
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
