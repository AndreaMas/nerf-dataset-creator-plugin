Shader "Hidden/Shader_Normal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
			sampler2D _CameraDepthNormalsTexture;

			float4x4 UNITY_MATRIX_IV; //inverse matrix form camera
			uniform float chooseDorN;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                
				//assign normals to floatXYZ, depth to floatW
				float4 normalDepth;

				//decode depth normal maps: (input texture, out depth, out normals)
				DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, i.uv), normalDepth.w, normalDepth.xyz);

				// apply normals
				float3 worldNormal = mul(UNITY_MATRIX_IV, float4(normalDepth.xyz, 0)).xyz;
				col.rgb = worldNormal;

				if (normalDepth.w >= 1.0) col.rgb = 0;


                return col;
            }
            ENDCG
        }
    }
}
