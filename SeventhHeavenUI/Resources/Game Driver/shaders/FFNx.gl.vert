VSH    ��� u_proj     d3dViewport     d3dProjection     	worldView     VSFlags     I  attribute highp vec4 a_color0;
attribute highp vec4 a_position;
attribute highp vec2 a_texcoord0;
varying highp vec4 v_color0;
varying highp vec2 v_texcoord0;
uniform highp mat4 u_proj;
uniform highp mat4 d3dViewport;
uniform highp mat4 d3dProjection;
uniform highp mat4 worldView;
uniform highp vec4 VSFlags;
void main ()
{
  highp vec2 coords_1;
  highp vec4 color_2;
  highp vec4 pos_3;
  pos_3 = a_position;
  coords_1 = a_texcoord0;
  color_2 = a_color0.zyxw;
  if ((VSFlags.x > 0.0)) {
    pos_3.w = (1.0/(a_position.w));
    pos_3.xyz = (a_position.xyz * pos_3.w);
    pos_3 = (u_proj * pos_3);
  } else {
    highp vec4 tmpvar_4;
    tmpvar_4.w = 1.0;
    tmpvar_4.xyz = pos_3.xyz;
    pos_3 = (((d3dViewport * d3dProjection) * worldView) * tmpvar_4);
    if ((a_color0.w > 0.5)) {
      color_2.w = 0.5;
    };
  };
  if ((VSFlags.y == 4.0)) {
    color_2.w = 1.0;
  } else {
    if ((VSFlags.y == 3.0)) {
      color_2.w = 0.25;
    };
  };
  if ((VSFlags.z > 0.0)) {
    coords_1.y = (1.0 - a_texcoord0.y);
  };
  gl_Position = pos_3;
  v_color0 = color_2;
  v_texcoord0 = coords_1;
}

 