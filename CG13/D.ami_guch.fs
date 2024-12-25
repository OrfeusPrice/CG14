#version 330 core
out vec4 FragColor;

struct Material {
    sampler2D diffuseMap;
    sampler2D specularMap;
    float shiness;
}; 

struct LightSource {
    vec3 lightDirection;
    vec3 lightAmbient;
    vec3 lightDiffuse;
    vec3 lightSpecular;
};

in vec3 FragPos;  
in vec3 Normal;  
in vec2 TexCoords;

uniform vec3 cameraPosition;
uniform Material materialProperties;
uniform LightSource lightProperties;

void main()
{
    vec3 coldLightColor = vec3(1.0, 0.7, 0.3);
    vec3 warmLightColor = vec3(0.5, 0.6, 0.9);
    
    vec3 normal = normalize(Normal);
    vec3 lightDirection = normalize(-lightProperties.lightDirection);

    float diffuseFactor = max(dot(normal, lightDirection), 0.0);
    vec3 coldDiffuse = lightProperties.lightDiffuse * diffuseFactor * coldLightColor;
    vec3 warmDiffuse = lightProperties.lightDiffuse * diffuseFactor * warmLightColor;

    vec3 diffuseColorCool = min(coldLightColor + coldDiffuse * texture(materialProperties.diffuseMap, TexCoords).rgb, 1.0);
    vec3 diffuseColorWarm = min(warmLightColor + warmDiffuse * texture(materialProperties.diffuseMap, TexCoords).rgb, 1.0);

    vec3 finalDiffuse = mix(diffuseColorCool, diffuseColorWarm, diffuseFactor);

    vec3 viewDirection = normalize(cameraPosition - FragPos);
    vec3 reflectionDirection = reflect(-lightDirection, normal);
    float specularFactor = pow(max(dot(viewDirection, reflectionDirection), 0.0), materialProperties.shiness);
    vec3 specularColor = lightProperties.lightSpecular * specularFactor * texture(materialProperties.specularMap, TexCoords).rgb;

    vec3 finalColor = finalDiffuse + specularColor;
    
    FragColor = vec4(min(finalColor, vec3(1.0)), 1.0);
}
