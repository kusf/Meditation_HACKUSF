shader_type canvas_item;

uniform float imageOffset = 10.0;
uniform float imageStretch = 1.0;
uniform float magnifier = 2.0;

void fragment() {
    float u = imageOffset * TEXTURE_PIXEL_SIZE.x;
	vec2 newUV = vec2(UV.x + u, UV.y) / magnifier;
	newUV.y += sin(0.04 * (UV.y) * cos(imageStretch));
    COLOR = texture(TEXTURE, newUV);
}