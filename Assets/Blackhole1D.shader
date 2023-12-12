Shader "Custom/Blackhole1DShader" {
    Properties {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _BlackholeStartPosition("Blackhole Start Position", Vector) = (0,0,0,0)
        _BlackholeEndPosition("Blackhole End Position", Vector) = (1,0,0,0)
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
        float4 _BlackholeStartPosition;
        float4 _BlackholeEndPosition;
        float _BlackholeStrength;

        void vert(inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            // Transform vertex position to world space
            float4 worldPos = mul(unity_ObjectToWorld, v.vertex);

            // Calculate the vector from the vertex to the line defined by start and end positions
            float3 lineDir = normalize(_BlackholeEndPosition - _BlackholeStartPosition);
            float3 vLine = worldPos.xyz - _BlackholeStartPosition.xyz;
            float t = max(0, min(1, dot(vLine, lineDir)));
            float3 closestPoint = _BlackholeStartPosition.xyz + t * lineDir;
            float3 toLine = worldPos.xyz - closestPoint;
            float distance = length(toLine);

            // Normalize the direction and apply distortion based on inverse square of distance
            float3 direction = normalize(toLine);
            float distortion = _BlackholeStrength / (distance * distance);
            distortion = max(distortion, 0); // Avoid negative distortion

            // If, as a result of distortion, vertex position goes over the line, then set it to the closest point on the line
            if (distortion > distance) {
                distortion = distance;
            }

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
