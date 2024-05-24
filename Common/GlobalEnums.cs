namespace GaneshaDx.Common;

public enum ResourceType {
	InitialMeshData,
	OverrideMeshData,
	AlternateStateMehData,
	Texture,
	Padded,
	UnknownExtraDataA,
	UnknownExtraDataB,
	UnknownTwin,
	UnknownTrailingData,
	BadFormat
}

public enum MapWeather {
	None,
	NoneAlt,
	Normal,
	Strong,
	VeryStrong
}

public enum MapTime {
	Day,
	Night
}

public enum MapArrangementState {
	Primary,
	Secondary
}

public enum Axis {
	X,
	Y,
	Z,
	None
}
	
public enum WidgetSelectionMode {
	Select,
	PolygonTranslate,
	PolygonEdgeTranslate,
	PolygonVertexTranslate,
	PolygonRotate
}

public enum TextureAnimationType {
	UvAnimation,
	PaletteAnimation,
	UnknownAnimation,
	None
}
	
public enum UvAnimationMode {
	ForwardLooping,
	ForwardAndReverseLooping,
	ForwardOnceOnTrigger,
	ReverseOnceOnTrigger,
	Disabled,
	Unknown
}

public enum PaletteAnimationMode {
	ForwardLooping,
	ForwardAndReverseLooping,
	ForwardOnceOnTrigger,
	ForwardLoopingOnTrigger,
	Unknown
}
	
public enum MeshType {
	PrimaryMesh,
	AnimatedMesh1,
	AnimatedMesh2,
	AnimatedMesh3,
	AnimatedMesh4,
	AnimatedMesh5,
	AnimatedMesh6,
	AnimatedMesh7,
	AnimatedMesh8
}

public enum PolygonType {
	TexturedTriangle,
	TexturedQuad,
	UntexturedTriangle,
	UntexturedQuad,
}

public enum TerrainSurfaceType {
	NaturalSurface,
	SandArea,
	Stalactite,
	Grassland,
	Thicket,
	Snow,
	RockyCliff,
	Gravel,
	Wasteland,
	Swamp,
	Marsh,
	PoisonedMarsh,
	LavaRocks,
	Ice,
	Waterway,
	River,
	Lake,
	Sea,
	Lava,
	Road,
	WoodenFloor,
	StoneFloor,
	Roof,
	StoneWall,
	Sky,
	Darkness,
	Salt,
	Book,
	Obstacle,
	Rug,
	Tree,
	Box,
	Brick,
	Chimney,
	MudWall,
	Bridge,
	WaterPlant,
	Stairs,
	Furniture,
	Ivy,
	Deck,
	Machine,
	IronPlate,
	Moss,
	Tombstone,
	Waterfall,
	Coffin,
	FftbgPool,
	UnusedX30,
	UnusedX31,
	UnusedX32,
	UnusedX33,
	UnusedX34,
	UnusedX35,
	UnusedX36,
	UnusedX37,
	UnusedX38,
	UnusedX39,
	UnusedX3A,
	UnusedX3B,
	UnusedX3C,
	UnusedX3D,
	UnusedX3E,
	CrossSection
}

public enum TerrainSlopeType {
	Flat,
	InclineNorth,
	InclineEast,
	InclineSouth,
	InclineWest,
	ConvexNortheast,
	ConvexSoutheast,
	ConvexSouthwest,
	ConvexNorthwest,
	ConcaveNortheast,
	ConcaveSoutheast,
	ConcaveSouthwest,
	ConcaveNorthwest,
}

public enum TerrainDarkness {
	Normal,
	Dark,
	Darker,
	Darkest
}

public enum MeshAnimationTweenType {
	TweenTo,
	TweenBy,
	Oscillate,
	OscillateOffset,
	Unk9,
	Unk17,
	Invalid,
	Unknown
}