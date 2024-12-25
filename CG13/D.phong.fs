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
    vec3 ambientLight = lightProperties.lightAmbient * texture(materialProperties.diffuseMap, TexCoords).rgb;

    vec3 normalizedNormal = normalize(Normal);
    vec3 lightDirection = normalize(-lightProperties.lightDirection);
    float diffuseFactor = max(dot(normalizedNormal, lightDirection), 0.0);
    vec3 diffuseLight = lightProperties.lightDiffuse * diffuseFactor * texture(materialProperties.diffuseMap, TexCoords).rgb;

    vec3 viewDirection = normalize(cameraPosition - FragPos);
    vec3 reflectionDirection = reflect(-lightDirection, normalizedNormal);
    float specularFactor = pow(max(dot(viewDirection, reflectionDirection), 0.0), materialProperties.shiness);
    vec3 specularLight = lightProperties.lightSpecular * specularFactor * texture(materialProperties.specularMap, TexCoords).rgb;

    vec3 finalColor = ambientLight + diffuseLight + specularLight;
    FragColor = vec4(finalColor, 1.0);
}
