Shader "Custom/Blackhole0D" {
    Properties {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _BlackholePosition("Blackhole Position", Vector) = (0,0,0,0)
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
        float4 _BlackholePosition;
        float _BlackholeStrength;

        void vert(inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            // Transform vertex position to world space
            float4 worldPos = mul(unity_ObjectToWorld, v.vertex);

            // Calculate the vector from the vertex to the black hole position in world space
            float3 toBlackhole = _BlackholePosition - worldPos.xyz;
            float distance = length(toBlackhole);

            // Normalize the direction and apply distortion based on inverse square of distance
            float3 direction = normalize(toBlackhole);
            float distortion;
            if (distance > 0) {
                distortion = _BlackholeStrength / (distance);
                distortion = max(distortion, 0); // Avoid negative distortion
            } else {
                distortion = 0; // Avoid division by zero
            }

            // If, as a result of distorsion, vertex position goes over the black hole position, then just set it to the black hole position
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
