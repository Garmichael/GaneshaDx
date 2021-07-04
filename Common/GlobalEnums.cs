namespace GaneshaDx.Common {
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
		Z
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
		None
	}
	
	public enum UvAnimationMode {
		ForwardLooping,
		ForwardAndReverseLooping,
		ForwardOnceOnTrigger,
		ReverseOnceOnTrigger,
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
		UnknownA,
		UnknownB,
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

	public enum MeshAnimationKeyFrameType {
		ChangeTo,
		ChangeBy,
		Unknown9,
		Unknown10,
		Unknown17,
		Unknown18,
		Other
	}
}