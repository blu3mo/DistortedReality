Shader "Custom/Blackhole0D" {
    Properties {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        //_BlackholePosition("Blackhole Position", Vector) = (0,0,0,0)
        //_BlackholeStrength("Blackhole Distortion Strength", Float) = 30.0
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
        float _BlackholeStrength[100];
        // fix: should get blackhole positions array instead. make float array for x, y, z positions 
        float _BlackholePositionX[100];
        float _BlackholePositionY[100];
        float _BlackholePositionZ[100];
        float _BlackholeRotationX[100];
        float _BlackholeRotationY[100];
        float _BlackholeRotationZ[100];

        void vert(inout appdata_full v, out Input o) {
    UNITY_INITIALIZE_OUTPUT(Input, o);

    // Transform vertex position to world space
    float4 originalWorldPos = mul(unity_ObjectToWorld, v.vertex);
    float4 rotatedWorldPos = mul(unity_ObjectToWorld, v.vertex);

    for (int i = 0; i < 100; i++) {
        // Calculate the vector from the vertex to the black hole position in world space
        float3 _BlackholePosition = float3(_BlackholePositionX[i], _BlackholePositionY[i], _BlackholePositionZ[i]);
        float3 toBlackhole = _BlackholePosition - originalWorldPos.xyz;
        float distance = length(toBlackhole);

        // Rotation logic
        float rotationAngleX = 0.4 * _BlackholeRotationX[i] / (distance);
        float rotationAngleY = 0.4 * _BlackholeRotationY[i] / (distance);
        float rotationAngleZ = 0.4 * _BlackholeRotationZ[i] / (distance);

        float4x4 rotationMatrixX = float4x4(1, 0, 0, 0,
                                            0, cos(rotationAngleX), -sin(rotationAngleX), 0,
                                            0, sin(rotationAngleX), cos(rotationAngleX), 0,
                                            0, 0, 0, 1);

        float4x4 rotationMatrixY = float4x4(cos(rotationAngleY), 0, sin(rotationAngleY), 0,
                                            0, 1, 0, 0,
                                            -sin(rotationAngleY), 0, cos(rotationAngleY), 0,
                                            0, 0, 0, 1);

        float4x4 rotationMatrixZ = float4x4(cos(rotationAngleZ), -sin(rotationAngleZ), 0, 0,
                                            sin(rotationAngleZ), cos(rotationAngleZ), 0, 0,
                                            0, 0, 1, 0,
                                            0, 0, 0, 1);

        rotatedWorldPos = mul(rotationMatrixX, rotatedWorldPos);
        rotatedWorldPos = mul(rotationMatrixY, rotatedWorldPos);
        rotatedWorldPos = mul(rotationMatrixZ, rotatedWorldPos);
    }
    float4 squeezedWorldPos = rotatedWorldPos;

    for (int i = 0; i < 100; i++) {
        float3 _BlackholePosition = float3(_BlackholePositionX[i], _BlackholePositionY[i], _BlackholePositionZ[i]);
        float3 toBlackhole = _BlackholePosition - rotatedWorldPos.xyz;
        float distance = length(toBlackhole);

        // Normalize the direction and apply distortion based on inverse square of distance
        float3 direction = normalize(toBlackhole);
        float distortion;
        if (distance != 0) {
            distortion = _BlackholeStrength[i] / (distance * distance);
            distortion = max(distortion, 0); // Avoid negative distortion
        } else {
            distortion = 0; // Avoid division by zero
        }

        // If, as a result of distortion, vertex position goes over the black hole position, then just set it to the black hole position
        if (distortion > distance) {
            distortion = distance;
        }

        // Apply the distortion to the vertex position in world space
        squeezedWorldPos.xyz += distortion * direction;
    }

        // Transform the vertex position back to object space
        v.vertex = mul(unity_WorldToObject, squeezedWorldPos);
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
