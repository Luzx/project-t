Shader "Custom/AddTextureShader" {
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
		uniform sampler2D _WaterTex, _SandTex, _GrassTex, _RockTex, _IceTex;
		uniform float _LevelWidth, _TexWidth, _TexHeight, _AspectRatio, _Brightness;
		
		//uniform float _waterThreshold, _sandThreshold, _grassThreshold, _rockThreshold;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		
		float3 getNormalFromBlue(sampler2D heightmap, float2 pos, float strength)
		{
			//Compute normal
			//This assumes heightmap is encoded into the red channel of the heightmap
			
			//Get four surrounding heightvalues
			float up = tex2D (heightmap, pos + float2(0, 0.005)).b;
			float down = tex2D (heightmap, pos + float2(0, -0.005)).b;
			float left = tex2D (heightmap, pos + float2(-0.005, 0)).b;
			float right = tex2D (heightmap, pos + float2(0.005, 0)).b;
			
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
		
		float getTexAlpha(float sig, float actual)
		{
			float alpha = abs(actual - sig);
			
			//Comment out this line to get smooth texture transitions
			return (alpha > _LevelWidth * 0.5) ? 0 : 1;
			
			alpha = 1 - (alpha / _LevelWidth);
			return max(alpha, 0);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			

			//c *= tex2D(_GrassTex, fmod(IN.uv_MainTex * 10, 1024));
			
			
			//o.Albedo = float3(1, 1, 1) * c.r;
			//o.Albedo.r = c.r;
			//o.Albedo.r = c.r;
			//float2 tileReductionSamplePos = fmod((IN.uv_MainTex * float2(_AspectRatio, 1)) * float2(-9, 9), 1);
			IN.uv_MainTex = fmod((IN.uv_MainTex * float2(_AspectRatio, 1)) * 5, 1);
			
			
			
			float alpha;
			alpha = getTexAlpha(0, c.g);
			if (alpha > 0) o.Albedo += tex2D (_WaterTex, IN.uv_MainTex).rgb * alpha * c.r * _Brightness;
			
			
			alpha = getTexAlpha(_LevelWidth, c.g);
			if (alpha > 0) o.Albedo += (tex2D (_SandTex, IN.uv_MainTex).rgb) * alpha * c.r * _Brightness;
			
			alpha = getTexAlpha(2 * _LevelWidth, c.g);
			if (alpha > 0) o.Albedo += (tex2D (_GrassTex, IN.uv_MainTex).rgb) * alpha * c.r * _Brightness;
			
			alpha = getTexAlpha(3 * _LevelWidth, c.g);
			if (alpha > 0) o.Albedo += (tex2D (_RockTex, IN.uv_MainTex).rgb) * alpha * c.r * _Brightness;
			
			alpha = getTexAlpha(4 * _LevelWidth, c.g);
			if (alpha > 0) o.Albedo += tex2D (_IceTex, IN.uv_MainTex).rgb * alpha * c.r * _Brightness;
			
			
			o.Smoothness = min(1, getTexAlpha(0, c.g) + getTexAlpha(4 * _LevelWidth, c.g));//Make it shiny if it's water or snow
			
			//o.Albedo.g = c.g;
			//o.Albedo.b = c.b;
			// Metallic and smoothness come from slider variables
			o.Metallic = 0;
			o.Smoothness = 0;
			o.Alpha = 1;
			
			o.Normal = float3(0, 0, 1);
			//o.Normal = getNormalFromBlue(_MainTex, IN.uv_MainTex, 50.0);
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
