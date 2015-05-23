Shader "Custom/Terrain" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		uniform float _waterThreshold, _sandThreshold, _grassThreshold, _rockThreshold, _perlinShadowBias, _elevation, _mapCoefficient, _heightVariance, _waterAnimation;
		uniform fixed3 _waterColor, _sandColor, _grassColor, _rockColor, _iceColor;
		
		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			//Capture heightmap pixel
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			
			//Get height from red channel
			float n = c.r;

			
			n *= _heightVariance;
				
			//Islands/Lakes
			n = n - _mapCoefficient*n*n + _mapCoefficient*n*n*n;
			
			//Brightness
			_perlinShadowBias += _heightVariance;
			
			
			n = n + _elevation;
			
			//Assign color zones
			//TODO: make colors accessible via unity
			//o.Albedo (fixed3) is rgb color of output pixel
			//See "http://docs.unity3d.com/Manual/SL-SurfaceShaders.html" for other parameters
			if (n < _waterThreshold) 
			{
				if (n < 0) n = 0;
				o.Albedo = (n + 0.7) * _perlinShadowBias * _waterColor;//fixed3 (((n) + 0.7) * _perlinShadowBias * 0.2, ((n) + 0.7) * _perlinShadowBias * 0.3, ((n) + 0.7) * _perlinShadowBias * 0.8);
			}

			else if (n * _heightVariance < _sandThreshold) 
				o.Albedo = n * _perlinShadowBias * _sandColor;//fixed3 (n * _perlinShadowBias * 1, n * _perlinShadowBias * 0.7, n * _perlinShadowBias * 0.3);

					
			else if (n * _heightVariance < _grassThreshold) 
				o.Albedo = n * _perlinShadowBias * _grassColor;//fixed3 (n * _perlinShadowBias * 0.2, n * _perlinShadowBias * 0.7, n * _perlinShadowBias * 0.2);
	
					
			else if (n * _heightVariance < _rockThreshold) 
				o.Albedo = n * _perlinShadowBias * _rockColor;//fixed3 (n * _perlinShadowBias * 0.4, n * _perlinShadowBias * 0.4, n * _perlinShadowBias * 0.3);


			else o.Albedo = n * _perlinShadowBias * _iceColor;//fixed3 (n * _perlinShadowBias * 0.6, n * _perlinShadowBias * 0.6, n * _perlinShadowBias * 0.7);

			
			//Other parameters
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = 1;//c.a;
			
			
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
