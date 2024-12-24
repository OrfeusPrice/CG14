#version 330 core

in vec3 FragPos;   // Позиция фрагмента
in vec3 Normal;    // Нормаль фрагмента
in vec2 TexCoords; // Текстурные координаты

out vec4 FragColor;

uniform vec3 viewPos;   // Позиция камеры (или наблюдателя)
uniform float r0;       // Начальное значение Fresnel
uniform float roughness; 
uniform sampler2D lookupMap; // Текстура для вычислений с шероховатостью

// Функция для расчета Fresnel-эффекта
float fresnel(float ca) {
    return (r0 + (1.0 - r0) * pow(1.0 - ca, 5.0)) / ca;
}

void main() {
    const vec4 diffColor = vec4(0.5, 0.0, 0.0, 1.0); // Диффузный цвет
    const vec4 specColor = vec4(0.7, 0.7, 0.0, 1.0); // Спекулярный цвет
    const float e = 2.7182818284;  // Число Эйлера
    const float pi = 3.1415926;    // Число Пи

    // Нормализация вектора нормали
    vec3 n = normalize(Normal);

    // Направление к источнику света и камера
    vec3 l = normalize(vec3(0.0, 1.0, 0.0));   // Позиция источника света (например, фиксированная)
    vec3 v = normalize(viewPos - FragPos);     // Направление к камере
    vec3 h = normalize(v + l);                 // Полусумма векторов l и v

    // Расчеты для Cook-Torrance
    float nh = dot(n, h);   // Косинус угла между нормалью и полусуммой
    float nv = dot(n, v);   // Косинус угла между нормалью и вектором к камере
    float nl = dot(n, l);   // Косинус угла между нормалью и источником света

    // Извлечение значения из lookupMap для шероховатости
    float d = texture(lookupMap, vec2(roughness, nh)).x;

    // Fresnel-эффект
    float f = mix(pow(1.0 - nv, 5.0), 1.0, r0);

    // Геометрическое затухание
    float x = 2.0 * nh / dot(v, h);
    float g = min(1.0, min(x * nl, x * nv));

    // Подсчет общего освещения
    float ct = d * f * g / nv;

    // Диффузное и спекулярное освещение
    vec4 diff = diffColor * max(0.0, nl);
    vec4 spec = specColor * max(0.0, ct);

    // Итоговый цвет фрагмента
    FragColor = diff + spec;
}
