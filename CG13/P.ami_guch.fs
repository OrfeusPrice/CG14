#version 330 core
out vec4 FragColor;

struct Material {
    sampler2D diffuseMap;
    sampler2D specularMap;    
    float shiness;
}; 

struct Light {
    vec3 position;  
  
    vec3 ambient;
    vec3 diffuseMap;
    vec3 specularMap;
	
    float constant;
    float linear;
    float quadratic;
};

in vec3 FragPos;  
in vec3 Normal;  
in vec2 TexCoords;
  
uniform vec3 cameraPosition;
uniform Material material;
uniform Light light;

void main()
{
    vec3 coldLightColor = vec3(1.0, 0.7, 0.3);
	vec3 warmLightColor = vec3(0.5, 0.6, 0.9);
	
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(light.position - FragPos);
    float NdotL = max(dot(norm, lightDir), 0.0);
    
	vec3 diffuseCool = light.diffuseMap * NdotL * coldLightColor;
    vec3 diffuseWarm = light.diffuseMap * NdotL * warmLightColor;
    
	vec3 kCool = min(coldLightColor + diffuseCool * texture(material.diffuseMap, TexCoords).rgb, 1.0);
    vec3 kWarm = min(warmLightColor + diffuseWarm * texture(material.diffuseMap, TexCoords).rgb, 1.0);
    
    vec3 kFinal = mix(kCool, kWarm, NdotL);
    
    vec3 viewDir = normalize(cameraPosition - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);  
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shiness);
    vec3 specularMap = light.specularMap * spec * texture(material.specularMap, TexCoords).rgb;  
    
    float distance    = length(light.position - FragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));     
    
	vec3 result = (kFinal + specularMap) * attenuation;
    
    FragColor = vec4(min(result, vec3(1.0)), 1.0);
} 