// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Fx/Effect_shader_jiuge"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[Enum(Particle,0,Material,1)]_Particle("粒子模式或材质模式", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)]_Effect_Blend("混合模式", Float) = 1
		[Enum(UnityEngine.Rendering.CompareFunction)]_ZTestMode("深度模式", Float) = 4
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("剔除模式", Float) = 0
		[Enum(on,1,off,0)]_ClipMode("裁切模式", Float) = 0
		_Cutout("裁切大小", Range( 0 , 1)) = 0.5
		[HDR]_Main_color("主贴图颜色", Color) = (1,1,1,1)
		[Toggle]_Usebackcolor("开启背面颜色", Range( 0 , 1)) = 0
		_Backcolor("背面贴图颜色", Color) = (1,1,1,1)
		_Man_Tex("主贴图", 2D) = "white" {}
		[Toggle]_DiagonalUV("开启对角贴图模式", Range( 0 , 1)) = 0
		_TexContrast("贴图对比度", Float) = 1
		_TexBright("贴图亮度", Float) = 1
		_TexAlpha("贴图Alpha强度", Float) = 1
		[Enum(Alpha,0,R,1)]_Texchoose_R("使用Alpha通道或者R通道", Float) = 0
		_Tex_Desaturate("贴图去色", Range( 0 , 1)) = 1
		[Toggle]_UseMask("使用遮罩", Range( 0 , 1)) = 0
		_Mask("遮罩贴图", 2D) = "white" {}
		_Noise("扰动溶解贴图", 2D) = "white" {}
		[Toggle]_UseDistort("开启扰动", Float) = 0
		_MainPannerX("U方向贴图偏移", Float) = 0
		_MainPannerY("V方向贴图偏移", Float) = 0
		_U_Distort_Speed("U方向扰动速度", Float) = 0
		_V_Distort_Speed("V方向扰动速度", Float) = 0
		_DistortPower("扰动强度", Float) = 0
		[Toggle]_UseDissolve("开启溶解", Range( 0 , 1)) = 0
		_Dissolve("溶解强度", Range( 0 , 1)) = 0.1924071
		_Hardness("溶解软硬强度", Range( 0 , 1)) = 0.6136052
		_EdgeWidth("溶解宽度", Range( 0 , 1)) = 0
		[HDR]_Widthcolor("溶解宽度颜色", Color) = (1,1,1,1)
		_FadeLength("贴图深度衰退", Range( 0 , 1)) = 0
		[Toggle]_Usefresnel("开启菲涅尔", Range( 0 , 1)) = 0
		_Fresnel("菲涅尔轮廓大小", Range( 0 , 1)) = 0
		[Toggle]_FresnelFilp("菲涅尔翻转", Range( 0 , 1)) = 0
		_Fresnelintensity("菲涅尔强度", Range( 0 , 1)) = 0
		[Toggle]_Use_Rotator("开启定向溶解", Range( 0 , 1)) = 0
		_Rotator_Dissolve("定向溶解强度", Range( 0 , 1)) = 0.5602916
		_Rotator("定向溶解方向", Range( 0 , 6.3)) = 6.3
		[Toggle]_polar("开启极坐标", Range( 0 , 1)) = 0
		_TlingOffset2("极坐标UV重复度", Vector) = (1,1,0,0)
		_UVpanner2("极坐标UV偏移速度", Vector) = (0,0,0,0)
		_ExtrusionPoint("挤压点", Float) = 0
		_Noise_niuqu("顶点扰动噪声", 2D) = "white" {}
		_Noise_off("噪声 off", Float) = 2
		
		[Space(20)]
		[Toggle(ASE_FOG)]_ASE("ASE FOG", float) = 0
		_Stop("stop=0", float) = 1

		[Space(20)]
		[Toggle(_SOFTPARTICLE_ON)]_SoftParticles_Enable("开启软粒子", float) = 0
		_SoftParticlesNearFadeDistance("软粒子 近淡出", Float) = 0.0
        _SoftParticlesFarFadeDistance("软粒子 远淡出", Float) = 0.2
        _CameraNearFadeDistance("相机 近淡出", Float) = 4
        _CameraFarFadeDistance("相机 远淡出", Float) = 0.1
      
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

		[HideInInspector]_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		[HideInInspector]_TessValue( "Tess Max Tessellation", Range( 1, 32 ) ) = 16
		[HideInInspector]_TessMin( "Tess Min Distance", Float ) = 10
		[HideInInspector]_TessMax( "Tess Max Distance", Float ) = 25
		[HideInInspector]_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		[HideInInspector]_TessMaxDisp( "Tess Max Displacement", Float ) = 25
		
		[Header(Stencil)]
		_StencilComp ("Stencil Comparison", Float) = 8
	    _Stencil ("Stencil ID", Float) = 0
	    _StencilOp ("Stencil Operation", Float) = 0
	    _StencilWriteMask ("Stencil Write Mask", Float) = 255
	    _StencilReadMask ("Stencil Read Mask", Float) = 255
	}

	SubShader
	{
		LOD 0

		
		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent+100" }
		
		Cull [_CullMode]
		
		Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp] 
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
		
		HLSLINCLUDE
		#pragma target 3.0

		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}
		
		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlane (float3 pos, float4 plane)
		{
			float d = dot (float4(pos,1.0f), plane);
			return d;
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForward" }
			
			Blend SrcAlpha [_Effect_Blend]
			ZWrite [_ClipMode]
			ZTest [_ZTestMode]
			Offset 0,0
			ColorMask RGBA
			

			HLSLPROGRAM
			#define _RECEIVE_SHADOWS_OFF 1
			#pragma multi_compile_instancing
			#define ASE_SRP_VERSION 999999
			#define REQUIRE_DEPTH_TEXTURE 1

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma shader_feature _ HEIGHT_FOG// ATMOSPHERIC_FOG_DAY ATMOSPHERIC_FOG_NIGHT
			#pragma shader_feature_local _ ASE_FOG
			#pragma shader_feature_local _ _SOFTPARTICLE_ON
			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#if (HEIGHT_FOG || ATMOSPHERIC_FOG_DAY || ATMOSPHERIC_FOG_NIGHT) && ASE_FOG
			#include "Packages/com.pwrd.time-of-day/Resources/Shader/Include/FogCore.hlsl"
			#endif
			#if ASE_SRP_VERSION <= 70108
			#define REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
			#endif

			#define ASE_NEEDS_FRAG_COLOR
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_VERT_POSITION


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD1;
				#endif
				#if (HEIGHT_FOG || ATMOSPHERIC_FOG_DAY || ATMOSPHERIC_FOG_NIGHT) && ASE_FOG
					float fogFactor : TEXCOORD2;
				#endif
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_color : COLOR;
				float4 ase_texcoord6 : TEXCOORD6;
				float4 ase_texcoord7 : TEXCOORD7;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Mask_ST;
			float4 _Main_color;
			float4 _Backcolor;
			float4 _Noise_ST;
			half4 _UVpanner2;
			half4 _TlingOffset2;
			float4 _Man_Tex_ST;
			half4 _Widthcolor;
			half _FresnelFilp;
			half _Fresnelintensity;
			float _UseMask;
			half _TexContrast;
			float _TexAlpha;
			half _Texchoose_R;
			half _UseDissolve;
			half _Hardness;
			float _Usebackcolor;
			half _Usefresnel;
			half _Cutout;
			half _TexBright;
			half _Fresnel;
			float _ZTestMode;
			half _polar;
			half _ClipMode;
			half _CullMode;
			half _Effect_Blend;
			half _EdgeWidth;
			float _Particle;
			half _DiagonalUV;
			half _MainPannerX;
			half _MainPannerY;
			half _Dissolve;
			half _Rotator_Dissolve;
			half _Rotator;
			half _Use_Rotator;
			half _DistortPower;
			half _U_Distort_Speed;
			half _V_Distort_Speed;
			half _UseDistort;
			float _Tex_Desaturate;
			half _FadeLength;
			float _TessPhongStrength;
			float _TessValue;
			float _TessMin;
			float _TessMax;
			float _TessEdgeLength;
			float _TessMaxDisp;
			float _Noise_off;
			float _ExtrusionPoint;
			
			float _Stop;

			float _SoftParticlesNearFadeDistance;
			float _SoftParticlesFarFadeDistance;
			float _CameraNearFadeDistance;
			float _CameraFarFadeDistance;
			CBUFFER_END
			sampler2D _Man_Tex;
			sampler2D _Noise;
			sampler2D _Noise_niuqu;
			sampler2D _Mask;
			uniform float4 _CameraDepthTexture_TexelSize;

			//软粒子
			half CameraFade(float near, float far, float4 projection)
			{
			    float thisZ = LinearEyeDepth(projection.z / projection.w, _ZBufferParams);
			    return saturate((thisZ - near) * far);
			}
			
			float SoftParticles(float near, float far, float4 projection)
			{
			    float fade = 1;
			    if (near > 0.0 || far > 0.0)
			    {
			        float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, projection.xy / projection.w), _ZBufferParams);
			        float thisZ = LinearEyeDepth(projection.z / projection.w, _ZBufferParams);
			        fade = saturate (far * ((sceneZ - near) - thisZ));
			    }
			    return fade;
			}	
						
			VertexOutput VertexFunction ( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord6.xyz = ase_worldNormal;
				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord7 = screenPos;
				
				o.ase_texcoord3.xy = v.ase_texcoord.xy;
				o.ase_texcoord4 = v.ase_texcoord2;
				o.ase_texcoord5 = v.ase_texcoord1;
				o.ase_color = v.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord3.zw = 0;
				o.ase_texcoord6.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue =  ( v.ase_normal * max( ( ( v.vertex.xyz.x + ( ( tex2Dlod( _Noise_niuqu, float4( ( ( _TimeParameters.x ) + v.ase_texcoord.xy ), 0, 0.0) ).r * _Noise_off ) - 1.0 ) ) * _ExtrusionPoint ) , 0.0 ) );
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				VertexPositionInputs vertexInput = (VertexPositionInputs)0;
				vertexInput.positionWS = positionWS;
				vertexInput.positionCS = positionCS;
				o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				#if (HEIGHT_FOG || ATMOSPHERIC_FOG_DAY || ATMOSPHERIC_FOG_NIGHT) && ASE_FOG
				  TRANSFER_FOG_FACTOR(o.fogFactor, positionWS)
				#endif
				o.clipPos = positionCS;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord2 = v.ase_texcoord2;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord1 = v.ase_texcoord1;
				o.ase_color = v.ase_color;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord2 = patch[0].ase_texcoord2 * bary.x + patch[1].ase_texcoord2 * bary.y + patch[2].ase_texcoord2 * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag ( VertexOutput IN , half ase_vface : VFACE ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif
				float2 uv0_Man_Tex = IN.ase_texcoord3.xy * _Man_Tex_ST.xy + _Man_Tex_ST.zw;
				float4 uv2273 = IN.ase_texcoord4;
				uv2273.xy = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float4 appendResult274 = (float4(uv2273.x , uv2273.y , uv2273.z , uv2273.w));
				float2 appendResult518 = (float2(_UVpanner2.x , _UVpanner2.y));
				float2 panner521 = ( 1.0 * _Time.y * appendResult518 + float2( 0,0 ));
				float4 appendResult279 = (float4(1.0 , 1.0 , _EdgeWidth , panner521.x));
				float4 lerpResult275 = lerp( appendResult274 , appendResult279 , _Particle);
				float4 break280 = lerpResult275;
				float2 appendResult290 = (float2(break280.x , break280.y));
				half2 UV3399 = appendResult290;
				float2 temp_output_284_0 = ( UV3399 + float2( -1,-1 ) );
				float2 lerpResult427 = lerp( uv0_Man_Tex , abs( (uv0_Man_Tex*2.0 + -1.0) ) , _DiagonalUV);
				float4 uv1_Man_Tex = IN.ase_texcoord5;
				uv1_Man_Tex.xy = IN.ase_texcoord5.xy * _Man_Tex_ST.xy + _Man_Tex_ST.zw;
				float4 appendResult261 = (float4(uv1_Man_Tex.x , uv1_Man_Tex.y , uv1_Man_Tex.z , uv1_Man_Tex.w));
				float2 temp_cast_1 = (_Dissolve).xx;
				float temp_output_449_0 = (0.0 + (_Rotator_Dissolve - 0.0) * (1.038 - 0.0) / (1.0 - 0.0));
				float2 uv1430 = IN.ase_texcoord5.xy * float2( 1,1 ) + float2( 0,0 );
				float cos432 = cos( _Rotator );
				float sin432 = sin( _Rotator );
				float2 rotator432 = mul( uv1430 - float2( 0.5,0.5 ) , float2x2( cos432 , -sin432 , sin432 , cos432 )) + float2( 0.5,0.5 );
				float2 temp_cast_2 = (5.0).xx;
				float2 lerpResult436 = lerp( temp_cast_1 , ( temp_output_449_0 * pow( ( temp_output_449_0 + ( temp_output_449_0 * rotator432 ) ) , temp_cast_2 ) ) , _Use_Rotator);
				//* pwrd ykk:时停
				float t = _TimeParameters.x * _Stop;
				float4 appendResult267 = (float4(( _MainPannerX * (t) ) , ( (t) * _MainPannerY ) , lerpResult436.x , _DistortPower));
				float4 lerpResult272 = lerp( appendResult261 , appendResult267 , _Particle);
				float4 break271 = lerpResult272;
				float2 appendResult282 = (float2(break271.x , break271.y));
				half2 UV2398 = appendResult282;
				float2 uv0_Noise = IN.ase_texcoord3.xy * _Noise_ST.xy + _Noise_ST.zw;
				float2 appendResult247 = (float2(_U_Distort_Speed , _V_Distort_Speed));
				half Distort_2405 = tex2D( _Noise, ( uv0_Noise + ( appendResult247 * (t) ) ) ).r;
				half Distort295 = break271.w;
				float lerpResult300 = lerp( 0.0 , ( ( Distort_2405 - 0.5 ) * Distort295 ) , _UseDistort);
				half Distort_1403 = lerpResult300;
				float2 UV_main482 = ( ( ( uv0_Man_Tex * temp_output_284_0 ) + ( temp_output_284_0 * float2( -0.5,-0.5 ) ) ) + ( ( lerpResult427 + UV2398 ) + Distort_1403 ) );
				float2 temp_output_458_0 = (UV_main482*2.0 + -1.0);
				float2 break460 = temp_output_458_0;
				float2 appendResult463 = (float2(length( temp_output_458_0 ) , (0.0 + (atan2( break460.y , break460.x ) - 0.0) * (1.0 - 0.0) / (3.141593 - 0.0))));
				float2 lerpResult464 = lerp( UV_main482 , appendResult463 , _polar);
				float2 Jzb477 = lerpResult464;
				float2 appendResult487 = (float2(_TlingOffset2.x , _TlingOffset2.y));
				half Bright303 = break280.w;
				float2 JZB494 = ( (Jzb477*appendResult487 + 0.0) + Bright303 );
				float4 tex2DNode189 = tex2D( _Man_Tex, JZB494 );
				float3 desaturateInitialColor429 = tex2DNode189.rgb;
				float desaturateDot429 = dot( desaturateInitialColor429, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar429 = lerp( desaturateInitialColor429, desaturateDot429.xxx, _Tex_Desaturate );
				float3 temp_cast_5 = (_TexContrast).xxx;
				float4 lerpResult40 = lerp( _Main_color , _Backcolor , _Usebackcolor);
				float4 lerpResult199 = lerp( lerpResult40 , _Main_color , max( ase_vface , 0.0 ));
				float4 Sidedcolor203 = lerpResult199;
				float4 temp_output_14_0 = ( float4( pow( desaturateVar429 , temp_cast_5 ) , 0.0 ) * _TexBright * Sidedcolor203 * IN.ase_color );
				float2 temp_cast_8 = (1.0).xx;
				float temp_output_120_0 = ( Distort_2405 + 1.0 );
				float Dissolve294 = break271.z;
				half DissWidth296 = break280.z;
				float temp_output_186_0 = ( Dissolve294 * ( 1.0 + DissWidth296 ) );
				float temp_output_307_0 = (0.6 + (_Hardness - 0.0) * (0.99 - 0.6) / (1.0 - 0.0));
				half Hardness242 = temp_output_307_0;
				float temp_output_141_0 = ( 1.0 - Hardness242 );
				float2 appendResult216 = (float2(saturate( ( ( ( temp_output_120_0 - ( temp_output_186_0 * ( 1.0 + temp_output_141_0 ) ) ) - temp_output_307_0 ) / ( 1.0 - temp_output_307_0 ) ) ) , saturate( ( ( ( temp_output_120_0 - ( ( temp_output_186_0 - DissWidth296 ) * ( 1.0 + temp_output_141_0 ) ) ) - temp_output_307_0 ) / ( 1.0 - temp_output_307_0 ) ) )));
				float2 lerpResult414 = lerp( temp_cast_8 , appendResult216 , _UseDissolve);
				float2 break217 = lerpResult414;
				float4 lerpResult181 = lerp( ( _Widthcolor * temp_output_14_0 ) , temp_output_14_0 , break217.x);
				float lerpResult190 = lerp( tex2DNode189.a , tex2DNode189.r , _Texchoose_R);
				float2 uv_Mask = IN.ase_texcoord3.xy * _Mask_ST.xy + _Mask_ST.zw;
				float lerpResult227 = lerp( 1.0 , tex2D( _Mask, uv_Mask ).r , _UseMask);
				float Mask395 = lerpResult227;
				float Alpha326 = _Main_color.a;
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 ase_worldNormal = IN.ase_texcoord6.xyz;
				float dotResult377 = dot( ase_worldViewDir , ase_worldNormal );
				float temp_output_380_0 = ( max( dotResult377 , 0.0 ) * _Fresnelintensity );
				float lerpResult383 = lerp( temp_output_380_0 , ( 1.0 - temp_output_380_0 ) , _FresnelFilp);
				float lerpResult391 = lerp( 1.0 , saturate( ( ( lerpResult383 - _Fresnel ) / ( 0.0 - _Fresnel ) ) ) , _Usefresnel);
				half Fresnel392 = lerpResult391;
				float temp_output_204_0 = ( IN.ase_color.a * lerpResult190 * _TexAlpha * Mask395 * Alpha326 * Fresnel392 * break217.y );
				float temp_output_239_0 = min( temp_output_204_0 , 1.0 );
				float4 appendResult224 = (float4(lerpResult181.rgb , temp_output_239_0));
				clip( temp_output_239_0 - min( _Cutout , _ClipMode ));
				float3 appendResult235 = (float3(lerpResult181.rgb));
				float4 appendResult236 = (float4(appendResult235 , 1.0));
				float4 lerpResult234 = lerp( appendResult224 , appendResult236 , _ClipMode);
				float4 appendResult311 = (float4(lerpResult234));
				float4 screenPos = IN.ase_texcoord7;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth319 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth319 = abs( ( screenDepth319 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( (0.0 + (_FadeLength - 0.0) * (2.0 - 0.0) / (1.0 - 0.0)) ) );
				half Fade321 = saturate( distanceDepth319 );
				
				float3 BakedAlbedo = 0;
				float3 BakedEmission = 0;
				float3 Color = ( appendResult311 * Fade321 ).xyz;
				float Alpha = temp_output_204_0;
				float AlphaClipThreshold = 0.5;

				//软粒子
				#if _SOFTPARTICLE_ON
				Alpha *= SoftParticles(_SoftParticlesNearFadeDistance, _SoftParticlesFarFadeDistance, screenPos);
				Alpha *= CameraFade(_CameraNearFadeDistance, _CameraFarFadeDistance, screenPos);
				#endif
				
				#ifdef _ALPHATEST_ON
					clip( Alpha - AlphaClipThreshold );
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				//防止溢出
				half4 c = half4( clamp(Color.rgb, 0, 100), Alpha );
				  #if (HEIGHT_FOG || ATMOSPHERIC_FOG_DAY || ATMOSPHERIC_FOG_NIGHT) && ASE_FOG
				 APPLY_FOG_COLOR(c, IN.fogFactor, WorldPosition, _MainLightPosition.xyz, ase_worldViewDir)

				#endif

				return c;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0

			HLSLPROGRAM
			#define _RECEIVE_SHADOWS_OFF 1
			#pragma multi_compile_instancing
			#define ASE_SRP_VERSION 999999

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_FRAG_WORLD_POSITION


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Mask_ST;
			float4 _Main_color;
			float4 _Backcolor;
			float4 _Noise_ST;
			half4 _UVpanner2;
			half4 _TlingOffset2;
			float4 _Man_Tex_ST;
			half4 _Widthcolor;
			half _FresnelFilp;
			half _Fresnelintensity;
			float _UseMask;
			half _TexContrast;
			float _TexAlpha;
			half _Texchoose_R;
			half _UseDissolve;
			half _Hardness;
			float _Usebackcolor;
			half _Usefresnel;
			half _Cutout;
			half _TexBright;
			half _Fresnel;
			float _ZTestMode;
			half _polar;
			half _ClipMode;
			half _CullMode;
			half _Effect_Blend;
			half _EdgeWidth;
			float _Particle;
			half _DiagonalUV;
			half _MainPannerX;
			half _MainPannerY;
			half _Dissolve;
			half _Rotator_Dissolve;
			half _Rotator;
			half _Use_Rotator;
			half _DistortPower;
			half _U_Distort_Speed;
			half _V_Distort_Speed;
			half _UseDistort;
			float _Tex_Desaturate;
			half _FadeLength;
			float _TessPhongStrength;
			float _TessValue;
			float _TessMin;
			float _TessMax;
			float _TessEdgeLength;
			float _TessMaxDisp;
			float _Noise_off;
			float _ExtrusionPoint;
			
			float _Stop;

			float _SoftParticlesNearFadeDistance;
			float _SoftParticlesFarFadeDistance;
			float _CameraNearFadeDistance;
			float _CameraFarFadeDistance;
			CBUFFER_END
			sampler2D _Man_Tex;
			sampler2D _Noise;
			sampler2D _Noise_niuqu;
			sampler2D _Mask;


			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord5.xyz = ase_worldNormal;
				
				o.ase_color = v.ase_color;
				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				o.ase_texcoord3 = v.ase_texcoord2;
				o.ase_texcoord4 = v.ase_texcoord1;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.zw = 0;
				o.ase_texcoord5.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = ( v.ase_normal * max( ( ( v.vertex.xyz.x + ( ( tex2Dlod( _Noise_niuqu, float4( ( ( _TimeParameters.x ) + v.ase_texcoord.xy ), 0, 0.0) ).r * _Noise_off ) - 1.0 ) ) * _ExtrusionPoint ) , 0.0 ) );
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				o.clipPos = TransformWorldToHClip( positionWS );
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord1 : TEXCOORD1;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_color = v.ase_color;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord2 = v.ase_texcoord2;
				o.ase_texcoord1 = v.ase_texcoord1;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_texcoord2 = patch[0].ase_texcoord2 * bary.x + patch[1].ase_texcoord2 * bary.y + patch[2].ase_texcoord2 * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 uv0_Man_Tex = IN.ase_texcoord2.xy * _Man_Tex_ST.xy + _Man_Tex_ST.zw;
				float4 uv2273 = IN.ase_texcoord3;
				uv2273.xy = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float4 appendResult274 = (float4(uv2273.x , uv2273.y , uv2273.z , uv2273.w));
				float2 appendResult518 = (float2(_UVpanner2.x , _UVpanner2.y));
				float2 panner521 = ( 1.0 * _Time.y * appendResult518 + float2( 0,0 ));
				float4 appendResult279 = (float4(1.0 , 1.0 , _EdgeWidth , panner521.x));
				float4 lerpResult275 = lerp( appendResult274 , appendResult279 , _Particle);
				float4 break280 = lerpResult275;
				float2 appendResult290 = (float2(break280.x , break280.y));
				half2 UV3399 = appendResult290;
				float2 temp_output_284_0 = ( UV3399 + float2( -1,-1 ) );
				float2 lerpResult427 = lerp( uv0_Man_Tex , abs( (uv0_Man_Tex*2.0 + -1.0) ) , _DiagonalUV);
				float4 uv1_Man_Tex = IN.ase_texcoord4;
				uv1_Man_Tex.xy = IN.ase_texcoord4.xy * _Man_Tex_ST.xy + _Man_Tex_ST.zw;
				float4 appendResult261 = (float4(uv1_Man_Tex.x , uv1_Man_Tex.y , uv1_Man_Tex.z , uv1_Man_Tex.w));
				float2 temp_cast_1 = (_Dissolve).xx;
				float temp_output_449_0 = (0.0 + (_Rotator_Dissolve - 0.0) * (1.038 - 0.0) / (1.0 - 0.0));
				float2 uv1430 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float cos432 = cos( _Rotator );
				float sin432 = sin( _Rotator );
				float2 rotator432 = mul( uv1430 - float2( 0.5,0.5 ) , float2x2( cos432 , -sin432 , sin432 , cos432 )) + float2( 0.5,0.5 );
				float2 temp_cast_2 = (5.0).xx;
				float2 lerpResult436 = lerp( temp_cast_1 , ( temp_output_449_0 * pow( ( temp_output_449_0 + ( temp_output_449_0 * rotator432 ) ) , temp_cast_2 ) ) , _Use_Rotator);
				//* pwrd ykk:时停
				float t = _TimeParameters.x * _Stop;
				float4 appendResult267 = (float4(( _MainPannerX * (t) ) , ( (t) * _MainPannerY ) , lerpResult436.x , _DistortPower));
				float4 lerpResult272 = lerp( appendResult261 , appendResult267 , _Particle);
				float4 break271 = lerpResult272;
				float2 appendResult282 = (float2(break271.x , break271.y));
				half2 UV2398 = appendResult282;
				float2 uv0_Noise = IN.ase_texcoord2.xy * _Noise_ST.xy + _Noise_ST.zw;
				float2 appendResult247 = (float2(_U_Distort_Speed , _V_Distort_Speed));
				half Distort_2405 = tex2D( _Noise, ( uv0_Noise + ( appendResult247 * (t) ) ) ).r;
				half Distort295 = break271.w;
				float lerpResult300 = lerp( 0.0 , ( ( Distort_2405 - 0.5 ) * Distort295 ) , _UseDistort);
				half Distort_1403 = lerpResult300;
				float2 UV_main482 = ( ( ( uv0_Man_Tex * temp_output_284_0 ) + ( temp_output_284_0 * float2( -0.5,-0.5 ) ) ) + ( ( lerpResult427 + UV2398 ) + Distort_1403 ) );
				float2 temp_output_458_0 = (UV_main482*2.0 + -1.0);
				float2 break460 = temp_output_458_0;
				float2 appendResult463 = (float2(length( temp_output_458_0 ) , (0.0 + (atan2( break460.y , break460.x ) - 0.0) * (1.0 - 0.0) / (3.141593 - 0.0))));
				float2 lerpResult464 = lerp( UV_main482 , appendResult463 , _polar);
				float2 Jzb477 = lerpResult464;
				float2 appendResult487 = (float2(_TlingOffset2.x , _TlingOffset2.y));
				half Bright303 = break280.w;
				float2 JZB494 = ( (Jzb477*appendResult487 + 0.0) + Bright303 );
				float4 tex2DNode189 = tex2D( _Man_Tex, JZB494 );
				float lerpResult190 = lerp( tex2DNode189.a , tex2DNode189.r , _Texchoose_R);
				float2 uv_Mask = IN.ase_texcoord2.xy * _Mask_ST.xy + _Mask_ST.zw;
				float lerpResult227 = lerp( 1.0 , tex2D( _Mask, uv_Mask ).r , _UseMask);
				float Mask395 = lerpResult227;
				float Alpha326 = _Main_color.a;
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 ase_worldNormal = IN.ase_texcoord5.xyz;
				float dotResult377 = dot( ase_worldViewDir , ase_worldNormal );
				float temp_output_380_0 = ( max( dotResult377 , 0.0 ) * _Fresnelintensity );
				float lerpResult383 = lerp( temp_output_380_0 , ( 1.0 - temp_output_380_0 ) , _FresnelFilp);
				float lerpResult391 = lerp( 1.0 , saturate( ( ( lerpResult383 - _Fresnel ) / ( 0.0 - _Fresnel ) ) ) , _Usefresnel);
				half Fresnel392 = lerpResult391;
				float2 temp_cast_4 = (1.0).xx;
				float temp_output_120_0 = ( Distort_2405 + 1.0 );
				float Dissolve294 = break271.z;
				half DissWidth296 = break280.z;
				float temp_output_186_0 = ( Dissolve294 * ( 1.0 + DissWidth296 ) );
				float temp_output_307_0 = (0.6 + (_Hardness - 0.0) * (0.99 - 0.6) / (1.0 - 0.0));
				half Hardness242 = temp_output_307_0;
				float temp_output_141_0 = ( 1.0 - Hardness242 );
				float2 appendResult216 = (float2(saturate( ( ( ( temp_output_120_0 - ( temp_output_186_0 * ( 1.0 + temp_output_141_0 ) ) ) - temp_output_307_0 ) / ( 1.0 - temp_output_307_0 ) ) ) , saturate( ( ( ( temp_output_120_0 - ( ( temp_output_186_0 - DissWidth296 ) * ( 1.0 + temp_output_141_0 ) ) ) - temp_output_307_0 ) / ( 1.0 - temp_output_307_0 ) ) )));
				float2 lerpResult414 = lerp( temp_cast_4 , appendResult216 , _UseDissolve);
				float2 break217 = lerpResult414;
				float temp_output_204_0 = ( IN.ase_color.a * lerpResult190 * _TexAlpha * Mask395 * Alpha326 * Fresnel392 * break217.y );
				
				float Alpha = temp_output_204_0;
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				return 0;
			}
			ENDHLSL
		}

	
	}
	CustomEditor "UnityEditor.ShaderGraph.PBRMasterGUI"
	Fallback "Hidden/InternalErrorShader"
	
}
/*ASEBEGIN
Version=18100
24;234;1920;1059;10225.08;626.6873;3.965416;True;True
Node;AmplifyShaderEditor.CommentaryNode;450;-8919.584,-703.7409;Inherit;False;2106.086;755.2009;Roator;13;430;447;432;449;441;442;444;443;438;269;437;436;431;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;447;-8884.399,-604.4169;Half;False;Property;_Rotator_Dissolve;定向溶解强度;36;0;Create;False;0;0;False;0;False;0.5602916;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;430;-8856.171,-352.5367;Inherit;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;431;-8859.519,-127.1448;Half;False;Property;_Rotator;定向溶解方向;37;0;Create;False;0;0;False;0;False;6.3;2.31;0;6.3;0;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;432;-8436.328,-202.54;Inherit;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode;449;-8584.789,-610.3353;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1.038;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;441;-8101.179,-262.4975;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;408;-7572.258,1024.134;Inherit;False;2564.721;2652.489;Noise;17;403;300;301;259;302;258;299;406;405;119;252;251;250;247;244;245;253;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;442;-7865.184,-325.904;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;444;-7916.429,-143.31;Float;False;Constant;_Float3;Float 3;37;0;Create;True;0;0;False;0;False;5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;443;-7698.435,-327.4783;Inherit;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;397;-6554.056,-998.1737;Inherit;False;1844.7;1560.713;Custom Vertex Streams  OR  Materal;30;277;279;276;303;294;296;282;399;288;290;295;280;271;275;274;272;261;267;281;273;264;263;270;268;265;266;262;517;518;521;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;244;-7384.312,1224.704;Half;False;Property;_U_Distort_Speed;U方向扰动速度;22;0;Create;False;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;245;-7381.942,1360.313;Half;False;Property;_V_Distort_Speed;V方向扰动速度;23;0;Create;False;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;265;-6498.662,-771.6641;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;517;-6519.458,328.9679;Half;False;Property;_UVpanner2;极坐标UV偏移速度;40;0;Create;False;0;0;False;0;False;0,0,0,0;1,0,-1,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;266;-6250.589,-478.4392;Half;False;Property;_MainPannerY;V方向贴图偏移;21;0;Create;False;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;247;-7117.228,1278.732;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TimeNode;250;-7172.103,1399.045;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;438;-7482.318,-210.7454;Half;False;Property;_Use_Rotator;开启定向溶解;35;1;[Toggle];Create;False;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;269;-7701.712,-701.0985;Half;True;Property;_Dissolve;溶解强度;26;0;Create;False;0;0;False;0;False;0.1924071;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;262;-6224.002,-754.9412;Half;False;Property;_MainPannerX;U方向贴图偏移;20;0;Create;False;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;437;-7547.413,-472.8355;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;253;-7347.238,1069.167;Inherit;False;0;119;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;270;-6221.479,-333.8674;Half;False;Property;_DistortPower;扰动强度;24;0;Create;False;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;518;-6257.439,361.3818;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;264;-6010.683,-700.6338;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;263;-6007.43,-561.0692;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;268;-6296.526,-960.1693;Inherit;False;1;189;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;251;-6860.685,1306.292;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;436;-7071.368,-631.9454;Inherit;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;267;-5751.49,-679.9666;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;281;-5912.912,-287.3784;Float;False;Property;_Particle;粒子模式或材质模式;0;1;[Enum];Create;False;2;Particle;0;Material;1;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;277;-6414.581,177.8425;Half;False;Property;_EdgeWidth;溶解宽度;28;0;Create;False;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;276;-6270.42,58.55854;Half;False;Constant;_UVRpeat;UVRpeat;24;0;Create;True;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;521;-6033.361,330.3162;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;273;-6348.944,-166.6655;Inherit;False;2;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;252;-6707.991,1235.352;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;261;-5813.387,-930.9711;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;274;-5927.328,-153.5945;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;272;-5568.75,-723.793;Inherit;True;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;119;-5890.603,1140.33;Inherit;True;Property;_Noise;扰动溶解贴图;18;0;Create;False;0;0;False;0;False;-1;65b2e12bfce69354aaffa609d9249bb6;65b2e12bfce69354aaffa609d9249bb6;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;279;-5763.468,91.01948;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;405;-5542.016,1164.373;Half;False;Distort_2;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;271;-5290.044,-726.6156;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.LerpOp;275;-5537.68,70.50603;Inherit;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.BreakToComponentsNode;280;-5341.945,64.06562;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RegisterLocalVarNode;295;-4961.742,-528.7687;Half;False;Distort;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;406;-7091.6,1545.149;Inherit;False;405;Distort_2;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;290;-5079.768,12.93427;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;258;-6859.14,1553.233;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;288;-4957.815,-356.7907;Inherit;True;0;189;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;299;-7076.639,1772.199;Inherit;False;295;Distort;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;282;-5037.383,-867.2945;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;424;-4711.55,-479.6385;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT;2;False;2;FLOAT;-1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;259;-6571.079,1547.409;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;411;-4533.438,-1008.426;Inherit;False;1513.438;1441.668;UV;15;401;400;284;292;285;286;291;287;283;404;426;427;428;289;482;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;302;-6534.273,1432.696;Half;False;Constant;_off;off;26;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;399;-4900.872,3.25147;Half;False;UV3;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;301;-6490.14,1717.127;Half;False;Property;_UseDistort;开启扰动;19;1;[Toggle];Create;False;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;398;-4832.688,-774.0287;Half;False;UV2;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.AbsOpNode;426;-4500.953,-347.5654;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;401;-4514.758,-601.9921;Inherit;False;399;UV3;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;300;-6324.919,1438.902;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;428;-4685.3,-65.24387;Half;False;Property;_DiagonalUV;开启对角贴图模式;10;1;[Toggle];Create;False;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;403;-6069.514,1431.939;Half;False;Distort_1;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;400;-4321.925,7.817873;Inherit;True;398;UV2;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;283;-4388.455,-969.6325;Inherit;True;0;189;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;427;-4394.893,-277.1262;Inherit;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;284;-4226.536,-594.2858;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;-1,-1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;286;-3924.113,-954.9189;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;404;-3862.312,295.9827;Inherit;False;403;Distort_1;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;285;-3872.663,-591.7634;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;-0.5,-0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;292;-3966.74,-0.3398559;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;287;-3630.9,-839.0114;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;291;-3691.69,-22.50852;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;289;-3439.511,-446.3502;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;511;-6696.639,-2204.165;Inherit;False;4029.417;1119.851;JZB;17;484;458;460;461;459;462;463;465;464;486;477;304;478;487;485;509;494;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;482;-3209.142,-452.6086;Inherit;True;UV_main;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;484;-6675.579,-1487.807;Inherit;True;482;UV_main;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;410;-4692.232,817.6569;Inherit;False;2232.948;1455.769;Dissolve;30;126;307;242;186;142;185;161;120;162;143;121;155;351;350;357;356;352;358;359;353;216;407;293;243;141;187;297;314;412;414;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;126;-3921.947,1627.85;Half;False;Property;_Hardness;溶解软硬强度;27;0;Create;False;0;0;False;0;False;0.6136052;0.4000002;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;458;-6308.482,-1932.052;Inherit;True;3;0;FLOAT2;0,0;False;1;FLOAT;2;False;2;FLOAT;-1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;409;-1773.173,2909.898;Inherit;False;2644.397;800.1641;Fresnel;18;375;376;377;378;379;380;381;382;383;384;386;385;387;389;390;388;391;392;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TFHCRemapNode;307;-3581.656,1634.758;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.6;False;4;FLOAT;0.99;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;460;-5982.252,-1719.56;Inherit;True;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RegisterLocalVarNode;296;-4983.244,148.0981;Half;False;DissWidth;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ATan2OpNode;461;-5706.066,-1702.927;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;242;-3202.127,1639.774;Half;False;Hardness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;294;-4884.552,-658.0953;Float;False;Dissolve;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;297;-4664.313,1913.867;Inherit;False;296;DissWidth;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;376;-1721.249,3038.589;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;375;-1723.173,3258.374;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;187;-4398.604,2001.441;Inherit;False;2;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LengthOpNode;459;-5610.376,-1948.24;Inherit;True;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;377;-1445.87,3089.333;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;243;-4672.144,1671.841;Inherit;False;242;Hardness;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;462;-5413.286,-1701.475;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;3.141593;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;293;-4520.355,1339.284;Inherit;False;294;Dissolve;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;465;-5251.723,-1223.561;Half;False;Property;_polar;开启极坐标;38;1;[Toggle];Create;False;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;186;-4242.042,1524.182;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;463;-5062.925,-1947.506;Inherit;True;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;378;-1194.988,3353.19;Half;False;Property;_Fresnelintensity;菲涅尔强度;34;0;Create;False;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;379;-1187.511,3089.324;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;141;-4465.223,1690.338;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;142;-4189.361,1660.584;Inherit;False;2;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;185;-3945.061,1868.816;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;161;-4083.619,2138.426;Inherit;False;2;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;380;-928.9192,3067.427;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;407;-4101.986,1261.179;Inherit;False;405;Distort_2;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;464;-4766.202,-1458.652;Inherit;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;162;-3808.594,2113.619;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;120;-3885.678,1242.732;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;486;-4100.481,-1750.699;Half;False;Property;_TlingOffset2;极坐标UV重复度;39;0;Create;False;0;0;False;0;False;1,1,0,0;3,2,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;382;-899.3552,3466.812;Half;False;Property;_FresnelFilp;菲涅尔翻转;33;1;[Toggle];Create;False;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;143;-3979.702,1459.787;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;477;-4382.568,-1456.686;Inherit;False;Jzb;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;381;-803.5994,3162.387;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;383;-518.9954,3121.481;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;478;-4032.149,-1960.224;Inherit;True;477;Jzb;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;155;-3545.085,1897.553;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;121;-3642.256,1244.306;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;487;-3858.367,-1730.497;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;303;-5017.657,307.7097;Half;False;Bright;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;384;-693.5752,3585.43;Half;False;Property;_Fresnel;菲涅尔轮廓大小;32;0;Create;False;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;386;-232.5222,3456.062;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;351;-3285.113,1331.464;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;350;-3167.498,1461.552;Inherit;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;357;-3099.656,1887.183;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;385;-287.4983,3138.457;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;485;-3630.171,-1734.764;Inherit;True;3;0;FLOAT2;0,0;False;1;FLOAT2;1,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;356;-3211.656,2044.182;Inherit;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;304;-3884.408,-1449.718;Inherit;False;303;Bright;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;396;-2390.247,1996.135;Inherit;False;989.2947;498.6267;Mask;5;231;229;230;227;395;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;358;-2969.656,2018.182;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;509;-3322.967,-1603.478;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;387;-32.77124,3155.549;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;352;-3047.582,1341.134;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;230;-2246.247,2139.153;Inherit;True;Property;_Mask;遮罩贴图;17;0;Create;False;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;231;-2185.23,2046.135;Float;False;Constant;_Float0;Float 0;17;0;Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;229;-2302.472,2378.761;Float;False;Property;_UseMask;使用遮罩;16;1;[Toggle];Create;False;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;89;816.0287,520.6448;Inherit;False;1202.567;673.0172;Two Sided Color;9;8;40;198;197;199;201;39;203;326;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;494;-2833.678,-1531.378;Inherit;False;JZB;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;359;-2856.061,1945.152;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;390;85.92676,3493.58;Half;False;Property;_Usefresnel;开启菲涅尔;31;1;[Toggle];Create;False;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;388;216.7566,3180.077;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;389;79.90967,2959.898;Half;False;Constant;_Fresnel_1;Fresnel_1;33;0;Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;353;-2906.154,1354.597;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;412;-3102.309,986.8421;Half;False;Constant;_Float2;Float 2;28;0;Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;227;-1832.027,2066.414;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;216;-2774.525,1449.407;Inherit;True;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;495;-2861.653,-572.732;Inherit;False;494;JZB;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;391;467.7557,3075.656;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;8;821.1118,585.769;Float;False;Property;_Main_color;主贴图颜色;6;1;[HDR];Create;False;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;314;-3195.048,1179.039;Half;False;Property;_UseDissolve;开启溶解;25;1;[Toggle];Create;False;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;326;1039.998,784.3749;Float;False;Alpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;392;647.2238,3076.307;Half;False;Fresnel;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;414;-2655.482,1126.587;Inherit;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;395;-1624.952,2067.792;Float;False;Mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;189;-2593.931,-604.3818;Inherit;True;Property;_Man_Tex;主贴图;9;0;Create;False;0;0;False;0;False;-1;54133254f9f5fe24f98a59fc2d0a4d2c;65b2e12bfce69354aaffa609d9249bb6;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;191;-2842.191,424.4478;Half;False;Property;_Texchoose_R;使用Alpha通道或者R通道;14;1;[Enum];Create;False;2;Alpha;0;R;1;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;190;-2523.408,379.1715;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;217;-2326.911,1127.456;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.GetLocalVarNode;393;-1827.36,1450.006;Inherit;False;392;Fresnel;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;324;-2034.236,1196.451;Inherit;False;326;Alpha;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;206;-2031.409,479.1264;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;225;-2327.727,881.7167;Float;False;Property;_TexAlpha;贴图Alpha强度;13;0;Create;False;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;394;-1973.187,1311.604;Inherit;False;395;Mask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;204;-1646.412,934.6755;Inherit;False;7;7;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;423;-4177,426.0503;Inherit;False;258.5127;389.9115;Enum;3;18;241;306;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;85;829.1014,1294.249;Inherit;False;1406.321;579.9919;Depth Fade;5;56;82;319;320;321;;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;202;-2166.904,329.7782;Inherit;False;203;Sidedcolor;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;198;1094.97,1068.806;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;235;-687.5457,523.6752;Inherit;False;FLOAT3;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ClipNode;238;-895.9811,523.8351;Inherit;False;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;311;21.93763,321.7029;Inherit;False;FLOAT4;4;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;56;879.1016,1615.241;Half;True;Property;_FadeLength;贴图深度衰退;30;0;Create;False;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;218;-1837.956,-198.2598;Half;False;Property;_Widthcolor;溶解宽度颜色;29;1;[HDR];Create;False;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;203;1798.229,694.3567;Float;False;Sidedcolor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;456;-521.6043,1267.607;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;234;-304.1651,322.0435;Inherit;True;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;236;-518.519,524.2219;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMinOpNode;239;-1317.683,614.9854;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;305;-1531.18,171.6351;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;240;-1183.343,1040.928;Half;False;Property;_ClipMode;裁切模式;4;1;[Enum];Create;False;2;on;1;off;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;224;-841.4728,319.407;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMinOpNode;233;-1033.982,704.5384;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FaceVariableNode;197;884.0804,1068.537;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;201;1112.278,946.6862;Float;False;Property;_Usebackcolor;开启背面颜色;7;1;[Toggle];Create;False;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;420;-2588.194,-965.5669;Float;True;Property;_Tex_Desaturate;贴图去色;15;0;Create;False;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;82;1182.843,1619.235;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;195;-1990.997,-46.95124;Inherit;False;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;321;1828.676,1589.285;Half;False;Fade;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;319;1413.99,1595.163;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-2041.388,220.2828;Half;False;Property;_TexBright;贴图亮度;12;0;Create;False;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;306;-4121.081,705.8974;Float;False;Property;_ZTestMode;深度模式;2;1;[Enum];Create;False;0;1;UnityEngine.Rendering.CompareFunction;True;0;False;4;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;194;-2295.259,37.32059;Half;False;Property;_TexContrast;贴图对比度;11;0;Create;False;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;322;22.01798,550.1909;Inherit;False;321;Fade;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;40;1331.381,804.4466;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;199;1553.575,705.847;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;181;-1350.899,317.7219;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;39;842.9215,874.2026;Float;False;Property;_Backcolor;背面贴图颜色;8;0;Create;False;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;18;-4114.032,492.7049;Half;False;Property;_CullMode;剔除模式;3;1;[Enum];Create;False;0;1;UnityEngine.Rendering.CullMode;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;241;-4110.016,597.8671;Half;False;Property;_Effect_Blend;混合模式;1;1;[Enum];Create;False;0;1;UnityEngine.Rendering.BlendMode;True;0;False;1;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-1742.332,366.6379;Inherit;False;4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;237;-1418.141,848.9739;Half;False;Property;_Cutout;裁切大小;5;0;Create;False;0;0;False;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DesaturateOpNode;429;-2174.469,-551.9377;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;467;-394.3598,1009.024;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;323;251.0554,321.567;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SaturateNode;320;1671.181,1594.547;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;452;451.1485,319.8601;Float;False;True;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;3;Fx/Effect_shader_jiuge;2992e84f91cbeb14eab234972e07ea9d;True;Forward;0;1;Forward;7;False;False;False;True;0;True;18;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;2;0;True;2;5;False;-1;10;True;241;0;1;False;-1;10;False;-1;False;False;False;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;2;True;240;True;3;True;306;True;False;0;False;-1;0;False;-1;True;1;LightMode=UniversalForward;False;0;Hidden/InternalErrorShader;0;0;Standard;21;Surface;1;  Blend;0;Two Sided;1;Cast Shadows;0;Receive Shadows;0;GPU Instancing;1;LOD CrossFade;0;Built-in Fog;0;Meta Pass;0;DOTS Instancing;0;Extra Pre Pass;0;Tessellation;0;  Phong;0;  Strength;0.5,False,-1;  Type;0;  Tess;16,False,-1;  Min;10,False,-1;  Max;25,False,-1;  Edge Length;16,False,-1;  Max Displacement;25,False,-1;Vertex Position,InvertActionOnDeselection;1;0;5;False;True;False;True;False;False;;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;453;785.5557,284.5424;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=ShadowCaster;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;455;785.5557,284.5424;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Meta;0;4;Meta;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;True;2;False;-1;False;False;False;False;False;True;1;LightMode=Meta;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;451;785.5557,284.5424;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;True;0;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;0;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;454;785.5557,284.5424;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthOnly;0;3;DepthOnly;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;False;True;False;False;False;False;0;False;-1;False;True;1;False;-1;False;False;True;1;LightMode=DepthOnly;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
WireConnection;432;0;430;0
WireConnection;432;2;431;0
WireConnection;449;0;447;0
WireConnection;441;0;449;0
WireConnection;441;1;432;0
WireConnection;442;0;449;0
WireConnection;442;1;441;0
WireConnection;443;0;442;0
WireConnection;443;1;444;0
WireConnection;247;0;244;0
WireConnection;247;1;245;0
WireConnection;437;0;449;0
WireConnection;437;1;443;0
WireConnection;518;0;517;1
WireConnection;518;1;517;2
WireConnection;264;0;262;0
WireConnection;264;1;265;2
WireConnection;263;0;265;2
WireConnection;263;1;266;0
WireConnection;251;0;247;0
WireConnection;251;1;250;2
WireConnection;436;0;269;0
WireConnection;436;1;437;0
WireConnection;436;2;438;0
WireConnection;267;0;264;0
WireConnection;267;1;263;0
WireConnection;267;2;436;0
WireConnection;267;3;270;0
WireConnection;521;2;518;0
WireConnection;252;0;253;0
WireConnection;252;1;251;0
WireConnection;261;0;268;1
WireConnection;261;1;268;2
WireConnection;261;2;268;3
WireConnection;261;3;268;4
WireConnection;274;0;273;1
WireConnection;274;1;273;2
WireConnection;274;2;273;3
WireConnection;274;3;273;4
WireConnection;272;0;261;0
WireConnection;272;1;267;0
WireConnection;272;2;281;0
WireConnection;119;1;252;0
WireConnection;279;0;276;0
WireConnection;279;1;276;0
WireConnection;279;2;277;0
WireConnection;279;3;521;0
WireConnection;405;0;119;1
WireConnection;271;0;272;0
WireConnection;275;0;274;0
WireConnection;275;1;279;0
WireConnection;275;2;281;0
WireConnection;280;0;275;0
WireConnection;295;0;271;3
WireConnection;290;0;280;0
WireConnection;290;1;280;1
WireConnection;258;0;406;0
WireConnection;282;0;271;0
WireConnection;282;1;271;1
WireConnection;424;0;288;0
WireConnection;259;0;258;0
WireConnection;259;1;299;0
WireConnection;399;0;290;0
WireConnection;398;0;282;0
WireConnection;426;0;424;0
WireConnection;300;0;302;0
WireConnection;300;1;259;0
WireConnection;300;2;301;0
WireConnection;403;0;300;0
WireConnection;427;0;288;0
WireConnection;427;1;426;0
WireConnection;427;2;428;0
WireConnection;284;0;401;0
WireConnection;286;0;283;0
WireConnection;286;1;284;0
WireConnection;285;0;284;0
WireConnection;292;0;427;0
WireConnection;292;1;400;0
WireConnection;287;0;286;0
WireConnection;287;1;285;0
WireConnection;291;0;292;0
WireConnection;291;1;404;0
WireConnection;289;0;287;0
WireConnection;289;1;291;0
WireConnection;482;0;289;0
WireConnection;458;0;484;0
WireConnection;307;0;126;0
WireConnection;460;0;458;0
WireConnection;296;0;280;2
WireConnection;461;0;460;1
WireConnection;461;1;460;0
WireConnection;242;0;307;0
WireConnection;294;0;271;2
WireConnection;187;1;297;0
WireConnection;459;0;458;0
WireConnection;377;0;376;0
WireConnection;377;1;375;0
WireConnection;462;0;461;0
WireConnection;186;0;293;0
WireConnection;186;1;187;0
WireConnection;463;0;459;0
WireConnection;463;1;462;0
WireConnection;379;0;377;0
WireConnection;141;0;243;0
WireConnection;142;1;141;0
WireConnection;185;0;186;0
WireConnection;185;1;297;0
WireConnection;161;1;141;0
WireConnection;380;0;379;0
WireConnection;380;1;378;0
WireConnection;464;0;484;0
WireConnection;464;1;463;0
WireConnection;464;2;465;0
WireConnection;162;0;185;0
WireConnection;162;1;161;0
WireConnection;120;0;407;0
WireConnection;143;0;186;0
WireConnection;143;1;142;0
WireConnection;477;0;464;0
WireConnection;381;0;380;0
WireConnection;383;0;380;0
WireConnection;383;1;381;0
WireConnection;383;2;382;0
WireConnection;155;0;120;0
WireConnection;155;1;162;0
WireConnection;121;0;120;0
WireConnection;121;1;143;0
WireConnection;487;0;486;1
WireConnection;487;1;486;2
WireConnection;303;0;280;3
WireConnection;386;1;384;0
WireConnection;351;0;121;0
WireConnection;351;1;307;0
WireConnection;350;1;307;0
WireConnection;357;0;155;0
WireConnection;357;1;307;0
WireConnection;385;0;383;0
WireConnection;385;1;384;0
WireConnection;485;0;478;0
WireConnection;485;1;487;0
WireConnection;356;1;307;0
WireConnection;358;0;357;0
WireConnection;358;1;356;0
WireConnection;509;0;485;0
WireConnection;509;1;304;0
WireConnection;387;0;385;0
WireConnection;387;1;386;0
WireConnection;352;0;351;0
WireConnection;352;1;350;0
WireConnection;494;0;509;0
WireConnection;359;0;358;0
WireConnection;388;0;387;0
WireConnection;353;0;352;0
WireConnection;227;0;231;0
WireConnection;227;1;230;1
WireConnection;227;2;229;0
WireConnection;216;0;353;0
WireConnection;216;1;359;0
WireConnection;391;0;389;0
WireConnection;391;1;388;0
WireConnection;391;2;390;0
WireConnection;326;0;8;4
WireConnection;392;0;391;0
WireConnection;414;0;412;0
WireConnection;414;1;216;0
WireConnection;414;2;314;0
WireConnection;395;0;227;0
WireConnection;189;1;495;0
WireConnection;190;0;189;4
WireConnection;190;1;189;1
WireConnection;190;2;191;0
WireConnection;217;0;414;0
WireConnection;204;0;206;4
WireConnection;204;1;190;0
WireConnection;204;2;225;0
WireConnection;204;3;394;0
WireConnection;204;4;324;0
WireConnection;204;5;393;0
WireConnection;204;6;217;1
WireConnection;198;0;197;0
WireConnection;235;0;238;0
WireConnection;238;0;181;0
WireConnection;238;1;239;0
WireConnection;238;2;233;0
WireConnection;311;0;234;0
WireConnection;203;0;199;0
WireConnection;456;0;204;0
WireConnection;234;0;224;0
WireConnection;234;1;236;0
WireConnection;234;2;467;0
WireConnection;236;0;235;0
WireConnection;239;0;204;0
WireConnection;305;0;218;0
WireConnection;305;1;14;0
WireConnection;224;0;181;0
WireConnection;224;3;239;0
WireConnection;233;0;237;0
WireConnection;233;1;240;0
WireConnection;82;0;56;0
WireConnection;195;0;429;0
WireConnection;195;1;194;0
WireConnection;321;0;320;0
WireConnection;319;0;82;0
WireConnection;40;0;8;0
WireConnection;40;1;39;0
WireConnection;40;2;201;0
WireConnection;199;0;40;0
WireConnection;199;1;8;0
WireConnection;199;2;198;0
WireConnection;181;0;305;0
WireConnection;181;1;14;0
WireConnection;181;2;217;0
WireConnection;14;0;195;0
WireConnection;14;1;31;0
WireConnection;14;2;202;0
WireConnection;14;3;206;0
WireConnection;429;0;189;0
WireConnection;429;1;420;0
WireConnection;467;0;240;0
WireConnection;323;0;311;0
WireConnection;323;1;322;0
WireConnection;320;0;319;0
WireConnection;452;2;323;0
WireConnection;452;3;456;0
ASEEND*/
//CHKSM=1E249B27FCE98BE393008172E0C610AC31AFA050