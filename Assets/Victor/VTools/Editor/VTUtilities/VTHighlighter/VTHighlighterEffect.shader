Shader "VTools/Editor/VTHighlighterEffect"
{
    Properties
    {
	    _MainTex("", 2D) = "white" {}
	    _Color ("Color", Color) = (1,1,1,1)
	    _ChildColor("Child Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Pass
        {
		    Cull off
		    ZWrite off

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
                float4 vertex : SV_POSITION;
			    float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
			    o.uv = v.uv;
                return o;
            }

		    fixed4 _Color;
		    fixed4 _ChildColor;

		    sampler2D _MainTex;
		    sampler2D _VTHighlightMask;

            fixed4 frag (v2f i) : SV_Target
            {
			    fixed4 main = tex2D(_MainTex, i.uv);
			    fixed4 mask = tex2D(_VTHighlightMask, i.uv);
			    fixed amount = mask.a;
			    fixed root = mask.r;
			    fixed4 maskColor = lerp(_ChildColor, _Color, root);
			    fixed4 masked = fixed4(lerp(main.rgb, maskColor.rgb, maskColor.a), 1);
			    return lerp(main, masked, amount);
            }

            ENDCG
        }
    }
}
