using System.Collections.Generic;
using System.Linq;
using GaneshaDx.Common;
using GaneshaDx.Resources.ContentDataTypes.MeshAnimations;
using GaneshaDx.Resources.ContentDataTypes.Palettes;
using GaneshaDx.Resources.ContentDataTypes.Polygons;
using GaneshaDx.Resources.ContentDataTypes.Terrains;
using GaneshaDx.Resources.ContentDataTypes.TextureAnimations;
using GaneshaDx.Resources.ResourceContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GaneshaDx.Resources.ContentDataTypes {
	public class MapStateData {
		public MapArrangementState MapArrangementState;
		public MapTime MapTime;
		public MapWeather MapWeather;

		public List<MapResource> StateMeshResources;
		public MapResource StateTextureResource;

		public Dictionary<MeshType, Dictionary<PolygonType, List<Polygon>>> PolygonCollection =>
			_primaryMeshSource?.PolygonCollection;

		public List<Polygon> PolygonCollectionBucket {
			get {
				List<Polygon> polygonCollectionBucket = new List<Polygon>();
				foreach (MeshType meshType in CommonLists.MeshTypes) {
					foreach (PolygonType polygonType in CommonLists.PolygonTypes) {
						polygonCollectionBucket.AddRange(PolygonCollection[meshType][polygonType]);
					}
				}

				return polygonCollectionBucket.OrderByDescending(polygon => polygon.DistanceToCamera).ToList();
			}
		}

		private MeshResourceData _primaryMeshSource;
		private MeshResourceData _animatedMeshSource;
		private MeshResourceData _lightingAndBackgroundSource;
		private MeshResourceData _terrainSource;
		private MeshResourceData _paletteSource;
		private MeshResourceData _textureAnimationSource;

		public Texture2D Texture;

		public Color BackgroundTopColor {
			get => _lightingAndBackgroundSource?.BackgroundTopColor ?? Color.DeepPink;
			set {
				if (_lightingAndBackgroundSource != null) {
					_lightingAndBackgroundSource.BackgroundTopColor = value;
				}
			}
		}

		public Color BackgroundBottomColor {
			get => _lightingAndBackgroundSource?.BackgroundBottomColor ?? Color.DeepPink;
			set {
				if (_lightingAndBackgroundSource != null) {
					_lightingAndBackgroundSource.BackgroundBottomColor = value;
				}
			}
		}

		public Color AmbientLightColor {
			get => _lightingAndBackgroundSource?.AmbientLightColor ?? Color.DeepPink;
			set {
				if (_lightingAndBackgroundSource != null) {
					_lightingAndBackgroundSource.AmbientLightColor = value;
				}
			}
		}

		public List<DirectionalLight> DirectionalLights {
			get => _lightingAndBackgroundSource?.DirectionalLights;
			set {
				if (_lightingAndBackgroundSource != null) {
					_lightingAndBackgroundSource.DirectionalLights = value;
				}
			}
		}

		public List<Palette> Palettes {
			get => _paletteSource?.Palettes;
			set {
				if (_paletteSource.Palettes != null) {
					_paletteSource.Palettes = value;
				}
			}
		}

		public List<Palette> PaletteAnimationFrames {
			get => _textureAnimationSource?.PaletteAnimationFrames;
			set {
				if (_textureAnimationSource != null) {
					_textureAnimationSource.PaletteAnimationFrames = value;
				}
			}
		}

		public List<AnimatedTextureInstructions> TextureAnimations {
			get => _textureAnimationSource?.AnimatedTextureInstructions;
			set {
				if (_textureAnimationSource != null) {
					_textureAnimationSource.AnimatedTextureInstructions = value;
				}
			}
		}

		public MeshAnimationInstructions MeshAnimationInstructions {
			get => _animatedMeshSource?.AnimatedMeshInstructions;
			set {
				if (_animatedMeshSource != null) {
					_animatedMeshSource.AnimatedMeshInstructions = value;
				}
			}
		}

		public Terrain Terrain {
			get => _terrainSource?.Terrain;
			set {
				if (_terrainSource != null) {
					_terrainSource.Terrain = value;
				}
			}
		}

		public void SetStateResources(
			List<MapResource> initialMeshResources,
			List<MapResource> stateMeshResources,
			MapResource stateTextureResource
		) {
			StateMeshResources = stateMeshResources;
			StateTextureResource = stateTextureResource;

			//Texture
			TextureResourceData textureResourceData = (TextureResourceData) stateTextureResource.ResourceData;
			Texture = textureResourceData.Texture;

			//Primary Mesh Source
			foreach (MapResource mapResource in stateMeshResources) {
				MeshResourceData resourceData = (MeshResourceData) mapResource.ResourceData;
				if (resourceData.HasPrimaryMesh) {
					_primaryMeshSource = resourceData;
					break;
				}
			}

			if (_primaryMeshSource == null) {
				foreach (MapResource mapResource in initialMeshResources) {
					MeshResourceData resourceData = (MeshResourceData) mapResource.ResourceData;
					if (resourceData.HasPrimaryMesh) {
						_primaryMeshSource = resourceData;
						break;
					}
				}
			}

			//Lighting and Background Source
			foreach (MapResource mapResource in stateMeshResources) {
				MeshResourceData resourceData = (MeshResourceData) mapResource.ResourceData;
				if (resourceData.HasLightsAndBackground) {
					_lightingAndBackgroundSource = resourceData;
				}
			}

			if (_lightingAndBackgroundSource == null) {
				foreach (MapResource mapResource in initialMeshResources) {
					MeshResourceData resourceData = (MeshResourceData) mapResource.ResourceData;
					if (resourceData.HasLightsAndBackground) {
						_lightingAndBackgroundSource = resourceData;
						break;
					}
				}
			}

			// Palette Source
			foreach (MapResource mapResource in stateMeshResources) {
				MeshResourceData resourceData = (MeshResourceData) mapResource.ResourceData;
				if (resourceData.HasPalettes) {
					_paletteSource = resourceData;
					break;
				}
			}

			if (_paletteSource == null) {
				foreach (MapResource mapResource in initialMeshResources) {
					MeshResourceData resourceData = (MeshResourceData) mapResource.ResourceData;
					if (resourceData.HasPalettes) {
						_paletteSource = resourceData;
						break;
					}
				}
			}

			// Terrain Source
			foreach (MapResource mapResource in stateMeshResources) {
				MeshResourceData resourceData = (MeshResourceData) mapResource.ResourceData;
				if (resourceData.HasTerrain) {
					_terrainSource = resourceData;
					break;
				}
			}

			if (_terrainSource == null) {
				foreach (MapResource mapResource in initialMeshResources) {
					MeshResourceData resourceData = (MeshResourceData) mapResource.ResourceData;
					if (resourceData.HasTerrain) {
						_terrainSource = resourceData;
						break;
					}
				}
			}

			// Texture Animation Source
			foreach (MapResource mapResource in stateMeshResources) {
				MeshResourceData resourceData = (MeshResourceData) mapResource.ResourceData;
				if (resourceData.HasTextureAnimations) {
					_textureAnimationSource = resourceData;
					break;
				}
			}

			if (_textureAnimationSource == null) {
				foreach (MapResource mapResource in initialMeshResources) {
					MeshResourceData resourceData = (MeshResourceData) mapResource.ResourceData;
					if (resourceData.HasTextureAnimations) {
						_textureAnimationSource = resourceData;
						break;
					}
				}
			}

			// Texture Animation Source
			foreach (MapResource mapResource in stateMeshResources) {
				MeshResourceData resourceData = (MeshResourceData) mapResource.ResourceData;
				if (resourceData.HasTextureAnimations) {
					_textureAnimationSource = resourceData;
					break;
				}
			}

			if (_textureAnimationSource == null) {
				foreach (MapResource mapResource in initialMeshResources) {
					MeshResourceData resourceData = (MeshResourceData) mapResource.ResourceData;
					if (resourceData.HasTextureAnimations) {
						_textureAnimationSource = resourceData;
						break;
					}
				}
			}

			// Animated Mesh Source
			foreach (MapResource mapResource in stateMeshResources) {
				MeshResourceData resourceData = (MeshResourceData) mapResource.ResourceData;
				if (resourceData.HasAnimatedMeshes) {
					_animatedMeshSource = resourceData;
					break;
				}
			}

			if (_animatedMeshSource == null) {
				foreach (MapResource mapResource in initialMeshResources) {
					MeshResourceData resourceData = (MeshResourceData) mapResource.ResourceData;
					if (resourceData.HasAnimatedMeshes) {
						_animatedMeshSource = resourceData;
						break;
					}
				}
			}
		}
	}
}