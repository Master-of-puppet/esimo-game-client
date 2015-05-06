// Unlit
// Supports vertex color
// No lightmap



Shader "GUI/Text Detail" {
    Properties

    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB) Trans (A)", 2D ) = "white" {}
        _DetailTex ("Detail (RGB)" , 2D  ) = "white" {}
        
        
    }
    SubShader

    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha 
		
        Pass

        {

            CGPROGRAM
                #include "UnityCG.cginc"
                #pragma vertex vert
                #pragma fragment frag

                
                
                struct v2f
                {
                    fixed4 color : COLOR;
                    fixed4 pos : SV_POSITION;
                    fixed2 pack0 : TEXCOORD0;
                    fixed2 pack1 : TEXCOORD1;
                };
                
                sampler2D _MainTex;
                sampler2D _DetailTex;
                fixed4 _MainTex_ST;
                fixed4 _DetailTex_ST;
                fixed4 _Color;
                fixed _TextureScale;
               	fixed _yOffset;
               	fixed _xOffset;
                

                v2f vert(appdata_full v)

                {
                    v2f o;
                    o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                    o.pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                    o.pack1.xy = TRANSFORM_TEX(v.texcoord1, _DetailTex);
                    o.color = v.color;
                    
                    return o;

                }
                fixed4 frag(v2f i) : COLOR
                {

                    fixed4 tex = tex2D(_MainTex, i.pack0);
                    fixed4 detailTex = tex2D(_DetailTex, i.pack1);
                    fixed4 c = i.color *_Color * detailTex;
                    c.a = i.color.a *_Color.a * tex.a * c.a;
                    return c;

                }

            ENDCG

        }

    }

}