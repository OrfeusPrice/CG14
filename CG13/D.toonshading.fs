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
    vec3 ambient = lightProperties.lightAmbient * texture(materialProperties.diffuseMap, TexCoords).rgb;
    
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(-lightProperties.lightDirection);  
    float diff = max(dot(norm, lightDir), 0.0);
    
    diff = step(0.2, diff) * 0.3 + step(0.6, diff) * 0.7; 
    
    vec3 diffuse = lightProperties.lightDiffuse * diff * texture(materialProperties.diffuseMap, TexCoords).rgb;  
    
    vec3 viewDir = normalize(cameraPosition - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);  
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), materialProperties.shiness);
    vec3 specular = lightProperties.lightSpecular * spec * texture(materialProperties.specularMap, TexCoords).rgb;  
    
    vec3 result = ambient + diffuse + specular;
    FragColor = vec4(result, 1.0);
}
