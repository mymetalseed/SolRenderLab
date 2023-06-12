#ifndef CUSTOM_SURFACE_INCLUDED
#define CUSTOM_SURFACE_INCLUDED

//Lighting in a shader is about simulating the interactions between light that hits a surface.
struct Surface {
    float3 normal;
    float3 color;
    float alpha;
    float metallic;
    float smoothness;
};

#endif