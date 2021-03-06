﻿Shader "Custom/Perlin" {
	SubShader {
		Pass {
			ZTest Always
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"


			uniform sampler2D _PermTable1D, _Gradient2D;
			uniform float _Frequency, _Lacunarity, _Gain, _x, _y, _scale;
			uniform float _left, _right, _top, _bottom, _dx, _dy;//All defined relative to (0, 0). (0, 0) is the upper left corner of the texture, and (1, 1) is the lower right.
			uniform sampler2D _previousFrame;
			
			
			struct v2f {
				float4 pos : SV_POSITION;
				float4 uv : TEXCOORD;
			};
			
			v2f vert (appdata_base v) {
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord;
				return o;
			}
			
			float2 fade(float2 t) {
				return t * t * t * (t * (t * 6 - 15) + 10);
			}
			
			float perm(float x) {
				return tex2D(_PermTable1D, float2(x,0)).a;
			}
			
			float grad(float x, float2 p) {
				float2 g = tex2D(_Gradient2D, float2(x*8.0, 0) ).rg *2.0 - 1.0;
				return dot(g, p);
			}

			float inoise(float2 p) {
				float2 P = fmod(floor(p), 256.0);	// FIND UNIT SQUARE THAT CONTAINS POINT
				p -= floor(p);                      // FIND RELATIVE X,Y OF POINT IN SQUARE.
				float2 f = fade(p);                 // COMPUTE FADE CURVES FOR EACH OF X,Y.
				
				P = P / 256.0;
				const float one = 1.0 / 256.0;
				
				// HASH COORDINATES OF THE 4 SQUARE CORNERS
				float A = perm(P.x) + P.y;
				float B = perm(P.x + one) + P.y;
				
				// AND ADD BLENDED RESULTS FROM 4 CORNERS OF SQUARE
				return lerp( lerp( grad(perm(A    ), p ),  
					grad(perm(B    ), p + float2(-1, 0) ), f.x),
				lerp( grad(perm(A+one    ), p + float2(0, -1) ),
					grad(perm(B+one    ), p + float2(-1, -1) ), f.x), f.y);
				
			}

			float fBm(float2 p, int octaves) {
				float freq = _Frequency, amp = 0.5;
				float sum = 0;	
				for(int i = 0; i < octaves; i++) 
				{
					sum += inoise(p * freq) * amp;
					freq *= _Lacunarity;
					amp *= _Gain;
				}
				return sum;
			}
			
			// fractal abs sum, range 0.0 - 1.0
			float turbulence(float2 p, int octaves) {
				float sum = 0;
				float freq = _Frequency, amp = 1.0;
				for(int i = 0; i < octaves; i++) 
				{
					sum += abs(inoise(p*freq))*amp;
					freq *= _Lacunarity;
					amp *= _Gain;
				}
				return sum;
			}
			
			// Ridged multifractal, range 0.0 - 1.0
			// See "Texturing & Modeling, A Procedural Approach", Chapter 12
			float ridge(float h, float offset) {
				h = abs(h);
				h = offset - h;
				h = h * h;
				return h;
			}
			
			float ridgedmf(float2 p, int octaves, float offset, float freq, float lac) {
				float sum = 0;
				float amp = 0.5;
				float prev = 1.0;
				for(int i = 0; i < octaves; i++) 
				{
					float n = ridge(inoise(p*freq), offset);
					sum += n*amp*prev;
					prev = n;
					freq *= lac;
					amp *= _Gain;
				}
				return sum;
			}

			fixed4 frag (v2f i) : SV_Target {
				if (i.uv.x < _left || i.uv.x > _right || i.uv.y < _top || i.uv.y > _bottom)
				{
					return tex2D(_previousFrame, i.uv.xy + float2(_dx, _dy));
				}
				i.uv.xy *= _scale;
				
				i.uv.xy += float2(_x, _y);
			
				//Biomes
				//Constant factor is biome scale, float2 is seed relative to master seed.
				float biomeHeighVariance = ridgedmf(i.uv.xy * 0.01 + float2(1000, 1000), 4, 1.0, _Frequency, _Lacunarity) * ridgedmf(i.uv.xy * 0.1 + float2(10000, 10000), 4, 1.0, _Frequency, _Lacunarity) + ridgedmf(i.uv.xy * 0.1 + float2(2000, 1000), 4, 1.0, _Frequency, _Lacunarity) * 0.1;//inoise(i.uv.xy * 0.03 + float2(1000, 1000));
				float biomeElevation = ridgedmf(i.uv.xy * 0.01 + float2(-1000, -1000), 4, 1.0, _Frequency, _Lacunarity) * ridgedmf(i.uv.xy * 0.005 + float2(-1000, -1000), 4, 1.0, _Frequency, _Lacunarity) + ridgedmf(i.uv.xy * 0.1 + float2(1000, 10000), 4, 1.0, _Frequency, _Lacunarity) * 0.1;//inoise(i.uv.xy * 0.03 + float2(-1000, -1000));
				

				//fractal noise
				//float n = fBm(i.uv.xy, 4);

				
				//turbulent noise
				//float n = turbulence(i.uv.xy, 4);
				
				
				//ridged multi fractal
				float height = ridgedmf(i.uv.xy, 4, 1.0, _Frequency, _Lacunarity);
				

				height = height * inoise(i.uv.xy * 0.5) * 0.7 + height * 0.3;
				//height += inoise(i.uv.xy * 100000 + i.uv.xy * 100000 * i.uv.xy) * 0.004;//ridgedmf(i.uv.xy * 3 + float2(1000, 1000), 4, 1.0, _Frequency, _Lacunarity) * max(1, (0.5/_scale) + 0.5);

				
				return fixed4 (height, biomeHeighVariance, biomeElevation, 1);
			}

			ENDCG

		}
	}
}