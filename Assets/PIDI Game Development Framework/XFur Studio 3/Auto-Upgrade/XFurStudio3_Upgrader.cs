namespace XFurStudio3.UpgradeTools {
#if UNITY_EDITOR
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using XFurStudio2;
    using XFurStudio3.Core;
    using XFurStudio3.Editor;
    using UnityEditor;

    public class XFurStudio3_Upgrader : MonoBehaviour {


        public GUISkin xfur3Skin;
        public Texture2D xfurStudioLogo;

        public XFurStudio3.Core.XFurStudioDatabase _newXFurDatabase;


        [MenuItem("XFur Studio 3/Auto-Upgrade/Upgrade Selected XFur Profiles")]
        public static void UpdateXFurProfile() {

            var assets = Selection.assetGUIDs;

            if ( assets.Length > 0 ) {
                for ( int i = 0; i < assets.Length; i++ ) {
                    var profile = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( assets[i] ), typeof( ScriptableObject ) );
                    if ( profile is XFurStudio2.XFurStudioFurProfile ) {
                        var xfur3Profile = new XFurStudio3.Core.XFurStudioFurProfile();
                        xfur3Profile.FurTemplate = new Core.XFurTemplate( true );
                        xfur3Profile.name = profile.name +"_X3";
                        UpdateFurTemplate( xfur3Profile.FurTemplate, ( (XFurStudio2.XFurStudioFurProfile)profile ).FurTemplate );
                        AssetDatabase.CreateAsset( xfur3Profile, AssetDatabase.GUIDToAssetPath( assets[i] ).Replace( profile.name + ".asset", xfur3Profile.name + ".asset" ) );
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        Debug.Log( "Updated Profile " + profile.name );
                    }
                }
            }

        }


        public void AutoUpgrade() {

            var x2 = GetComponent<XFurStudio2.XFurStudioInstance>();
           

            if ( !_newXFurDatabase ) {

                var databases = AssetDatabase.FindAssets( x2.FurDatabase.name+ "_X3" );

                if ( databases.Length > 0 ) {
                    _newXFurDatabase = (Core.XFurStudioDatabase)AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( databases[0] ), typeof( Core.XFurStudioDatabase ) );
                }
                else {

                    databases = AssetDatabase.FindAssets( x2.FurDatabase.name );

                    for ( int i = 0; i < databases.Length; i++ ) {

                        var databaseAsset = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( databases[i] ), typeof( ScriptableObject ) );

                        if ( databaseAsset is XFurStudio3.Core.XFurStudioDatabase ) {
                            _newXFurDatabase = (XFurStudio3.Core.XFurStudioDatabase)databaseAsset;
                        }
                        else {
                            _newXFurDatabase = new Core.XFurStudioDatabase();
                            _newXFurDatabase.name = x2.FurDatabase.name + "_X3";
                            AssetDatabase.CreateAsset( _newXFurDatabase, AssetDatabase.GetAssetPath( x2.FurDatabase ).Replace( x2.FurDatabase.name + ".asset", _newXFurDatabase.name + ".asset" ) );
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                            _newXFurDatabase.LoadResources();
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();

                            UpdateDatabase( x2.FurDatabase );

                        }

                    }
                }
            }


            if ( _newXFurDatabase ) {
                var x3 = gameObject.AddComponent<XFurStudio3.Core.XFurStudioInstance>();
                x3.InitialSetup();

                x3.FurDatabase = _newXFurDatabase;

                var rend = x3.MainRenderer;

                rend.AssignRenderer( x2.MainRenderer.renderer );


                x3.AutoAdjustForScale = x2.AutoAdjustForScale;
                x3.AutoUpdateMaterials = x2.AutoUpdateMaterials;
                x3.BetaMode = x2.BetaMode;
                x3.FullForwardMode = x2.FullForwardMode;
                x3.RenderingMode = x2.RenderingMode == XFurStudio2.XFurStudioInstance.XFurRenderingMode.BasicShells ? Core.XFurStudioInstance.XFurRenderingMode.BasicShells : Core.XFurStudioInstance.XFurRenderingMode.XFShells;
                x3.ShowAdvancedProperties = x2.ShowAdvancedProperties;
                x3.UpdateFrequency = x2.UpdateFrequency;
                x3.UseLossyScale = x2.UseLossyScale;

                x3.PhysicsModule.isEnabled = x2.PhysicsModule.enabled;
                x3.PhysicsModule.disableOnLOD = x2.PhysicsModule.disableOnLOD;
                x3.PhysicsModule.experimentalFeatures = x2.PhysicsModule.experimentalFeatures;
                x3.PhysicsModule.gravityStrength = x2.PhysicsModule.gravityStrength;
                x3.PhysicsModule.physicsSensitivity = x2.PhysicsModule.physicsSensitivity;
                x3.PhysicsModule.quality = (XFurStudio3.Modules.XF3_Physics.ModuleQuality)((int)x2.PhysicsModule.quality);


                x3.LODModule.isEnabled = x2.LODModule.enabled;
                x3.LODModule.BasicShellsRegion = x2.LODModule.BasicShellsRegion;
                x3.LODModule.BasicShellsSamples = x2.LODModule.BasicShellsSamples;
                x3.LODModule.experimentalFeatures = x2.LODModule.experimentalFeatures;
                x3.LODModule.FurSamplesRange = x2.LODModule.FurSamplesRange;
                x3.LODModule.hasMobileMode = x2.LODModule.hasMobileMode;
                x3.LODModule.hasSRPMode = x2.LODModule.hasSRPMode;

                x3.LODModule.lodRenderers = new Core.XFurRendererData[x2.LODModule.lodRenderers.Length];

                for (int i = 0; i < x2.LODModule.lodRenderers.Length; i++ ) {
                    x3.LODModule.lodRenderers[i].AssignRenderer( x2.LODModule.lodRenderers[i].renderer );
                }

                x3.LODModule.MaxFurDistance = x2.LODModule.MaxFurDistance;
                x3.LODModule.MinFurDistance = x2.LODModule.MinFurDistance;
                x3.LODModule.SwitchToBasicShells = x2.LODModule.SwitchToBasicShells;
                x3.LODModule.useOverdrawReduction = x2.LODModule.useOverdrawReduction;

                x3.DecalsModule.isEnabled = x2.DecalsModule.enabled;

                x3.DecalsModule.ProfileDecals = new List<Modules.XF3_Decals.PerProfileDecals>( new Modules.XF3_Decals.PerProfileDecals[x2.DecalsModule.ProfileDecals.Count] );
                
                for (int i = 0; i < x2.DecalsModule.ProfileDecals.Count; i++ ) {
                    x3.DecalsModule.ProfileDecals[i] = new Modules.XF3_Decals.PerProfileDecals();
                    x3.DecalsModule.ProfileDecals[i].decals = new List<Modules.XF3_Decals.DecalDefinition>( new Modules.XF3_Decals.DecalDefinition[x2.DecalsModule.ProfileDecals[i].decals.Count] );

                    for ( int d = 0; d < x2.DecalsModule.ProfileDecals[i].decals.Count; d++ ) {
                        x3.DecalsModule.ProfileDecals[i].decals[d] = new Modules.XF3_Decals.DecalDefinition();
                        x3.DecalsModule.ProfileDecals[i].decals[d].color = x2.DecalsModule.ProfileDecals[i].decals[d].color;
                        x3.DecalsModule.ProfileDecals[i].enabled = x2.DecalsModule.ProfileDecals[i].enabled;

                        switch ( x2.DecalsModule.ProfileDecals[i].decals[d].mixingMode ) {
                            case XFurStudio2_Decals.MixingMode.Add:
                                x3.DecalsModule.ProfileDecals[i].decals[d].mixingMode = Modules.XF3_Decals.MixingMode.Add;
                                break;
                        
                            case XFurStudio2_Decals.MixingMode.Overlay:
                                x3.DecalsModule.ProfileDecals[i].decals[d].mixingMode = Modules.XF3_Decals.MixingMode.Overlay;
                                break;
                        
                            case XFurStudio2_Decals.MixingMode.Multiply:
                                x3.DecalsModule.ProfileDecals[i].decals[d].mixingMode = Modules.XF3_Decals.MixingMode.Multiply;
                                break;
                        }


                        x3.DecalsModule.ProfileDecals[i].outputMode = x2.DecalsModule.ProfileDecals[i].outputMode;

                        x3.DecalsModule.ProfileDecals[i].decals[d].offset = x2.DecalsModule.ProfileDecals[i].decals[d].offset;
                        x3.DecalsModule.ProfileDecals[i].decals[d].sourceDecal = x2.DecalsModule.ProfileDecals[i].decals[d].sourceDecal;
                        x3.DecalsModule.ProfileDecals[i].decals[d].tiling = x2.DecalsModule.ProfileDecals[i].decals[d].tiling;
                    }

                }

                x3.RandomizationModule.isEnabled = x2.RandomizationModule.enabled;
                x3.RandomizationModule.RandomSettings = new List<Modules.XF3_Random.RandomizationSettings>( new Modules.XF3_Random.RandomizationSettings[x2.RandomizationModule.RandomSettings.Count] );

                for (int i = 0; i < x3.RandomizationModule.RandomSettings.Count; i++ ) {

                    x3.RandomizationModule.RandomSettings[i] = new Modules.XF3_Random.RandomizationSettings();
                    x3.RandomizationModule.RandomSettings[i].Enabled = x2.RandomizationModule.RandomSettings[i].Enabled; 
                    x3.RandomizationModule.RandomSettings[i].RandomizationMode = x2.RandomizationModule.RandomSettings[i].RandomizationMode; 
                    x3.RandomizationModule.RandomSettings[i].RandomizeColorMap = x2.RandomizationModule.RandomSettings[i].RandomizeColorMap; 
                    x3.RandomizationModule.RandomSettings[i].RandomizeColorMix = x2.RandomizationModule.RandomSettings[i].RandomizeColorMix; 
                    x3.RandomizationModule.RandomSettings[i].RandomizeDataMaps = x2.RandomizationModule.RandomSettings[i].RandomizeDataMaps; 
                    x3.RandomizationModule.RandomSettings[i].RandomizeFurStrands = x2.RandomizationModule.RandomSettings[i].RandomizeFurStrands;
                    x3.RandomizationModule.RandomSettings[i].RandomizeTo = new Core.XFurTemplate(true);
                    UpdateFurTemplate( x3.RandomizationModule.RandomSettings[i].RandomizeTo, x2.RandomizationModule.RandomSettings[i].RandomizeTo );
                    
                }

                x3.VFXModule.isEnabled = x2.VFXModule.enabled;
                x3.VFXModule.Blood.color = x2.VFXModule.Blood.color;
                x3.VFXModule.Blood.fadeoutTime = x2.VFXModule.Blood.fadeoutTime;
                x3.VFXModule.Blood.ignoreWeatherComponent = x2.VFXModule.Blood.ignoreWeatherComponent;
                x3.VFXModule.Blood.intensity = x2.VFXModule.Blood.intensity;
                x3.VFXModule.Blood.normalFalloff = x2.VFXModule.Blood.normalFalloff;
                x3.VFXModule.Blood.penetration = x2.VFXModule.Blood.penetration;
                x3.VFXModule.Blood.smoothness = x2.VFXModule.Blood.smoothness;
                x3.VFXModule.Blood.specular = x2.VFXModule.Blood.specular;
                x3.VFXModule.Blood.vfxMask = x2.VFXModule.Blood.vfxMask;
                x3.VFXModule.Blood.vfxTiling = x2.VFXModule.Blood.vfxTiling;

                

                x3.VFXModule.Snow.color = x2.VFXModule.Snow.color;
                x3.VFXModule.Snow.fadeoutTime = x2.VFXModule.Snow.fadeoutTime;
                x3.VFXModule.Snow.ignoreWeatherComponent = x2.VFXModule.Snow.ignoreWeatherComponent;
                x3.VFXModule.Snow.intensity = x2.VFXModule.Snow.intensity;
                x3.VFXModule.Snow.normalFalloff = x2.VFXModule.Snow.normalFalloff;
                x3.VFXModule.Snow.penetration = x2.VFXModule.Snow.penetration;
                x3.VFXModule.Snow.smoothness = x2.VFXModule.Snow.smoothness;
                x3.VFXModule.Snow.specular = x2.VFXModule.Snow.specular;
                x3.VFXModule.Snow.vfxMask = x2.VFXModule.Snow.vfxMask;
                x3.VFXModule.Snow.vfxTiling = x2.VFXModule.Snow.vfxTiling;

                

                x3.VFXModule.Rain.color = x2.VFXModule.Rain.color;
                x3.VFXModule.Rain.fadeoutTime = x2.VFXModule.Rain.fadeoutTime;
                x3.VFXModule.Rain.ignoreWeatherComponent = x2.VFXModule.Rain.ignoreWeatherComponent;
                x3.VFXModule.Rain.intensity = x2.VFXModule.Rain.intensity;
                x3.VFXModule.Rain.normalFalloff = x2.VFXModule.Rain.normalFalloff;
                x3.VFXModule.Rain.penetration = x2.VFXModule.Rain.penetration;
                x3.VFXModule.Rain.smoothness = x2.VFXModule.Rain.smoothness;
                x3.VFXModule.Rain.specular = x2.VFXModule.Rain.specular;
                x3.VFXModule.Rain.vfxMask = x2.VFXModule.Rain.vfxMask;
                x3.VFXModule.Rain.vfxTiling = x2.VFXModule.Rain.vfxTiling;


                for (int i = 0; i < x2.MainRenderer.isFurMaterial.Length; i++ ) {
                    rend.isFurMaterial[i] = x2.MainRenderer.isFurMaterial[i];
                }


                x3.MainRenderer = rend;

                x3.InitialSetup();

                for ( int i = 0; i < x3.FurDataProfiles.Length; i++ ) {
                    var x3Fur = x3.FurDataProfiles[i];
                    var x2Fur = x2.FurDataProfiles[i];

                    UpdateFurTemplate( x3Fur, x2Fur );

                }

            }

            x2.enabled = false;



        }





        public static void UpdateFurTemplate( XFurStudio3.Core.XFurTemplate x3Fur, XFurStudio2.XFurTemplate x2Fur ) {


            x3Fur.FurStrandsAsset = UpdateStrandsAsset( x2Fur.FurStrandsAsset );

            x3Fur.CastShadows = x2Fur.CastShadows;
            x3Fur.DoubleSided = x2Fur.DoubleSided;
            x3Fur.FurBaseTiling = x2Fur.FurBaseTiling;
            x3Fur.FurColorA = x2Fur.FurColorA;
            x3Fur.FurColorB = x2Fur.FurColorB;
            x3Fur.FurColorC = x2Fur.FurColorC;
            x3Fur.FurColorD = x2Fur.FurColorD;
            x3Fur.FurColorMap = x2Fur.FurColorMap;
            x3Fur.FurColorTiling = x2Fur.FurColorTiling;
            x3Fur.FurColorVariation = x2Fur.FurColorVariation;
            x3Fur.FurCurlAmountX = x2Fur.FurCurlAmountX;
            x3Fur.FurCurlAmountY = x2Fur.FurCurlAmountY;
            x3Fur.FurCurlSizeX = x2Fur.FurCurlSizeX;
            x3Fur.FurCurlSizeY = x2Fur.FurCurlSizeY;
            x3Fur.FurData0 = x2Fur.FurData0;
            x3Fur.FurData1 = x2Fur.FurData1;
            x3Fur.FurEmissionColor = x2Fur.FurEmissionColor;
            x3Fur.FurEmissionMap = x2Fur.FurEmissionMap;
            x3Fur.FurGroomStrength = x2Fur.FurGroomStrength;
            x3Fur.FurLength = x2Fur.FurLength;
            x3Fur.FurMainTint = x2Fur.FurMainTint;
            x3Fur.FurNormalmap = x2Fur.FurNormalmap;
            x3Fur.FurOcclusion = x2Fur.FurOcclusion;
            x3Fur.FurOcclusionCurve = x2Fur.FurOcclusionCurve;
            x3Fur.FurOverColorMod = x2Fur.FurOverColorMod;
            x3Fur.FurRim = x2Fur.FurRim;
            x3Fur.FurRimBoost = x2Fur.FurRimBoost;
            x3Fur.FurRimPower = x2Fur.FurRimPower;
            x3Fur.FurSamples = x2Fur.FurSamples;
            x3Fur.FurShadowsTint = x2Fur.FurShadowsTint;
            x3Fur.FurSmoothness = x2Fur.FurSmoothness;
            x3Fur.FurSpecularTint = x2Fur.FurSpecularTint;
            x3Fur.FurThickness = x2Fur.FurThickness;
            x3Fur.FurThicknessCurve = x2Fur.FurThicknessCurve;
            x3Fur.FurTransmission = x2Fur.FurTransmission;
            x3Fur.FurUnderColorMod = x2Fur.FurUnderColorMod;
            x3Fur.FurUVTiling = x2Fur.FurUVTiling;
            x3Fur.ProbeUse = x2Fur.ProbeUse;
            x3Fur.ReceiveShadows = x2Fur.ReceiveShadows;
            x3Fur.SelfWindStrength = x2Fur.SelfWindStrength;
            x3Fur.SkinColor = x2Fur.SkinColor;
            x3Fur.SkinColorMap = x2Fur.SkinColorMap;
            x3Fur.SkinNormalMap = x2Fur.SkinNormalMap;
            x3Fur.SkinSmoothness = x2Fur.SkinSmoothness;
            x3Fur.UseCurlyFur = x2Fur.UseCurlyFur;
            x3Fur.UseEmissiveFur = x2Fur.UseEmissiveFur;

        }



        static XFurStudio3.Core.XFurStudioStrandsAsset UpdateStrandsAsset( XFurStudio2.XFurStudioStrandsAsset original ) {

            if ( !original ) {
                return null;
            }

            XFurStudio3.Core.XFurStudioStrandsAsset xfurStrand3;

            var strandAssets = AssetDatabase.FindAssets( original.name+"_X3" );

            if (strandAssets.Length > 0 ) {
                xfurStrand3 = (Core.XFurStudioStrandsAsset)AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( strandAssets[0] ), typeof( Core.XFurStudioStrandsAsset ) );
                return xfurStrand3;
            }
            else {
                strandAssets = AssetDatabase.FindAssets( original.name );
            }

            if ( strandAssets.Length > 0 ) {

                for ( int i = 0; i < strandAssets.Length; i++ ) {

                    var strand = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( strandAssets[i] ), typeof( ScriptableObject ) );

                    if ( strand is XFurStudio3.Core.XFurStudioStrandsAsset ) {
                        xfurStrand3 = (XFurStudio3.Core.XFurStudioStrandsAsset)strand;
                        return xfurStrand3;
                    }
                    else if ( strand == original ) {
                        Debug.Log( "CRETING NEW STRANDS ASSET" );

                        xfurStrand3 = new Core.XFurStudioStrandsAsset();
                        xfurStrand3.name = original.name + "_X3";
                        AssetDatabase.CreateAsset( xfurStrand3, AssetDatabase.GetAssetPath( original ).Replace( original.name + ".asset", xfurStrand3.name + ".asset" ) );
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        xfurStrand3.CustomStrandsTexture = original.CustomStrandsTexture;
                        xfurStrand3.firstPassDensity = original.firstPassDensity;
                        xfurStrand3.firstPassSize = original.firstPassSize;
                        xfurStrand3.firstPassVariation = original.firstPassVariation;
                        xfurStrand3.generateMips = original.generateMips;
                        xfurStrand3.gradientStrength = original.gradientStrength;
                        xfurStrand3.horizontalAspect = original.horizontalAspect;
                        xfurStrand3.perlinNoiseScale = original.perlinNoiseScale;
                        xfurStrand3.randomizeRotations = original.randomizeRotations;
                        xfurStrand3.roundness = original.roundness;
                        xfurStrand3.secondPassDensity = original.secondPassDensity;
                        xfurStrand3.secondPassSize = original.secondPassSize;
                        xfurStrand3.secondPassVariation = original.secondPassVariation;
                        xfurStrand3.strandsDensity = original.strandsDensity;
                        xfurStrand3.strandShapeA.gradient = original.strandShapeA.gradient;
                        xfurStrand3.strandShapeA.horizontalSize = original.strandShapeA.horizontalSize;
                        xfurStrand3.strandShapeA.verticalSize = original.strandShapeA.verticalSize;
                        xfurStrand3.strandShapeB.gradient = original.strandShapeB.gradient;
                        xfurStrand3.strandShapeB.horizontalSize = original.strandShapeB.horizontalSize;
                        xfurStrand3.strandShapeB.verticalSize = original.strandShapeB.verticalSize;
                        xfurStrand3.verticalAspect = original.verticalAspect;
                        xfurStrand3.xfurStrandsMethod = original.xfurStrandsMethod == XFurStudio2.XFurStudioStrandsAsset.XFurStrandsMethod.CustomTexture ? Core.XFurStudioStrandsAsset.XFurStrandsMethod.CustomTexture : Core.XFurStudioStrandsAsset.XFurStrandsMethod.ProcedurallyGenerated;

                        xfurStrand3.PoissonStrandsGenerator();


                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        return xfurStrand3;

                    }
                }
            }

            return null;
            
        }


        void UpdateDatabase( XFurStudio2.XFurStudioDatabase original ) {

            switch( original.RenderingMode ) {

                case XFurStudio2.XFurStudioDatabase.XFurRenderingMode.Standard:
                    _newXFurDatabase.RenderingMode = Core.XFurStudioDatabase.XFurRenderingMode.Standard;
                    break;
                    
                case XFurStudio2.XFurStudioDatabase.XFurRenderingMode.Universal:
                    _newXFurDatabase.RenderingMode = Core.XFurStudioDatabase.XFurRenderingMode.Universal;
                    break;
                    
                case XFurStudio2.XFurStudioDatabase.XFurRenderingMode.HighDefinition:
                    _newXFurDatabase.RenderingMode = Core.XFurStudioDatabase.XFurRenderingMode.HighDefinition;
                    break;

            }


            _newXFurDatabase.UseNormalmaps = original.UseNormalmaps;
            _newXFurDatabase.receiveURPShadows = original.receiveURPShadows;

        }


    }


    [CustomEditor(typeof(XFurStudio3_Upgrader))]
    public class XFurStudio3_Upgrader_Editor : Editor {


        GUISkin pidiSkin2;
        Texture2D xfurStudioLogo;

        public override void OnInspectorGUI() {


            XFurStudio3_Upgrader upgrader = (XFurStudio3_Upgrader)target;

            pidiSkin2 = upgrader.xfur3Skin;
            xfurStudioLogo = upgrader.xfurStudioLogo;


            var lStyle = new GUIStyle( EditorStyles.label );

            GUI.color = EditorGUIUtility.isProSkin ? new Color( 0.1f, 0.1f, 0.15f, 1 ) : new Color( 0.5f, 0.5f, 0.6f );
            GUILayout.BeginVertical();
            GUI.color = Color.white;

            AssetLogoAndVersion();                  


            GUILayout.BeginHorizontal(); GUILayout.Space( 32 ); GUILayout.BeginVertical();

            GUILayout.Space( 16 );

            if ( !serializedObject.FindProperty( "_newXFurDatabase" ).objectReferenceValue ) {
                EditorGUILayout.HelpBox( "If no Database for XFur Studio 3 is assigned, the upgrader will create a new one at the location of the original one from version 2.x.x", MessageType.Info );
                GUILayout.Space( 16 );

            }

            EditorGUILayout.PropertyField( serializedObject.FindProperty( "_newXFurDatabase" ) );


            GUILayout.Space( 16 );

            GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();

            if ( GUILayout.Button( "Auto-Upgrade", pidiSkin2.button, GUILayout.Width( 250 ) ) ) {
                upgrader.AutoUpgrade();
            }

            GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();

            GUILayout.Space( 16 );


            GUILayout.EndVertical(); GUILayout.Space( 32 ); GUILayout.EndHorizontal();

            GUILayout.Space( 16 );


            GUILayout.Space( 16 );

            GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();

            lStyle = new GUIStyle( EditorStyles.label );
            lStyle.fontStyle = FontStyle.Italic;
            lStyle.fontSize = 8;

            GUILayout.Label( "Copyright© 2017-2023,   Jorge Pinal N.", lStyle );

            GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();

            GUILayout.Space( 24 );
            GUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();

        }



        #region PIDI 2020 EDITOR




        /// <summary>
        /// Draws the asset's logo and its current version
        /// </summary>
        public void AssetLogoAndVersion() {

            GUILayout.BeginVertical( xfurStudioLogo, pidiSkin2 ? pidiSkin2.customStyles[1] : null );
            GUILayout.Space( 45 );
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label( "3.0.0", pidiSkin2.customStyles[2] );
            GUILayout.Space( 6 );
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Draws a label centered in the Editor window
        /// </summary>
        /// <param name="label"></param>
        public void CenteredLabel( string label ) {

            GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();
            GUILayout.Label( label, EditorStyles.boldLabel );
            GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();

        }



        #endregion



    }



#endif

}