Shader "Custom/Blackhole2DShader" {
    Properties {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _SurfaceCenter("Surface Center", Vector) = (0,0,0,0)
        _SurfaceNormal("Surface Normal", Vector) = (0,1,0,0)
        _BlackholeStrength("Blackhole Distortion Strength", Float) = 30.0
    }

    SubShader {
        Tags { "RenderType" = "Opaque" }

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert

        sampler2D _MainTex;

        struct Input {
            float2 uv_MainTex;
        };

        fixed4 _Color;
        float4 _SurfaceCenter;
        float4 _SurfaceNormal;
        float _BlackholeStrength;

        void vert(inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            // Transform vertex position to world space
            float4 worldPos = mul(unity_ObjectToWorld, v.vertex);

            // Calculate the shortest distance from the vertex to the plane
            float3 toSurface = worldPos.xyz - _SurfaceCenter.xyz;
            float distanceToPlane = dot(toSurface, _SurfaceNormal.xyz);

            // Normalize the direction and apply distortion based on inverse square of distance
            float3 direction = normalize(_SurfaceNormal.xyz);
            float distortion = _BlackholeStrength / (distanceToPlane * distanceToPlane);
            distortion = max(distortion, 0); // Avoid negative distortion

            // Apply the distortion to the vertex position in world space
            worldPos.xyz += distortion * direction;

            // Transform the vertex position back to object space
            v.vertex = mul(unity_WorldToObject, worldPos);
        }

        void surf(Input IN, inout SurfaceOutputStandard o) {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }

    FallBack "Diffuse"
}
