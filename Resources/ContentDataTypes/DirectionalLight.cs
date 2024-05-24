using Microsoft.Xna.Framework;

namespace GaneshaDx.Resources.ContentDataTypes;

public class DirectionalLight {
	public Color LightColor = new(0, 0, 0, 255);
	public Vector3 Overflow = Vector3.Zero;
	public double DirectionElevation;
	public double DirectionAzimuth;
}