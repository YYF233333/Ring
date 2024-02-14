shader_type canvas_item;
render_mode unshaded;

uniform float progress: hint_range(0.0, 1.0);
uniform float smooth_size: hint_range(0.0, 1.0);
uniform sampler2D control: source_color;

void fragment() {
	float value = texture(control, UV).r;
	float progress_inner = progress * (1.0 + smooth_size) - smooth_size;
	float alpha = 1.0 - smoothstep(progress_inner, progress_inner + smooth_size, value);
	if (smooth_size == 0.0)
	{
		progress_inner = progress * 1.01 - 0.005;
		alpha = value < progress_inner ? 1.0 : 0.0;
	}
	COLOR = vec4(COLOR.rgb, alpha);
}
