Shader "Custom/TerrainColor" {
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
		uniform float _waterThreshold, _sandThreshold, _grassThreshold, _rockThreshold, _perlinShadowBias, _elevation, _mapCoefficient, _heightVariance, _mountainHeight, _LevelWidth;
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
		
		fixed4 getPixel(sampler2D map, float2 pos)
		{
			//TODO: Add optional smoothing
			return tex2D(map, pos);
		}
		
		float computeHeight(fixed4 mapPixel)
		{
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
			
			
			return height;
		}
		
		

		void surf (Input IN, inout SurfaceOutputStandard o) {

			//Capture heightmap pixel
			fixed4 mapPixel = getPixel(_MainTex, IN.uv_MainTex) * _Color;
			
			float height = computeHeight(mapPixel);
			
			float2 offset = ParallaxOffset (height, _Parallax, IN.viewDir);
			IN.uv_MainTex += offset;
			IN.uv_BumpMap += offset;
			
			o.Normal = getNormal(_MainTex, IN.uv_MainTex, _mountainHeight) * mapPixel.g;
			
			
			//Compute slope as the cosine to the angle between the surface normal and the camera direction
			float slope = dot(o.Normal, float3(0, 0, 1));

			
			//Assign color zones
			//o.Albedo (fixed3) is rgb color of output pixel
			//See "http://docs.unity3d.com/Manual/SL-SurfaceShaders.html" for other parameters
			if (height < _waterThreshold) 
			{
				if (height < 0) height = 0;
				o.Albedo.r = (height - _waterThreshold) + _perlinShadowBias;
				o.Normal = o.Normal * height + float3(0, 0, 1) * (1 - height);
				o.Emission.g = 0;
				o.Emission.b = height;
			}

			else if (height * _heightVariance < _sandThreshold) 
			{
				o.Albedo.r = ((height - _sandThreshold) / 30) + _perlinShadowBias;
				o.Emission.g = _LevelWidth;	
				o.Emission.b = height * _heightVariance;
			}

					
			else if (height * _heightVariance < _grassThreshold) 
			{
				o.Albedo.r = (height - _grassThreshold) + _perlinShadowBias;
				o.Emission.g = 2 * _LevelWidth;
				o.Emission.b = height * _heightVariance;
			}
	
					
			else if (height * _heightVariance < _rockThreshold) 
			{
				if (slope > 0.8 || height * _heightVariance < _grassThreshold + 0.2) o.Albedo.r = (height - _grassThreshold) + _perlinShadowBias;
				else o.Albedo.r = (height - _rockThreshold) + _perlinShadowBias;
				
				o.Emission.g = 3 * _LevelWidth;
				o.Emission.b = height * _heightVariance;
			}


			else 
			{
				o.Albedo.r = slope * _perlinShadowBias + 0.7;
				o.Emission.g = 4 * _LevelWidth;
				o.Emission.b = height * _heightVariance;
			}
			
			
			
			
				
			
			
			//Other parameters
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = 1;//c.a;
			//o.Emission = float3(0, o.Albedo.g, o.Albedo.b);
			
			
			
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
