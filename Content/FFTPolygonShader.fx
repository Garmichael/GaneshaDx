#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;

float4 AmbientColor = float4(1, 1, 1, 1);

float3 DirectionalLightDirection0 = float3(1, 0, 0);
float4 DirectionalLightColor0 = float4(1, 1, 1, 1);
float4 DirectionalLightBoost0 = float4(1, 1, 1, 1);

float3 DirectionalLightDirection1 = float3(1, 0, 0);
float4 DirectionalLightColor1 = float4(1, 1, 1, 1);
float4 DirectionalLightBoost1 = float4(1, 1, 1, 1);

float3 DirectionalLightDirection2 = float3(1, 0, 0);
float4 DirectionalLightColor2 = float4(1, 1, 1, 1);
float4 DirectionalLightBoost2 = float4(1, 1, 1, 1);

float MaxAlpha = 1;

texture ModelTexture;
sampler2D textureSampler = sampler_state {
    Texture = (ModelTexture);
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

float4 PaletteColors[16];

bool HighlightBright;
bool HighlightDim;

bool UsesAnimatedUv0 = false;
bool UsesAnimatedUv1 = false;
bool UsesAnimatedUv2 = false;
bool UsesAnimatedUv3 = false;
bool UsesAnimatedUv4 = false;
bool UsesAnimatedUv5 = false;
bool UsesAnimatedUv6 = false;
bool UsesAnimatedUv7 = false;
bool UsesAnimatedUv8 = false;
bool UsesAnimatedUv9 = false;
bool UsesAnimatedUv10 = false;
bool UsesAnimatedUv11 = false;
bool UsesAnimatedUv12 = false;
bool UsesAnimatedUv13 = false;
bool UsesAnimatedUv14 = false;
bool UsesAnimatedUv15 = false;
bool UsesAnimatedUv16 = false;
bool UsesAnimatedUv17 = false;
bool UsesAnimatedUv18 = false;
bool UsesAnimatedUv19 = false;
bool UsesAnimatedUv20 = false;
bool UsesAnimatedUv21 = false;
bool UsesAnimatedUv22 = false;
bool UsesAnimatedUv23 = false;
bool UsesAnimatedUv24 = false;
bool UsesAnimatedUv25 = false;
bool UsesAnimatedUv26 = false;
bool UsesAnimatedUv27 = false;
bool UsesAnimatedUv28 = false;
bool UsesAnimatedUv29 = false;
bool UsesAnimatedUv30 = false;
bool UsesAnimatedUv31 = false;

float4 AnimatedUvCanvas0;
float4 AnimatedUvCanvas1;
float4 AnimatedUvCanvas2;
float4 AnimatedUvCanvas3;
float4 AnimatedUvCanvas4;
float4 AnimatedUvCanvas5;
float4 AnimatedUvCanvas6;
float4 AnimatedUvCanvas7;
float4 AnimatedUvCanvas8;
float4 AnimatedUvCanvas9;
float4 AnimatedUvCanvas10;
float4 AnimatedUvCanvas11;
float4 AnimatedUvCanvas12;
float4 AnimatedUvCanvas13;
float4 AnimatedUvCanvas14;
float4 AnimatedUvCanvas15;
float4 AnimatedUvCanvas16;
float4 AnimatedUvCanvas17;
float4 AnimatedUvCanvas18;
float4 AnimatedUvCanvas19;
float4 AnimatedUvCanvas20;
float4 AnimatedUvCanvas21;
float4 AnimatedUvCanvas22;
float4 AnimatedUvCanvas23;
float4 AnimatedUvCanvas24;
float4 AnimatedUvCanvas25;
float4 AnimatedUvCanvas26;
float4 AnimatedUvCanvas27;
float4 AnimatedUvCanvas28;
float4 AnimatedUvCanvas29;
float4 AnimatedUvCanvas30;
float4 AnimatedUvCanvas31;

float4 AnimatedUvSource0;
float4 AnimatedUvSource1;
float4 AnimatedUvSource2;
float4 AnimatedUvSource3;
float4 AnimatedUvSource4;
float4 AnimatedUvSource5;
float4 AnimatedUvSource6;
float4 AnimatedUvSource7;
float4 AnimatedUvSource8;
float4 AnimatedUvSource9;
float4 AnimatedUvSource10;
float4 AnimatedUvSource11;
float4 AnimatedUvSource12;
float4 AnimatedUvSource13;
float4 AnimatedUvSource14;
float4 AnimatedUvSource15;
float4 AnimatedUvSource16;
float4 AnimatedUvSource17;
float4 AnimatedUvSource18;
float4 AnimatedUvSource19;
float4 AnimatedUvSource20;
float4 AnimatedUvSource21;
float4 AnimatedUvSource22;
float4 AnimatedUvSource23;
float4 AnimatedUvSource24;
float4 AnimatedUvSource25;
float4 AnimatedUvSource26;
float4 AnimatedUvSource27;
float4 AnimatedUvSource28;
float4 AnimatedUvSource29;
float4 AnimatedUvSource30;
float4 AnimatedUvSource31;

struct AppToVertex { 
    float4 Position : POSITION0;
    float4 Normal : NORMAL0;
    float2 TextureCoordinate : TEXCOORD0;
};

struct VertexToPixel {
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float3 Normal : TEXCOORD0;
    float2 TextureCoordinate : TEXCOORD1;
};

float4 GetTextureColor(sampler2D textureSampler, float2 textureCoordinate){
    float4 originalColor = tex2D(textureSampler, textureCoordinate);

    if (UsesAnimatedUv0 &&
        textureCoordinate.x >= AnimatedUvCanvas0.x && 
        textureCoordinate.x <= AnimatedUvCanvas0.z &&
        textureCoordinate.y >= AnimatedUvCanvas0.y &&
        textureCoordinate.y <= AnimatedUvCanvas0.w
    ){
        float2 offset = float2(textureCoordinate.x - AnimatedUvCanvas0.x, textureCoordinate.y - AnimatedUvCanvas0.y);
        originalColor = tex2D(textureSampler, AnimatedUvSource0 + offset);
    }

    if (UsesAnimatedUv1 &&
        textureCoordinate.x >= AnimatedUvCanvas1.x && 
        textureCoordinate.x <= AnimatedUvCanvas1.z &&
        textureCoordinate.y >= AnimatedUvCanvas1.y &&
        textureCoordinate.y <= AnimatedUvCanvas1.w
    ){
        float2 offset = float2(textureCoordinate.x - AnimatedUvCanvas1.x, textureCoordinate.y - AnimatedUvCanvas1.y);
        originalColor = tex2D(textureSampler, AnimatedUvSource1 + offset);
    }

    if (UsesAnimatedUv2 &&
        textureCoordinate.x >= AnimatedUvCanvas2.x && 
        textureCoordinate.x <= AnimatedUvCanvas2.z &&
        textureCoordinate.y >= AnimatedUvCanvas2.y &&
        textureCoordinate.y <= AnimatedUvCanvas2.w
    ){
        float2 offset = float2(textureCoordinate.x - AnimatedUvCanvas2.x, textureCoordinate.y - AnimatedUvCanvas2.y);
        originalColor = tex2D(textureSampler, AnimatedUvSource2 + offset);
    }

    if (UsesAnimatedUv3 &&
        textureCoordinate.x >= AnimatedUvCanvas3.x && 
        textureCoordinate.x <= AnimatedUvCanvas3.z &&
        textureCoordinate.y >= AnimatedUvCanvas3.y &&
        textureCoordinate.y <= AnimatedUvCanvas3.w
    ){
        float2 offset = float2(textureCoordinate.x - AnimatedUvCanvas3.x, textureCoordinate.y - AnimatedUvCanvas3.y);
        originalColor = tex2D(textureSampler, AnimatedUvSource3 + offset);
    }

    if (UsesAnimatedUv4 &&
        textureCoordinate.x >= AnimatedUvCanvas4.x && 
        textureCoordinate.x <= AnimatedUvCanvas4.z &&
        textureCoordinate.y >= AnimatedUvCanvas4.y &&
        textureCoordinate.y <= AnimatedUvCanvas4.w
    ){
        float2 offset = float2(textureCoordinate.x - AnimatedUvCanvas4.x, textureCoordinate.y - AnimatedUvCanvas4.y);
        originalColor = tex2D(textureSampler, AnimatedUvSource4 + offset);
    }

    if (UsesAnimatedUv5 &&
        textureCoordinate.x >= AnimatedUvCanvas5.x && 
        textureCoordinate.x <= AnimatedUvCanvas5.z &&
        textureCoordinate.y >= AnimatedUvCanvas5.y &&
        textureCoordinate.y <= AnimatedUvCanvas5.w
    ){
        float2 offset = float2(textureCoordinate.x - AnimatedUvCanvas5.x, textureCoordinate.y - AnimatedUvCanvas5.y);
        originalColor = tex2D(textureSampler, AnimatedUvSource5 + offset);
    }

    if (UsesAnimatedUv6 &&
        textureCoordinate.x >= AnimatedUvCanvas6.x && 
        textureCoordinate.x <= AnimatedUvCanvas6.z &&
        textureCoordinate.y >= AnimatedUvCanvas6.y &&
        textureCoordinate.y <= AnimatedUvCanvas6.w
    ){
        float2 offset = float2(textureCoordinate.x - AnimatedUvCanvas6.x, textureCoordinate.y - AnimatedUvCanvas6.y);
        originalColor = tex2D(textureSampler, AnimatedUvSource6 + offset);
    }

    if (UsesAnimatedUv7 &&
        textureCoordinate.x >= AnimatedUvCanvas7.x && 
        textureCoordinate.x <= AnimatedUvCanvas7.z &&
        textureCoordinate.y >= AnimatedUvCanvas7.y &&
        textureCoordinate.y <= AnimatedUvCanvas7.w
    ){
        float2 offset = float2(textureCoordinate.x - AnimatedUvCanvas7.x, textureCoordinate.y - AnimatedUvCanvas7.y);
        originalColor = tex2D(textureSampler, AnimatedUvSource7 + offset);
    }

    if (UsesAnimatedUv8 &&
        textureCoordinate.x >= AnimatedUvCanvas8.x && 
        textureCoordinate.x <= AnimatedUvCanvas8.z &&
        textureCoordinate.y >= AnimatedUvCanvas8.y &&
        textureCoordinate.y <= AnimatedUvCanvas8.w
    ){
        float2 offset = float2(textureCoordinate.x - AnimatedUvCanvas8.x, textureCoordinate.y - AnimatedUvCanvas8.y);
        originalColor = tex2D(textureSampler, AnimatedUvSource8 + offset);
    }

    if (UsesAnimatedUv9 &&
        textureCoordinate.x >= AnimatedUvCanvas9.x && 
        textureCoordinate.x <= AnimatedUvCanvas9.z &&
        textureCoordinate.y >= AnimatedUvCanvas9.y &&
        textureCoordinate.y <= AnimatedUvCanvas9.w
    ){
        float2 offset = float2(textureCoordinate.x - AnimatedUvCanvas9.x, textureCoordinate.y - AnimatedUvCanvas9.y);
        originalColor = tex2D(textureSampler, AnimatedUvSource9 + offset);
    }

    if (UsesAnimatedUv10 &&
        textureCoordinate.x >= AnimatedUvCanvas10.x && 
        textureCoordinate.x <= AnimatedUvCanvas10.z &&
        textureCoordinate.y >= AnimatedUvCanvas10.y &&
        textureCoordinate.y <= AnimatedUvCanvas10.w
    ){
        float2 offset = float2(textureCoordinate.x - AnimatedUvCanvas10.x, textureCoordinate.y - AnimatedUvCanvas10.y);
        originalColor = tex2D(textureSampler, AnimatedUvSource10 + offset);
    }

    if (UsesAnimatedUv11 &&
        textureCoordinate.x >= AnimatedUvCanvas11.x && 
        textureCoordinate.x <= AnimatedUvCanvas11.z &&
        textureCoordinate.y >= AnimatedUvCanvas11.y &&
        textureCoordinate.y <= AnimatedUvCanvas11.w
    ){
        float2 offset = float2(textureCoordinate.x - AnimatedUvCanvas11.x, textureCoordinate.y - AnimatedUvCanvas11.y);
        originalColor = tex2D(textureSampler, AnimatedUvSource11 + offset);
    }

    if (UsesAnimatedUv12 &&
        textureCoordinate.x >= AnimatedUvCanvas12.x && 
        textureCoordinate.x <= AnimatedUvCanvas12.z &&
        textureCoordinate.y >= AnimatedUvCanvas12.y &&
        textureCoordinate.y <= AnimatedUvCanvas12.w
    ){
        float2 offset = float2(textureCoordinate.x - AnimatedUvCanvas12.x, textureCoordinate.y - AnimatedUvCanvas12.y);
        originalColor = tex2D(textureSampler, AnimatedUvSource12 + offset);
    }

    if (UsesAnimatedUv13 &&
        textureCoordinate.x >= AnimatedUvCanvas13.x && 
        textureCoordinate.x <= AnimatedUvCanvas13.z &&
        textureCoordinate.y >= AnimatedUvCanvas13.y &&
        textureCoordinate.y <= AnimatedUvCanvas13.w
    ){
        float2 offset = float2(textureCoordinate.x - AnimatedUvCanvas13.x, textureCoordinate.y - AnimatedUvCanvas13.y);
        originalColor = tex2D(textureSampler, AnimatedUvSource13 + offset);
    }

    if (UsesAnimatedUv14 &&
        textureCoordinate.x >= AnimatedUvCanvas14.x && 
        textureCoordinate.x <= AnimatedUvCanvas14.z &&
        textureCoordinate.y >= AnimatedUvCanvas14.y &&
        textureCoordinate.y <= AnimatedUvCanvas14.w
    ){
        float2 offset = float2(textureCoordinate.x - AnimatedUvCanvas14.x, textureCoordinate.y - AnimatedUvCanvas14.y);
        originalColor = tex2D(textureSampler, AnimatedUvSource14 + offset);
    }

    if (UsesAnimatedUv15 &&
        textureCoordinate.x >= AnimatedUvCanvas15.x && 
        textureCoordinate.x <= AnimatedUvCanvas15.z &&
        textureCoordinate.y >= AnimatedUvCanvas15.y &&
        textureCoordinate.y <= AnimatedUvCanvas15.w
    ){
        float2 offset = float2(textureCoordinate.x - AnimatedUvCanvas15.x, textureCoordinate.y - AnimatedUvCanvas15.y);
        originalColor = tex2D(textureSampler, AnimatedUvSource15 + offset);
    }

    if (UsesAnimatedUv16 &&
        textureCoordinate.x >= AnimatedUvCanvas16.x && 
        textureCoordinate.x <= AnimatedUvCanvas16.z &&
        textureCoordinate.y >= AnimatedUvCanvas16.y &&
        textureCoordinate.y <= AnimatedUvCanvas16.w
    ){
        float2 offset = float2(textureCoordinate.x - AnimatedUvCanvas16.x, textureCoordinate.y - AnimatedUvCanvas16.y);
        originalColor = tex2D(textureSampler, AnimatedUvSource16 + offset);
    }

    if (UsesAnimatedUv17 &&
        textureCoordinate.x >= AnimatedUvCanvas17.x && 
        textureCoordinate.x <= AnimatedUvCanvas17.z &&
        textureCoordinate.y >= AnimatedUvCanvas17.y &&
        textureCoordinate.y <= AnimatedUvCanvas17.w
    ){
        float2 offset = float2(textureCoordinate.x - AnimatedUvCanvas17.x, textureCoordinate.y - AnimatedUvCanvas17.y);
        originalColor = tex2D(textureSampler, AnimatedUvSource17 + offset);
    }

    if (UsesAnimatedUv18 &&
        textureCoordinate.x >= AnimatedUvCanvas18.x && 
        textureCoordinate.x <= AnimatedUvCanvas18.z &&
        textureCoordinate.y >= AnimatedUvCanvas18.y &&
        textureCoordinate.y <= AnimatedUvCanvas18.w
    ){
        float2 offset = float2(textureCoordinate.x - AnimatedUvCanvas18.x, textureCoordinate.y - AnimatedUvCanvas18.y);
        originalColor = tex2D(textureSampler, AnimatedUvSource18 + offset);
    }

    if (UsesAnimatedUv19 &&
        textureCoordinate.x >= AnimatedUvCanvas19.x && 
        textureCoordinate.x <= AnimatedUvCanvas19.z &&
        textureCoordinate.y >= AnimatedUvCanvas19.y &&
        textureCoordinate.y <= AnimatedUvCanvas19.w
    ){
        float2 offset = float2(textureCoordinate.x - AnimatedUvCanvas19.x, textureCoordinate.y - AnimatedUvCanvas19.y);
        originalColor = tex2D(textureSampler, AnimatedUvSource19 + offset);
    }

    if (UsesAnimatedUv20 &&
        textureCoordinate.x >= AnimatedUvCanvas20.x && 
        textureCoordinate.x <= AnimatedUvCanvas20.z &&
        textureCoordinate.y >= AnimatedUvCanvas20.y &&
        textureCoordinate.y <= AnimatedUvCanvas20.w
    ){
        float2 offset = float2(textureCoordinate.x - AnimatedUvCanvas20.x, textureCoordinate.y - AnimatedUvCanvas20.y);
        originalColor = tex2D(textureSampler, AnimatedUvSource20 + offset);
    }

    if (UsesAnimatedUv21 &&
        textureCoordinate.x >= AnimatedUvCanvas21.x && 
        textureCoordinate.x <= AnimatedUvCanvas21.z &&
        textureCoordinate.y >= AnimatedUvCanvas21.y &&
        textureCoordinate.y <= AnimatedUvCanvas21.w
    ){
        float2 offset = float2(textureCoordinate.x - AnimatedUvCanvas21.x, textureCoordinate.y - AnimatedUvCanvas21.y);
        originalColor = tex2D(textureSampler, AnimatedUvSource21 + offset);
    }

    if (UsesAnimatedUv22 &&
        textureCoordinate.x >= AnimatedUvCanvas22.x && 
        textureCoordinate.x <= AnimatedUvCanvas22.z &&
        textureCoordinate.y >= AnimatedUvCanvas22.y &&
        textureCoordinate.y <= AnimatedUvCanvas22.w
    ){
        float2 offset = float2(textureCoordinate.x - AnimatedUvCanvas22.x, textureCoordinate.y - AnimatedUvCanvas22.y);
        originalColor = tex2D(textureSampler, AnimatedUvSource22 + offset);
    }

    if (UsesAnimatedUv23 &&
        textureCoordinate.x >= AnimatedUvCanvas23.x && 
        textureCoordinate.x <= AnimatedUvCanvas23.z &&
        textureCoordinate.y >= AnimatedUvCanvas23.y &&
        textureCoordinate.y <= AnimatedUvCanvas23.w
    ){
        float2 offset = float2(textureCoordinate.x - AnimatedUvCanvas23.x, textureCoordinate.y - AnimatedUvCanvas23.y);
        originalColor = tex2D(textureSampler, AnimatedUvSource23 + offset);
    }

    if (UsesAnimatedUv24 &&
        textureCoordinate.x >= AnimatedUvCanvas24.x && 
        textureCoordinate.x <= AnimatedUvCanvas24.z &&
        textureCoordinate.y >= AnimatedUvCanvas24.y &&
        textureCoordinate.y <= AnimatedUvCanvas24.w
    ){
        float2 offset = float2(textureCoordinate.x - AnimatedUvCanvas24.x, textureCoordinate.y - AnimatedUvCanvas24.y);
        originalColor = tex2D(textureSampler, AnimatedUvSource24 + offset);
    }

    if (UsesAnimatedUv25 &&
        textureCoordinate.x >= AnimatedUvCanvas25.x && 
        textureCoordinate.x <= AnimatedUvCanvas25.z &&
        textureCoordinate.y >= AnimatedUvCanvas25.y &&
        textureCoordinate.y <= AnimatedUvCanvas25.w
    ){
        float2 offset = float2(textureCoordinate.x - AnimatedUvCanvas25.x, textureCoordinate.y - AnimatedUvCanvas25.y);
        originalColor = tex2D(textureSampler, AnimatedUvSource25 + offset);
    }

    if (UsesAnimatedUv26 &&
        textureCoordinate.x >= AnimatedUvCanvas26.x && 
        textureCoordinate.x <= AnimatedUvCanvas26.z &&
        textureCoordinate.y >= AnimatedUvCanvas26.y &&
        textureCoordinate.y <= AnimatedUvCanvas26.w
    ){
        float2 offset = float2(textureCoordinate.x - AnimatedUvCanvas26.x, textureCoordinate.y - AnimatedUvCanvas26.y);
        originalColor = tex2D(textureSampler, AnimatedUvSource26 + offset);
    }

    if (UsesAnimatedUv27 &&
        textureCoordinate.x >= AnimatedUvCanvas27.x && 
        textureCoordinate.x <= AnimatedUvCanvas27.z &&
        textureCoordinate.y >= AnimatedUvCanvas27.y &&
        textureCoordinate.y <= AnimatedUvCanvas27.w
    ){
        float2 offset = float2(textureCoordinate.x - AnimatedUvCanvas27.x, textureCoordinate.y - AnimatedUvCanvas27.y);
        originalColor = tex2D(textureSampler, AnimatedUvSource27 + offset);
    }

    if (UsesAnimatedUv28 &&
        textureCoordinate.x >= AnimatedUvCanvas28.x && 
        textureCoordinate.x <= AnimatedUvCanvas28.z &&
        textureCoordinate.y >= AnimatedUvCanvas28.y &&
        textureCoordinate.y <= AnimatedUvCanvas28.w
    ){
        float2 offset = float2(textureCoordinate.x - AnimatedUvCanvas28.x, textureCoordinate.y - AnimatedUvCanvas28.y);
        originalColor = tex2D(textureSampler, AnimatedUvSource28 + offset);
    }

    if (UsesAnimatedUv29 &&
        textureCoordinate.x >= AnimatedUvCanvas29.x && 
        textureCoordinate.x <= AnimatedUvCanvas29.z &&
        textureCoordinate.y >= AnimatedUvCanvas29.y &&
        textureCoordinate.y <= AnimatedUvCanvas29.w
    ){
        float2 offset = float2(textureCoordinate.x - AnimatedUvCanvas29.x, textureCoordinate.y - AnimatedUvCanvas29.y);
        originalColor = tex2D(textureSampler, AnimatedUvSource29 + offset);
    }

    if (UsesAnimatedUv30 &&
        textureCoordinate.x >= AnimatedUvCanvas30.x && 
        textureCoordinate.x <= AnimatedUvCanvas30.z &&
        textureCoordinate.y >= AnimatedUvCanvas30.y &&
        textureCoordinate.y <= AnimatedUvCanvas30.w
    ){
        float2 offset = float2(textureCoordinate.x - AnimatedUvCanvas30.x, textureCoordinate.y - AnimatedUvCanvas30.y);
        originalColor = tex2D(textureSampler, AnimatedUvSource30 + offset);
    }

    if (UsesAnimatedUv31 &&
        textureCoordinate.x >= AnimatedUvCanvas31.x && 
        textureCoordinate.x <= AnimatedUvCanvas31.z &&
        textureCoordinate.y >= AnimatedUvCanvas31.y &&
        textureCoordinate.y <= AnimatedUvCanvas31.w
    ){
        float2 offset = float2(textureCoordinate.x - AnimatedUvCanvas31.x, textureCoordinate.y - AnimatedUvCanvas31.y);
        originalColor = tex2D(textureSampler, AnimatedUvSource31 + offset);
    }

    return originalColor;
}

float4 OverlayColor(float4 base, float4 overlay) {
    return 2 * base * overlay;
}

float4 OverlayColor2(float4 base, float4 overlay) {
    return (overlay  >= 0.5) 
        ? 2 * base * overlay 
        : base + overlay;
}

float4 ApplyPalette(float4 textureColor) {
    for (float index = 0; index < 16; index++) {
        if (textureColor.r >= index * 16.0 / 255.0 && textureColor.r <= index * 18.0 / 255.0) {
            return PaletteColors[index];
        }
    }

    return textureColor;
}

VertexToPixel VertexShaderFunction(AppToVertex input) {
    VertexToPixel output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    
    float4 normal = mul(input.Normal, WorldInverseTranspose);
    output.Color = DirectionalLightColor0;
    output.Normal = normal;
    output.TextureCoordinate = input.TextureCoordinate;
    
    return output;
}

float4 PixelShaderFunction(VertexToPixel input) : COLOR0 {
    float3 normal = normalize(input.Normal);
    float4 textureColor = GetTextureColor(textureSampler, input.TextureCoordinate);
    textureColor = ApplyPalette(textureColor);
    
    float4 lightColor = float4(0,0,0,1);
        
    if(textureColor.a == 1){
        float angle;
        float4 directionLightColor;
            
        angle = dot(normal, DirectionalLightDirection0);
        if(angle >= 0){
            directionLightColor = DirectionalLightColor0 * angle;
            lightColor += directionLightColor;
        }

        angle = dot(normal, DirectionalLightDirection1);
        if(angle >= 0){
            directionLightColor = DirectionalLightColor1 * angle;
            lightColor += directionLightColor;
        }
        
        angle = dot(normal, DirectionalLightDirection2);
        if(angle >= 0){
            directionLightColor = DirectionalLightColor2 * angle;
            lightColor += directionLightColor;
        }
        
        float4 ambientColor = AmbientColor;
        ambientColor.a = 1;
        lightColor.a = 1;
                        
        textureColor = textureColor * (ambientColor * 1.5f + lightColor);

        lightColor = float4(0,0,0,1);

        angle = dot(normal, DirectionalLightDirection0);
        if(angle >= 0){
            directionLightColor = DirectionalLightBoost0 * angle;
            lightColor += directionLightColor;
        }

        angle = dot(normal, DirectionalLightDirection1);
        if(angle >= 0){
            directionLightColor = DirectionalLightBoost1 * angle;
            lightColor += directionLightColor;
        }

        angle = dot(normal, DirectionalLightDirection2);
        if(angle >= 0){
            directionLightColor = DirectionalLightBoost2 * angle;
            lightColor += directionLightColor;
        }

        textureColor += textureColor * lightColor;
    }
    
    if (HighlightBright){
        textureColor.r += 0.5;
        textureColor.b += 0.6;
        textureColor.g += 0.5;
    }
    
    if (HighlightDim){
        textureColor.r += 0.3;
        textureColor.b += 0.4;
        textureColor.g += 0.3;
    }
    
//    textureColor = saturate(textureColor);
    
    if (textureColor.a > MaxAlpha){
        textureColor.a *= MaxAlpha;
    }
    
    return textureColor;
}

technique Render {
    pass Pass1 {
        VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
        PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
    }
}