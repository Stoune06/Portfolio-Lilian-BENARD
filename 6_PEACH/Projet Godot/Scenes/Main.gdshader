shader_type canvas_item;

uniform vec2 player_screen_pos;
uniform float hole_radius = 100.0;
uniform float hole_feather = 60.0;
uniform float fade_amount = 1.0;
uniform float time;
uniform vec4 overlay_color : source_color = vec4(1.0, 0.0, 0.0, 0.6);

void fragment() {
    vec2 pixel_pos = FRAGCOORD.xy;
    float dist = distance(pixel_pos, player_screen_pos);

    // ✨ WIGGLE : on fait vibrer le bord du trou
    float angle = atan(pixel_pos.y - player_screen_pos.y, pixel_pos.x - player_screen_pos.x);
    float wiggle = sin(angle * 10.0 + time * 8.0) * 5.0; // + ou - 5 pixels

    float distorted_radius = hole_radius + wiggle;

    // Trou doux avec contour vibrant
    float hole_alpha = smoothstep(distorted_radius, distorted_radius + hole_feather, dist);

    float final_alpha = overlay_color.a * fade_amount * hole_alpha;

    COLOR = vec4(overlay_color.rgb, final_alpha);
}



