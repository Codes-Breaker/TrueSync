namespace XFurStudio3.Designer {

    using XFurStudio3.Core;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using System.IO;


    public class XFurStudio3Designer : EditorWindow {

        public XFurStudioInstance xfur;

        public int editFurProfile;

        public GUISkin pidiSkin2;

        public Texture2D xfurStudioLogo;

        public Texture2D genSettings, genProps, brushShave, brushLen, brushThick, brushOcc, brushGroom, exportData;

        public Vector2 scrollView;

        public int exportResolution = 1;

        MeshCollider xfurCollider;

        UndoManager xfurUndo = new UndoManager();


        [System.Serializable]
        class XFurBrushData {


            public int activeTool = 0;
            public bool invert;
            public bool mirror;

            public bool fineTuneBrush;
            public bool hasContact;

            public Vector3 brushCenter;
            public Vector3 brushNormal;

            public float opacity = 1.0f;
            public float falloff = 0.5f;
            public float size = 0.05f;

            public Vector2 minMaxSize = new Vector2( 0.0001f, 1.0f );

        }


        XFurBrushData brushData = new XFurBrushData();

        bool initialFocus;


        static void Init() {
            // Get existing open window or if none, make a new one:
            XFurStudio3Designer window = (XFurStudio3Designer)EditorWindow.GetWindow( typeof( XFurStudio3Designer ) );
            window.Show();
        }

        void OnEnable() {

            ActiveEditorTracker.sharedTracker.isLocked = true;
            
            SceneView.duringSceneGui -= OnSceneGUI;
            // Add (or re-add) the delegate.
            SceneView.duringSceneGui += OnSceneGUI;
        }

        void OnDestroy() {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        void OnSceneGUI( SceneView sceneView ) {


            Event currentEvent = Event.current;

            if ( currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.F || !initialFocus ) {
                sceneView.LookAt( xfur.MainRenderer.renderer.bounds.center );
                initialFocus = true;
                return;
            }

            Selection.SetActiveObjectWithContext( null, null );

            if ( !xfurCollider ) {
                xfurCollider = new GameObject( "XFCollider", typeof( MeshCollider ) ).GetComponent<MeshCollider>();
                xfurCollider.gameObject.hideFlags = HideFlags.HideAndDontSave;
                return;
            }

            xfurCollider.sharedMesh = xfur.CurrentMesh;
            xfurCollider.transform.position = xfur.transform.position;
            xfurCollider.transform.rotation = xfur.transform.rotation;

            if ( brushData.activeTool > 1 && brushData.activeTool < 7 ) {

                Ray ray = HandleUtility.GUIPointToWorldRay( currentEvent.mousePosition );
                var hits = Physics.RaycastAll( ray, 100f );


                if ( !currentEvent.alt ) {
                    HandleUtility.AddDefaultControl( GUIUtility.GetControlID( FocusType.Passive ) );
                }
                else {
                    return;
                }


                if ( currentEvent.control ) {

                    if ( currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.Z) {
                        xfurUndo.Undo( xfur, editFurProfile, brushData.activeTool );
                        currentEvent.Use();
                        return;
                    }

                }


                if ( currentEvent.shift ) {


                    if ( currentEvent.type == EventType.MouseDrag ) {

                        if ( currentEvent.button == 0 ) {

                            if ( Mathf.Abs( currentEvent.delta.x ) > Mathf.Abs( currentEvent.delta.y ) + 0.1 ) {
                                if ( currentEvent.shift ) {
                                    brushData.size += 0.005f * currentEvent.delta.x;
                                    brushData.size = Mathf.Clamp( brushData.size, brushData.minMaxSize.x, brushData.minMaxSize.y );
                                    currentEvent.Use();
                                }
                            }
                            else if ( Mathf.Abs( currentEvent.delta.y ) > Mathf.Abs( currentEvent.delta.x ) + 0.1 ) {
                                brushData.falloff -= 0.005f * currentEvent.delta.y;
                                brushData.falloff = Mathf.Clamp( brushData.falloff, 0.05f, 1 );
                                currentEvent.Use();
                            }


                        }
                        if ( currentEvent.button == 1 ) {
                            if ( currentEvent.shift ) {
                                brushData.opacity += 0.005f * currentEvent.delta.x;
                                brushData.opacity = Mathf.Clamp( brushData.opacity, 0.05f, 1 );
                                currentEvent.Use();
                            }
                        }

                    }


                }
                else {

                    if ( currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.X ) {
                        brushData.invert = !brushData.invert;
                        currentEvent.Use();
                    }

                    if ( currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.S ) {
                        brushData.mirror = !brushData.mirror;
                        currentEvent.Use();
                    }

                    for ( int i = 0; i < hits.Length; i++ ) {
                        if ( hits[i].collider == xfurCollider ) {
                            brushData.brushCenter = hits[i].point;
                            brushData.brushNormal = hits[i].normal;
                            brushData.hasContact = true;

                            break;
                        }
                    }


                    if ( currentEvent.type == EventType.MouseDrag && currentEvent.button == 0 && brushData.hasContact ) {
                        var r1 = HandleUtility.GUIPointToWorldRay( currentEvent.mousePosition );
                        var r2 = HandleUtility.GUIPointToWorldRay( currentEvent.mousePosition - currentEvent.delta );
                        var bDirection = Vector3.Normalize( r1.GetPoint( 10 ) - r2.GetPoint( 10 ) );

                        XFurDesignerPaint( bDirection );

                        currentEvent.Use();

                    }
                    
                    if ( currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && brushData.hasContact ) {
                        xfurUndo.RecordUndo( xfur, editFurProfile, brushData.activeTool );

                    }

                    if (currentEvent.type == EventType.MouseUp && currentEvent.button == 0 ) {
                        brushData.hasContact = false;
                    }

                }

                var hColor = Handles.color;

                Handles.color = new Color( 1, 1, 1, Mathf.Max( 0.1f, brushData.opacity ) );

                Handles.DrawWireDisc( brushData.brushCenter, brushData.brushNormal, brushData.size, 3 );
                Handles.DrawWireDisc( brushData.brushCenter, brushData.brushNormal, brushData.size * brushData.falloff, 3 );

                if ( brushData.mirror ) {
                    Handles.DrawWireDisc( xfur.transform.TransformPoint( Vector3.Reflect( xfur.transform.InverseTransformPoint(brushData.brushCenter), Vector3.right ) ), xfur.transform.TransformDirection( Vector3.Reflect( xfur.transform.InverseTransformDirection( brushData.brushNormal ), Vector3.right ) ), brushData.size, 3 );
                    Handles.DrawWireDisc( xfur.transform.TransformPoint( Vector3.Reflect( xfur.transform.InverseTransformPoint( brushData.brushCenter ), Vector3.right ) ), xfur.transform.TransformDirection( Vector3.Reflect( xfur.transform.InverseTransformDirection( brushData.brushNormal ), Vector3.right ) ), brushData.size * brushData.falloff, 3 );
                }



                Handles.color = hColor;

            }




        }



        #region XFur Studio Designer 3


        public class UndoManager {

            public int maxUndoSteps = 16;

            public List<RenderTexture> FurData0UndoSteps = new List<RenderTexture>();
            public List<RenderTexture> FurData1UndoSteps = new List<RenderTexture>();



            public void RecordUndo( XFurStudioInstance xfur, int onProfile, int dataToStore ) {

                if ( dataToStore > 1 ) {

                    if ( dataToStore == 6 ) {

                        if ( FurData1UndoSteps.Count == maxUndoSteps ) {
                            for ( int i = 0; i < FurData1UndoSteps.Count-2; i++ ) {
                                Graphics.Blit( FurData1UndoSteps[i + 1], FurData1UndoSteps[i] );
                            }

                            RenderTexture.ReleaseTemporary( FurData1UndoSteps[FurData1UndoSteps.Count - 1] );
                            FurData1UndoSteps.RemoveAt( FurData1UndoSteps.Count - 1 );

                        }

                        if ( xfur.FurDataProfiles[onProfile].FurData1 == null ) {
                            XFurStudioAPI.Groom( xfur, onProfile, xfur.transform.position + Vector3.up * 1000, Vector3.forward, 0, 0, 0, Vector3.zero );
                        }

                        FurData1UndoSteps.Add( RenderTexture.GetTemporary( xfur.FurDataProfiles[onProfile].FurData1.width, xfur.FurDataProfiles[onProfile].FurData1.height ) );
                        Graphics.Blit( xfur.FurDataProfiles[onProfile].FurData1, FurData1UndoSteps[FurData1UndoSteps.Count - 1] );
                    }
                    else {


                        if ( FurData0UndoSteps.Count == maxUndoSteps ) {
                            for ( int i = 0; i < FurData0UndoSteps.Count - 2; i++ ) {
                                Graphics.Blit( FurData0UndoSteps[i + 1], FurData0UndoSteps[i] );
                            }

                            RenderTexture.ReleaseTemporary( FurData0UndoSteps[FurData0UndoSteps.Count - 1] );
                            FurData0UndoSteps.RemoveAt( FurData0UndoSteps.Count - 1 );

                        }


                        if ( xfur.FurDataProfiles[onProfile].FurData0 == null ) {
                            XFurStudioAPI.Paint( xfur, XFurStudioAPI.PaintDataMode.FurMask, onProfile, xfur.transform.position + Vector3.up * 1000, Vector3.forward, 0, 0, 0, Color.white, Texture2D.whiteTexture );
                        }


                        FurData0UndoSteps.Add( RenderTexture.GetTemporary( xfur.FurDataProfiles[onProfile].FurData0.width, xfur.FurDataProfiles[onProfile].FurData0.height ) );
                        Graphics.Blit( xfur.FurDataProfiles[onProfile].FurData0, FurData0UndoSteps[FurData0UndoSteps.Count - 1] );
                    }

                }

            }


            public void Undo( XFurStudioInstance xfur, int onProfile, int dataToUndo ) {

                if ( dataToUndo > 1 ) {

                    if ( dataToUndo == 6 ) {
                        if ( FurData1UndoSteps.Count > 0 ) {
                            Graphics.Blit( FurData1UndoSteps[FurData1UndoSteps.Count-1], (RenderTexture)xfur.FurDataProfiles[onProfile].FurData1 );
                            RenderTexture.ReleaseTemporary( FurData1UndoSteps[FurData1UndoSteps.Count - 1] );
                            FurData1UndoSteps.RemoveAt( FurData1UndoSteps.Count - 1 );
                        }
                        

                    }
                    else {
                        if ( FurData0UndoSteps.Count > 0 ) {
                            Graphics.Blit( FurData0UndoSteps[FurData0UndoSteps.Count - 1], (RenderTexture)xfur.FurDataProfiles[onProfile].FurData0 );

                            RenderTexture.ReleaseTemporary( FurData0UndoSteps[FurData0UndoSteps.Count - 1] );
                            FurData0UndoSteps.RemoveAt( FurData0UndoSteps.Count - 1 );
                        }
                    }

                }

            }



            public void Clear() {

                foreach (RenderTexture rt in FurData0UndoSteps ) {
                    RenderTexture.ReleaseTemporary( rt );
                }

                foreach( RenderTexture rt in FurData1UndoSteps ) {
                    RenderTexture.ReleaseTemporary( rt );
                }

                FurData0UndoSteps.Clear();
                FurData1UndoSteps.Clear();

            }



        }




        void XFurDesignerPaint( Vector3 brushDirection ) {

            XFurStudioAPI.PaintDataMode paintMode = XFurStudioAPI.PaintDataMode.FurMask;

            var paintColor = brushData.invert ? Color.black : Color.white;

            switch ( brushData.activeTool ) {

                case 2:
                    paintMode = XFurStudioAPI.PaintDataMode.FurMask;
                    paintColor = brushData.invert ? Color.white : Color.black;
                    break;

                case 3:
                    paintMode = XFurStudioAPI.PaintDataMode.FurLength;
                    paintColor = brushData.invert ? Color.white : Color.black;
                    break;

                case 4:
                    paintMode = XFurStudioAPI.PaintDataMode.FurThickness;
                    paintColor = brushData.invert ? Color.white : Color.black;
                    break;

                case 5:
                    paintMode = XFurStudioAPI.PaintDataMode.FurOcclusion;
                    break;

                case 6:
                    XFurStudioAPI.Groom( xfur, editFurProfile, brushData.brushCenter, brushData.brushNormal, brushData.size, brushData.opacity, brushData.falloff, brushDirection, brushData.invert );

                    if ( brushData.mirror ) {
                        XFurStudioAPI.Groom( xfur, editFurProfile, xfur.transform.TransformPoint( Vector3.Reflect( xfur.transform.InverseTransformPoint( brushData.brushCenter ), Vector3.right ) ), xfur.transform.TransformDirection( Vector3.Reflect( xfur.transform.InverseTransformDirection( brushData.brushNormal ), Vector3.right ) ), brushData.size, brushData.opacity, brushData.falloff, Vector3.Reflect( brushDirection, Vector3.right ), brushData.invert );
                    }
                    return;

            }


            XFurStudioAPI.Paint( xfur, paintMode, editFurProfile, brushData.brushCenter, brushData.brushNormal, brushData.size, brushData.opacity, brushData.falloff, paintColor, Texture2D.whiteTexture );

            if ( brushData.mirror ) {
                XFurStudioAPI.Paint( xfur, paintMode, editFurProfile, xfur.transform.TransformPoint( Vector3.Reflect( xfur.transform.InverseTransformPoint( brushData.brushCenter ), Vector3.right ) ), xfur.transform.TransformDirection( Vector3.Reflect( xfur.transform.InverseTransformDirection( brushData.brushNormal ), Vector3.right ) ), brushData.size, brushData.opacity, brushData.falloff, paintColor, Texture2D.whiteTexture );
            }


        }


        public void ExportProfiles() {


            var path = EditorUtility.SaveFolderPanel( "Export Textures", "Assets/", "XFur Data Maps" );
            var temporaryOutput = RenderTexture.GetTemporary( 256 * Mathf.RoundToInt( Mathf.Pow( 2, exportResolution ) ), 256 * Mathf.RoundToInt( Mathf.Pow( 2, exportResolution ) ), 24, RenderTextureFormat.ARGB32 );
            var active = RenderTexture.active;
            RenderTexture.active = temporaryOutput;
            var outputTex = new Texture2D( RenderTexture.active.width, RenderTexture.active.height, TextureFormat.ARGB32, true );
            outputTex.wrapMode = TextureWrapMode.Clamp;


            xfur.GetFurData( editFurProfile, out XFurTemplate tempProfile );

            if ( !Directory.Exists( path + "/" + xfur.name.Replace( " ", "_" ) ) ) {
                Directory.CreateDirectory( path + "/" + xfur.name.Replace( " ", "_" ) );
            }


            var relativePath = path.Replace( Application.dataPath, "Assets/" );

            if ( relativePath != "Assets/" ) {
                relativePath += "/";
            }

            relativePath += xfur.name + "/";

            if ( xfur.FurDataProfiles[editFurProfile].FurData0 && xfur.FurDataProfiles[editFurProfile].FurData0 is RenderTexture ) {
                Graphics.Blit( xfur.FurDataProfiles[editFurProfile].FurData0, temporaryOutput );
                outputTex.ReadPixels( new Rect( 0, 0, outputTex.width, outputTex.height ), 0, 0 );
                outputTex.Apply();

                var pngData = outputTex.EncodeToPNG();

                if ( pngData != null ) {
                    File.WriteAllBytes( path + "/" + xfur.name.Replace( " ", "_" ) + "/" + xfur.name.Replace( " ", "_" ) + "_" + xfur.MainRenderer.materials[editFurProfile].name.Replace( " ", "_" ) + "_FurData0.png", pngData );
                    AssetDatabase.Refresh();
                    tempProfile.FurData0 = AssetDatabase.LoadAssetAtPath<Texture2D>( relativePath + xfur.name.Replace( " ", "_" ) + "_" + xfur.MainRenderer.materials[editFurProfile].name.Replace( " ", "_" ) + "_FurData0.png" );
                }
            }

            if ( xfur.FurDataProfiles[editFurProfile].FurData1 && xfur.FurDataProfiles[editFurProfile].FurData1 is RenderTexture ) {
                Graphics.Blit( xfur.FurDataProfiles[editFurProfile].FurData1, temporaryOutput );
                outputTex.ReadPixels( new Rect( 0, 0, outputTex.width, outputTex.height ), 0, 0 );
                outputTex.Apply();

                var pngData = outputTex.EncodeToPNG();

                if ( pngData != null ) {
                    File.WriteAllBytes( path + "/" + xfur.name.Replace( " ", "_" ) + "/" + xfur.name.Replace( " ", "_" ) + "_" + xfur.MainRenderer.materials[editFurProfile].name.Replace( " ", "_" ) + "_FurData1.png", pngData );
                    AssetDatabase.Refresh();
                    tempProfile.FurData1 = AssetDatabase.LoadAssetAtPath<Texture2D>( relativePath + xfur.name.Replace( " ", "_" ) + "_" + xfur.MainRenderer.materials[editFurProfile].name.Replace( " ", "_" ) + "_FurData1.png" );
                }
            }

            if ( xfur.FurDataProfiles[editFurProfile].FurColorMap && xfur.FurDataProfiles[editFurProfile].FurColorMap is RenderTexture ) {
                Graphics.Blit( xfur.FurDataProfiles[editFurProfile].FurColorMap, temporaryOutput );
                outputTex.ReadPixels( new Rect( 0, 0, outputTex.width, outputTex.height ), 0, 0 );
                outputTex.Apply();

                var pngData = outputTex.EncodeToPNG();

                if ( pngData != null ) {
                    File.WriteAllBytes( path + "/" + xfur.name.Replace( " ", "_" ) + "/" + xfur.name.Replace( " ", "_" ) + "_" + xfur.MainRenderer.materials[editFurProfile].name.Replace( " ", "_" ) + "_FurColorMap.png", pngData );
                    AssetDatabase.Refresh();
                    tempProfile.FurColorMap = AssetDatabase.LoadAssetAtPath<Texture2D>( relativePath + xfur.name.Replace( " ", "_" ) + "_" + xfur.MainRenderer.materials[editFurProfile].name.Replace( " ", "_" ) + "_FurColorMap.png" );
                }
            }

            if ( xfur.FurDataProfiles[editFurProfile].FurColorVariation && xfur.FurDataProfiles[editFurProfile].FurColorVariation is RenderTexture ) {
                Graphics.Blit( xfur.FurDataProfiles[editFurProfile].FurColorVariation, temporaryOutput );
                outputTex.ReadPixels( new Rect( 0, 0, outputTex.width, outputTex.height ), 0, 0 );
                outputTex.Apply();

                var pngData = outputTex.EncodeToPNG();

                if ( pngData != null ) {
                    File.WriteAllBytes( path + "/" + xfur.name.Replace( " ", "_" ) + "/" + xfur.name.Replace( " ", "_" ) + "_" + xfur.MainRenderer.materials[editFurProfile].name.Replace( " ", "_" ) + "_FurColorVariationMap.png", pngData );
                    AssetDatabase.Refresh();
                    tempProfile.FurColorVariation = AssetDatabase.LoadAssetAtPath<Texture2D>( relativePath + xfur.name.Replace( " ", "_" ) + "_" + xfur.MainRenderer.materials[editFurProfile].name.Replace( " ", "_" ) + "_FurColorVariationMap.png" );
                }
            }

            var asset = CreateInstance<XFurStudioFurProfile>();
            asset.FurTemplate = tempProfile;
            AssetDatabase.CreateAsset( asset, relativePath + xfur.name.Replace( " ", "_" ) + "_" + xfur.MainRenderer.materials[editFurProfile].name.Replace( " ", "_" ) + "_Profile.asset" );
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();


            var loadedProfile = (XFurStudioFurProfile)AssetDatabase.LoadAssetAtPath( relativePath + xfur.name.Replace( " ", "_" ) + "_" + xfur.MainRenderer.materials[editFurProfile].name.Replace( " ", "_" ) + "_Profile.asset", typeof( XFurStudioFurProfile ) );

            if (  loadedProfile != null ) {
                Debug.Log( "XFur Profile asset created successfully at " + relativePath );
                xfur.SetFurProfileAsset( editFurProfile, loadedProfile );
            }


            RenderTexture.active = active;

        }


        #endregion

        private void OnDisable() {

            if ( xfurCollider ) {
                DestroyImmediate( xfurCollider.gameObject, true );
            }

            ActiveEditorTracker.sharedTracker.isLocked = false;
            Selection.SetActiveObjectWithContext( xfur, xfur );

            if ( xfur.FurDataProfiles[editFurProfile].FurData0 is RenderTexture ) {
                var rt = xfur.FurDataProfiles[editFurProfile].FurData0;
                xfur.FurDataProfiles[editFurProfile].FurData0 = null;
                RenderTexture.ReleaseTemporary( (RenderTexture)rt );
            }

            if ( xfur.FurDataProfiles[editFurProfile].FurData1 is RenderTexture ) {
                var rt = xfur.FurDataProfiles[editFurProfile].FurData1;
                xfur.FurDataProfiles[editFurProfile].FurData1 = null;
                RenderTexture.ReleaseTemporary( (RenderTexture)rt );
            }

            xfurUndo.Clear();

        }




        void FurProfileSettings() {

            Undo.RecordObject( xfur, "Modified Fur Profile Properties" );


            if ( xfur.RenderingMode == XFurStudioInstance.XFurRenderingMode.BasicShells ) {
                CenteredLabel( "Basic Shells 'Skin' Properties" );

                GUILayout.Space( 16 );

                xfur.FurDataProfiles[editFurProfile].SkinColor = EditorGUILayout.ColorField( new GUIContent( "Skin Tint" ), xfur.FurDataProfiles[editFurProfile].SkinColor );
                xfur.FurDataProfiles[editFurProfile].SkinColorMap = ObjectField<Texture>( new GUIContent( "Skin Color Map", "The texture that controls the color applied for the skin under the fur in Basic Shells rendering mode" ), xfur.FurDataProfiles[editFurProfile].SkinColorMap );

                GUILayout.Space( 8 );
                xfur.FurDataProfiles[editFurProfile].SkinNormalMap = ObjectField<Texture>( new GUIContent( "Skin Normal Map", "The texture that controls the normals applied for the skin under the fur in Basic Shells rendering mode" ), xfur.FurDataProfiles[editFurProfile].SkinNormalMap );

                
                GUILayout.Space( 8 );
                xfur.FurDataProfiles[editFurProfile].SkinSmoothness = EditorGUILayout.Slider( new GUIContent( "Smoothness" ), xfur.FurDataProfiles[editFurProfile].SkinSmoothness, 0, 1 );
                xfur.FurDataProfiles[editFurProfile].SkinMetalMap = ObjectField<Texture>( new GUIContent( "Skin Metal / Smoothness Map", "The texture that controls the metal / smoothness applied for the skin under the fur in Basic Shells rendering mode" ), xfur.FurDataProfiles[editFurProfile].SkinMetalMap );



                GUILayout.Space( 24 );

            }

            CenteredLabel( "Common Properties" );

            GUILayout.Space( 16 );


            xfur.FurDataProfiles[editFurProfile].FurColorMap = ObjectField<Texture>( new GUIContent( "Fur Color Map", "The texture that controls the color / albedo applied over the whole fur surface" ), xfur.FurDataProfiles[editFurProfile].FurColorMap );

            xfur.FurDataProfiles[editFurProfile].FurMainTint = EditorGUILayout.ColorField( new GUIContent( "Fur Main Tint", "The main tint to be applied to the fur" ), xfur.FurDataProfiles[editFurProfile].FurMainTint );


            if ( xfur.FurDataProfiles[editFurProfile].FurColorTiling == Vector4.zero ) {
                xfur.FurDataProfiles[editFurProfile].FurColorTiling = new Vector4( xfur.FurDataProfiles[editFurProfile].FurBaseTiling, xfur.FurDataProfiles[editFurProfile].FurBaseTiling, 0, 0 );
            }

            GUILayout.Space( 8 );

            Vector2 tiling = Vector2Field( new GUIContent( "Color / Normals Tiling" ), new Vector2( xfur.FurDataProfiles[editFurProfile].FurColorTiling.x, xfur.FurDataProfiles[editFurProfile].FurColorTiling.y ) );
            Vector2 offset = Vector2Field( new GUIContent( "Color / Normals Offset" ), new Vector2( xfur.FurDataProfiles[editFurProfile].FurColorTiling.z, xfur.FurDataProfiles[editFurProfile].FurColorTiling.w ) );


            xfur.FurDataProfiles[editFurProfile].FurColorTiling = new Vector4( tiling.x, tiling.y, offset.x, offset.y );

            if ( xfur.FurDatabase.UseNormalmaps ) {
                GUILayout.Space( 8 );
                xfur.FurDataProfiles[editFurProfile].FurNormalmap = ObjectField<Texture>( new GUIContent( "Fur Normalmap", "The normalmap to be applied to the whole surface of the model" ), xfur.FurDataProfiles[editFurProfile].FurNormalmap );

            }

            if ( xfur.ShowAdvancedProperties ) {

                GUILayout.Space( 8 );

                xfur.FurDataProfiles[editFurProfile].FurUnderColorMod = EditorGUILayout.Slider( new GUIContent( "Fur Strands R Modifier", "Manually modifies the color boost applied to the first fur pass (fur strands texture's red channel" ), xfur.FurDataProfiles[editFurProfile].FurUnderColorMod, 0, 0.65f );
                xfur.FurDataProfiles[editFurProfile].FurOverColorMod = EditorGUILayout.Slider( new GUIContent( "Fur Strands G Modifier", "Manually modifies the color boost applied to the second fur pass (fur strands texture's green channel" ), xfur.FurDataProfiles[editFurProfile].FurOverColorMod, 0.25f, 1.5f );

                GUILayout.Space( 8 );

                xfur.FurDataProfiles[editFurProfile].FurTransmission = EditorGUILayout.Slider( new GUIContent( "Fur Transmission", "The translucency of the fur" ), xfur.FurDataProfiles[editFurProfile].FurTransmission, 0, 1 );

                xfur.FurDataProfiles[editFurProfile].FurSpecularTint = EditorGUILayout.ColorField( new GUIContent( "Fur Specular Tint", "The main tint to be applied to the fur's specularity" ), xfur.FurDataProfiles[editFurProfile].FurSpecularTint );

                GUILayout.Space( 8 );

                xfur.FurDataProfiles[editFurProfile].FurRim = EditorGUILayout.ColorField( new GUIContent( "Fur Rim Tint", "The main tint to be applied to the fur's rim lighting" ), xfur.FurDataProfiles[editFurProfile].FurRim );

                xfur.FurDataProfiles[editFurProfile].FurRimPower = EditorGUILayout.Slider( new GUIContent( "Fur Rim Power" ), xfur.FurDataProfiles[editFurProfile].FurRimPower, 0.1f, 10 );

                xfur.FurDataProfiles[editFurProfile].FurRimBoost = EditorGUILayout.Slider( new GUIContent( "Fur Rim Boost", "Applies an additional color boost to the fur's rim lighting effect" ), xfur.FurDataProfiles[editFurProfile].FurRimBoost, 1.0f, 3.0f );

            }



            if ( !xfur.FurDatabase.MobileMode ) {
                GUILayout.Space( 8 );
                xfur.FurDataProfiles[editFurProfile].FurColorVariation = ObjectField<Texture>( new GUIContent( "Color Variation Mask", "The texture that controls four additional coloring variations to be applied over the fur, either all four to the whole fur or two to the undercoat and two to the overcoat by using the four color channels." ), xfur.FurDataProfiles[editFurProfile].FurColorVariation );
            }

            if ( xfur.FurDataProfiles[editFurProfile].FurColorVariation ) {

                GUILayout.Space( 8 );
                xfur.FurDataProfiles[editFurProfile].FurColorA = EditorGUILayout.ColorField( new GUIContent( "Fur Color A", "The fur color to be applied on the red channel of the Color Variation map" ), xfur.FurDataProfiles[editFurProfile].FurColorA );
                xfur.FurDataProfiles[editFurProfile].FurColorB = EditorGUILayout.ColorField( new GUIContent( "Fur Color B", "The fur color to be applied on the green channel of the Color Variation map" ), xfur.FurDataProfiles[editFurProfile].FurColorB );
                xfur.FurDataProfiles[editFurProfile].FurColorC = EditorGUILayout.ColorField( new GUIContent( "Fur Color C", "The fur color to be applied on the blue channel of the Color Variation map" ), xfur.FurDataProfiles[editFurProfile].FurColorC );
                xfur.FurDataProfiles[editFurProfile].FurColorD = EditorGUILayout.ColorField( new GUIContent( "Fur Color D", "The fur color to be applied on the alpha channel of the Color Variation map" ), xfur.FurDataProfiles[editFurProfile].FurColorD );
                GUILayout.Space( 8 );

            }


            GUILayout.Space( 8 );

            xfur.FurDataProfiles[editFurProfile].FurData0 = ObjectField( new GUIContent( "Fur Data Map", "The texture that controls the parameters of the fur :\n\n R = fur mask\n G = length\n B = occlusion\n A = thickness" ), xfur.FurDataProfiles[editFurProfile].FurData0 );
            xfur.FurDataProfiles[editFurProfile].FurData1 = ObjectField( new GUIContent( "Fur Grooming Map", "The texture that controls the direction of the fur :\n\n RGB = absolute fur direction half-normalized in tangent space" ), xfur.FurDataProfiles[editFurProfile].FurData1 );

            GUILayout.Space( 12 );

            if ( xfur.FurDataProfiles[editFurProfile].FurData1 )
                xfur.FurDataProfiles[editFurProfile].FurGroomStrength = EditorGUILayout.Slider( new GUIContent( "Fur Grooming Strength" ), xfur.FurDataProfiles[editFurProfile].FurGroomStrength, 0, 1f );


            xfur.FurDataProfiles[editFurProfile].FurLength = EditorGUILayout.Slider( new GUIContent( "Fur Length", "The maximum overall length of the fur. This will be multiplied by the actual fur profile length and the length painted in XFur Studio™ - Designer" ), xfur.FurDataProfiles[editFurProfile].FurLength, 0.01f, 1 );

            GUILayout.Space( 8 );
            xfur.FurDataProfiles[editFurProfile].FurThickness = EditorGUILayout.Slider( new GUIContent( "Fur Thickness", "The maximum overall thickness of the fur. This will be multiplied by the actual fur profile thickness and the thickness painted in XFur Studio™ - Designer" ), xfur.FurDataProfiles[editFurProfile].FurThickness, 0.01f, 1 );
            xfur.FurDataProfiles[editFurProfile].FurThicknessCurve = EditorGUILayout.Slider( new GUIContent( "Thickness Curve", "How the fur strands' thickness bias will change from the root to the top of each strand" ), xfur.FurDataProfiles[editFurProfile].FurThicknessCurve, 0, 1 );
            GUILayout.Space( 8 );

            xfur.FurDataProfiles[editFurProfile].FurShadowsTint = EditorGUILayout.ColorField( new GUIContent( "Occlusion Tint" ), xfur.FurDataProfiles[editFurProfile].FurShadowsTint );

            xfur.FurDataProfiles[editFurProfile].FurOcclusion = EditorGUILayout.Slider( new GUIContent( "Fur Occlusion / Shadowing", "The shadowing applied over the surface of the fur strands as a simple occlusion pass. Multiplied by the per-profile occlusion value and the one painted through XFur Studio™ - Designer" ), xfur.FurDataProfiles[editFurProfile].FurOcclusion, 0, 1 );
            xfur.FurDataProfiles[editFurProfile].FurOcclusionCurve = EditorGUILayout.Slider( new GUIContent( "Fur Occlusion Curve", "How the shadowing / occlusion of the fur will go from the root to the tip of each strand" ), xfur.FurDataProfiles[editFurProfile].FurOcclusionCurve, 0, 1 );

            GUILayout.Space( 8 );
            xfur.FurDataProfiles[editFurProfile].FurSmoothness = EditorGUILayout.Slider( new GUIContent( "Fur Smoothness" ), xfur.FurDataProfiles[editFurProfile].FurSmoothness, 0, 1 );
            GUILayout.Space( 8 );

            if ( xfur.FurDataProfiles[editFurProfile].UseEmissiveFur ) {

                GUILayout.Space( 8 );

                CenteredLabel( "Emissive Fur" );

                GUILayout.Space( 16 );

                xfur.FurDataProfiles[editFurProfile].FurEmissionColor = EditorGUILayout.ColorField( new GUIContent( "Emissive Color" ), xfur.FurDataProfiles[editFurProfile].FurEmissionColor, true, false, true );
                xfur.FurDataProfiles[editFurProfile].FurEmissionMap = ObjectField<Texture>( new GUIContent( "Emission Map" ), xfur.FurDataProfiles[editFurProfile].FurEmissionMap );

                GUILayout.Space( 16 );

            }


            if ( xfur.FurDataProfiles[editFurProfile].UseCurlyFur ) {

                CenteredLabel( "Curly Fur" );

                GUILayout.Space( 16 );

                xfur.FurDataProfiles[editFurProfile].FurCurlAmountX = EditorGUILayout.Slider( new GUIContent( "Curl Amount X" ), xfur.FurDataProfiles[editFurProfile].FurCurlAmountX, 0, 1 );
                xfur.FurDataProfiles[editFurProfile].FurCurlAmountY = EditorGUILayout.Slider( new GUIContent( "Curl Amount Y" ), xfur.FurDataProfiles[editFurProfile].FurCurlAmountY, 0, 1 );
                xfur.FurDataProfiles[editFurProfile].FurCurlSizeX = EditorGUILayout.Slider( new GUIContent( "Curl Size X" ), xfur.FurDataProfiles[editFurProfile].FurCurlSizeX, 0, 0.1f );
                xfur.FurDataProfiles[editFurProfile].FurCurlSizeY = EditorGUILayout.Slider( new GUIContent( "Curl Size Y" ), xfur.FurDataProfiles[editFurProfile].FurCurlSizeY, 0, 0.1f );

                GUILayout.Space( 16 );

            }

            CenteredLabel( "Per Instance Wind Settings" );

            GUILayout.Space( 24 );

            xfur.FurDataProfiles[editFurProfile].SelfWindStrength = EditorGUILayout.Slider( new GUIContent( "Wind Strength Multiplier", "The value by which the global wind strength will be multiplied, useful to fine tune the overall wind strength applied over this instance" ), xfur.FurDataProfiles[editFurProfile].SelfWindStrength, 0.0f, 8.0f );

            GUILayout.Space( 32 );



        }



        void SaveLoadChanges() {

            CenteredLabel( "Save & Load Data" );

            GUILayout.Space( 16 );

            exportResolution = EditorGUILayout.Popup( "Export Resolution", exportResolution, new string[] { "256x256 px", "512x512 px", "1024x1024 px", "2048x2048 px" }, pidiSkin2.button );

            GUILayout.Space( 16 );


            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if ( StandardButton( "Load Profile", 200 ) ) {
                var path = EditorUtility.OpenFilePanel( "Load Fur Profile Asset", "Assets/", "asset" );

                path = path.Replace( Application.dataPath, "Assets" );

                var asset = (XFurStudioFurProfile)AssetDatabase.LoadAssetAtPath( path, typeof( XFurStudioFurProfile ) );

                if ( asset && asset.GetType() == typeof( XFurStudioFurProfile ) ) {

                    if ( xfur.FurDataProfiles[editFurProfile].FurData0 is RenderTexture || xfur.FurDataProfiles[editFurProfile].FurData1 is RenderTexture ) {
                        if ( !EditorUtility.DisplayDialog( "Unsaved Data detected", "You have unsaved work done for this XFur Studio Instance. Loading a Fur profile will overwritte this data in an irreversible way. Do you wish to continue?", "Yes", "No" ) ) {
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();
                            return;
                        }
                        else {
                            if ( xfur.FurDataProfiles[editFurProfile].FurData0 is RenderTexture ) {
                                RenderTexture.ReleaseTemporary( (RenderTexture)xfur.FurDataProfiles[editFurProfile].FurData0 );
                                xfur.FurDataProfiles[editFurProfile].FurData0 = null;
                            }

                            if ( xfur.FurDataProfiles[editFurProfile].FurData1 is RenderTexture ) {
                                RenderTexture.ReleaseTemporary( (RenderTexture)xfur.FurDataProfiles[editFurProfile].FurData1 );
                                xfur.FurDataProfiles[editFurProfile].FurData1 = null;
                            }
                        }
                    }

                    xfur.SetFurProfileAsset( editFurProfile, asset );
                    Debug.Log( "Successfully loaded XFur Profile" );
                }
                else {
#if XFURDESKTOP_LEGACY

                                         var legacyAsset = (XFurStudio.XFur_CoatingProfile)AssetDatabase.LoadAssetAtPath( path, typeof( XFurStudio.XFur_CoatingProfile ) );

                                         if ( legacyAsset && legacyAsset.GetType() == typeof( XFurStudio.XFur_CoatingProfile ) ) {
                                             xfur.LoadLegacyXFurProfile( i, legacyAsset );
                                             Debug.Log( "Successfully loaded Legacy XFur Profile" );
                                         }
#endif

#if XFurStudioMobile_LEGACY

                                         var legacyAsset = (XFurStudioMobile.XFur_CoatingProfile)AssetDatabase.LoadAssetAtPath( path, typeof( XFurStudioMobile.XFur_CoatingProfile ) );

                                         if ( legacyAsset && legacyAsset.GetType() == typeof( XFurStudioMobile.XFur_CoatingProfile ) ) {
                                             xfur.LoadLegacyXFurProfile( i, legacyAsset );
                                             Debug.Log( "Successfully loaded Legacy XFur Profile" );
                                         }
#endif

                }



            }

            GUILayout.Space( 32 );

            if ( StandardButton( "Export Profile", 200 ) ) {

                ExportProfiles();

            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            GUILayout.Space( 32 );

        }


        void MainFurSettings() {

            Undo.RecordObject( xfur, "Modified Main Fur Settings" );

            CenteredLabel( "Main Fur Settings" );

            GUILayout.Space( 16 );

            xfur.BetaMode = EnableDisableToggle( new GUIContent( "Experimental Features", "Enables or disables the support for experimental features. WARNING : Experimental features are not intended for use in finished projects as they are not yet production ready. They are available for testing purposes and are subject to change between versions" ), xfur.BetaMode );
            GUILayout.Space( 4 );
            xfur.ShowAdvancedProperties = EnableDisableToggle( new GUIContent( "Show Advanced Properties", "Exposes advanced fur properties for editing." ), xfur.ShowAdvancedProperties );

            GUILayout.Space( 16 );



            CenteredLabel( "Basic Fur Appearance" );

            GUILayout.Space( 16 );

            xfur.FurDataProfiles[editFurProfile].FurStrandsAsset = (XFurStudioStrandsAsset)EditorGUILayout.ObjectField( new GUIContent( "Fur Strands Asset", "The texture map used to generate the fur strands for this fur profile" ), xfur.FurDataProfiles[editFurProfile].FurStrandsAsset, typeof( XFurStudioStrandsAsset ), false );

            xfur.FurDataProfiles[editFurProfile].FurSamples = EditorGUILayout.IntSlider( new GUIContent( "Fur Samples", "The amount of samples used to render the fur. More samples give better results, but may result in a reduced performance (especially when using ss" ), xfur.FurDataProfiles[editFurProfile].FurSamples, 4, 128 );


            if ( xfur.FurDataProfiles[editFurProfile].FurStrandsAsset ) {
                xfur.FurDataProfiles[editFurProfile].FurUVTiling = FloatField( new GUIContent( "Fur Strands Tiling", "The tiling (UV size) to be applied to the fur strands" ), xfur.FurDataProfiles[editFurProfile].FurUVTiling );
            }

            GUILayout.Space( 16 );

            if ( xfur.RenderingMode == XFurStudioInstance.XFurRenderingMode.XFShells ) {

                if ( xfur.FurDatabase.HasDoubleSide )
                    xfur.FurDataProfiles[editFurProfile].DoubleSided = EnableDisableToggle( new GUIContent( "Double Sided Fur" ), xfur.FurDataProfiles[editFurProfile].DoubleSided );

                GUILayout.Space( 16 );

                CenteredLabel( "Fur Lighting" );

                GUILayout.Space( 16 );

                xfur.FurDataProfiles[editFurProfile].ProbeUse = EnableDisableToggle( new GUIContent( "Use Light Probes" ), xfur.FurDataProfiles[editFurProfile].ProbeUse, true );
                xfur.FurDataProfiles[editFurProfile].CastShadows = EnableDisableToggle( new GUIContent( "Cast Shadows" ), xfur.FurDataProfiles[editFurProfile].CastShadows, true );
                xfur.FurDataProfiles[editFurProfile].ReceiveShadows = EnableDisableToggle( new GUIContent( "Receive Shadows" ), xfur.FurDataProfiles[editFurProfile].ReceiveShadows, true );
 


            }


            GUILayout.Space( 16 );

            CenteredLabel( "Additional Features" );

            GUILayout.Space( 16 );

            xfur.FurDataProfiles[editFurProfile].UseCurlyFur = EnableDisableToggle( new GUIContent( "Curly Fur" ), xfur.FurDataProfiles[editFurProfile].UseCurlyFur );

            xfur.FurDataProfiles[editFurProfile].UseEmissiveFur = EnableDisableToggle( new GUIContent( "Emissive Fur" ), xfur.FurDataProfiles[editFurProfile].UseEmissiveFur );

            GUILayout.Space( 32 );

        }







        void BrushSettingsDrawer() {

            string activeToolName = "Active Tool";

            switch ( brushData.activeTool ) {

                case 2:
                    activeToolName = "Fur Mask -";
                    break;

                case 3:
                    activeToolName = "Fur Length -";
                    break;


                case 4:
                    activeToolName = "Fur Thickness -";
                    break;


                case 5:
                    activeToolName = "Fur Shadowing -";
                    break;


                case 6:
                    activeToolName = "Grooming -";
                    break;

            }

            CenteredLabel( activeToolName + " Brush Settings" );

            GUILayout.Space( 16 );

            EditorGUILayout.HelpBox( "Please remember to save your work before closing this window. Any unsaved data will be lost.", MessageType.Warning );

            GUILayout.Space( 16 );

            brushData.fineTuneBrush = EditorGUILayout.Toggle( "Fine Tune Brush Size", brushData.fineTuneBrush );
            GUILayout.Space( 4 );

            if ( brushData.activeTool > 1 && brushData.activeTool < 7 ) {
                brushData.invert = EnableDisableToggle( new GUIContent( "Inverted Effect" ), brushData.invert );
            }

            brushData.mirror = EnableDisableToggle( new GUIContent( "Symmetry Mode" ), brushData.mirror );

            GUILayout.Space( 10 );
            brushData.size = EditorGUILayout.Slider( "Size", brushData.size, brushData.minMaxSize.x, brushData.minMaxSize.y );

            if ( brushData.fineTuneBrush ) {
                brushData.minMaxSize = Vector2Field( new GUIContent( "Min/Max Size" ), brushData.minMaxSize );
                GUILayout.Space( 4 );
            }

            brushData.falloff = EditorGUILayout.Slider( "Falloff", brushData.falloff, 0.05f, 1.0f );
            brushData.opacity = EditorGUILayout.Slider( "Opacity", brushData.opacity, 0.05f, 1.0f );


            GUILayout.Space( 24 );

            var lstyle = new GUIStyle();
            lstyle.fontStyle = FontStyle.Italic;
            lstyle.fontSize = 10;
            lstyle.normal.textColor = new Color( 1, 1, 1, 0.7f );
            lstyle.alignment = TextAnchor.MiddleLeft;

            GUILayout.Label( "Hold Shift + Horizontal Left Click Drag for brush size.", lstyle, GUILayout.Height( 14 ) );
            GUILayout.Label( "Hold Shift + Vertical Left Click Drag for brush falloff.", lstyle, GUILayout.Height( 14 ) );
            GUILayout.Label( "Hold Shift + Horizontal Right Click Drag for brush opacity.", lstyle, GUILayout.Height( 14 ) );

            GUILayout.Space( 8 );

            GUILayout.Label( "Press F to center the Scene View around the XFur Studio Instance.", lstyle, GUILayout.Height( 14 ) );
            GUILayout.Label( "Press X to invert the brush mode, when available.", lstyle, GUILayout.Height( 14 ) );
            GUILayout.Label( "Press S to switch symmetry mode on and off.", lstyle, GUILayout.Height( 14 ) );


            GUILayout.Space( 12 );

            GUILayout.Label( "Press Ctrl+Z to Undo. Please beware that a Redo function is not implemented.", lstyle, GUILayout.Height( 14 ) );

            GUILayout.Space( 32 );

        }


        void CurrentToolDrawer() {


            GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal( pidiSkin2.box, GUILayout.MaxWidth( 512 ) ); GUILayout.Space( 20 );
            GUILayout.BeginVertical();

            GUILayout.Space( 16 );

            switch ( brushData.activeTool ) {

                case 0:
                    MainFurSettings();
                    break;

                case 1:
                    FurProfileSettings();
                    break;

                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    BrushSettingsDrawer();
                    break;

                case 7:
                    SaveLoadChanges();
                    break;
            }


            GUILayout.EndVertical();
            GUILayout.Space( 20 ); GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();

        }


        void ToolsBox() {

            var smStyle = new GUIStyle();
            smStyle.fontSize = 10;
            smStyle.normal.textColor = new Color( 1, 1, 1, 0.8f );

            GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();
            GUILayout.BeginVertical( pidiSkin2.box, GUILayout.MaxWidth( 400 ) );

            GUILayout.Space( 12 );

            CenteredLabel( "Tools" );

            GUILayout.Space( 12 );

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();


            GUILayout.BeginVertical( GUILayout.Width( 48 ) );
            if ( GUILayout.Button( genSettings, brushData.activeTool == 0 ? pidiSkin2.customStyles[6] : pidiSkin2.button, GUILayout.Width( 48 ), GUILayout.Height( 48 ) ) ) {
                brushData.activeTool = 0;
            }
            GUILayout.Space( 4 );
            CenteredLabel( "Settings", smStyle );
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            GUILayout.BeginVertical( GUILayout.Width( 48 ) );
            if ( GUILayout.Button( genProps, brushData.activeTool == 1 ? pidiSkin2.customStyles[6] : pidiSkin2.button, GUILayout.Width( 48 ), GUILayout.Height( 48 ) ) ) {
                brushData.activeTool = 1;
            }
            GUILayout.Space( 4 );
            CenteredLabel( "Properties", smStyle );
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            GUILayout.BeginVertical( GUILayout.Width( 48 ) );
            if ( GUILayout.Button( brushShave, brushData.activeTool == 2 ? pidiSkin2.customStyles[6] : pidiSkin2.button, GUILayout.Width( 48 ), GUILayout.Height( 48 ) ) ) {
                brushData.activeTool = 2;
            }
            GUILayout.Space( 4 );
            CenteredLabel( "Fur Mask", smStyle );
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            GUILayout.BeginVertical( GUILayout.Width( 48 ) );
            if ( GUILayout.Button( brushLen, brushData.activeTool == 3 ? pidiSkin2.customStyles[6] : pidiSkin2.button, GUILayout.Width( 48 ), GUILayout.Height( 48 ) ) ) {
                brushData.activeTool = 3;
            }
            GUILayout.Space( 4 );
            CenteredLabel( "Fur Length", smStyle );
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            GUILayout.Space( 12 );

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();


            GUILayout.BeginVertical( GUILayout.Width( 48 ) );
            if ( GUILayout.Button( brushThick, brushData.activeTool == 4 ? pidiSkin2.customStyles[6] : pidiSkin2.button, GUILayout.Width( 48 ), GUILayout.Height( 48 ) ) ) {
                brushData.activeTool = 4;
            }
            GUILayout.Space( 4 );
            CenteredLabel( "Thinness", smStyle );
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            GUILayout.BeginVertical( GUILayout.Width( 48 ) );
            if ( GUILayout.Button( brushOcc, brushData.activeTool == 5 ? pidiSkin2.customStyles[6] : pidiSkin2.button, GUILayout.Width( 48 ), GUILayout.Height( 48 ) ) ) {
                brushData.activeTool = 5;
            }
            GUILayout.Space( 4 );
            CenteredLabel( "Shadows", smStyle );
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            GUILayout.BeginVertical( GUILayout.Width( 48 ) );
            if ( GUILayout.Button( brushGroom, brushData.activeTool == 6 ? pidiSkin2.customStyles[6] : pidiSkin2.button, GUILayout.Width( 48 ), GUILayout.Height( 48 ) ) ) {
                brushData.activeTool = 6;
            }
            GUILayout.Space( 4 );
            CenteredLabel( "Grooming", smStyle );
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            GUILayout.BeginVertical( GUILayout.Width( 48 ) );
            if ( GUILayout.Button( exportData, brushData.activeTool == 7 ? pidiSkin2.customStyles[6] : pidiSkin2.button, GUILayout.Width( 48 ), GUILayout.Height( 48 ) ) ) {
                brushData.activeTool = 7;
            }
            GUILayout.Space( 4 );
            CenteredLabel( "Save/Load", smStyle );
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            GUILayout.Space( 12 );

            GUILayout.EndVertical();
            GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();

        }


        void OnGUI() {

            if ( !xfur ) {
                Close();
            }

            Repaint();

            scrollView = GUILayout.BeginScrollView( scrollView );

            EditorGUIUtility.labelWidth = 200;

            GUILayout.Space( 12 );

            AssetLogoAndVersion();

            GUILayout.Space( 24 );

            GUILayout.BeginHorizontal(); GUILayout.Space( 20 );
            GUILayout.BeginVertical();

            if ( xfur ) {

                ToolsBox();

                GUILayout.Space( 32 );

                CurrentToolDrawer();

                GUILayout.Space( 32 );

                /*

                CenteredLabel( "Brush Settings" );

                GUILayout.Space( 12 );

                if ( brushData.fineTuneBrush ) {

                    brushData.minMaxSize = EditorGUILayout.Vector2Field( "Min. Max. Brush Size", brushData.minMaxSize );
                }

                brushData.mirror = EditorGUILayout.Toggle( "Mirror", brushData.mirror );
                brushData.invert = EditorGUILayout.Toggle( "Invert", brushData.invert );

                GUILayout.Space( 4 );

                brushData.size = EditorGUILayout.Slider( "Size", brushData.size, brushData.minMaxSize.x, brushData.minMaxSize.y );
                brushData.falloff = EditorGUILayout.Slider( "Falloff", brushData.falloff, 0.01f, 1.0f );
                brushData.opacity = EditorGUILayout.Slider( "Opacity", brushData.opacity, 0.01f, 1.0f );
                */
            }
            else {
                GUILayout.Space( 32 );
            }

            GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();

            var lStyle = new GUIStyle( EditorStyles.label );
            lStyle.fontStyle = FontStyle.Italic;
            lStyle.fontSize = 8;

            GUILayout.Label( "Copyright© 2017-2021,   Jorge Pinal N.", lStyle );

            GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();


            GUILayout.Space( 32 );


            GUILayout.EndVertical();
            GUILayout.Space( 20 ); GUILayout.EndHorizontal();


            GUILayout.EndScrollView();


        }





        #region PIDI 2020 EDITOR



        public void HelpBox( string message, MessageType messageType ) {
            EditorGUILayout.HelpBox( message, messageType );
        }




        /// <summary>
        /// Draws a standard object field in the PIDI 2020 style
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="label"></param>
        /// <param name="inputObject"></param>
        /// <param name="allowSceneObjects"></param>
        /// <returns></returns>
        public T ObjectField<T>( GUIContent label, T inputObject, bool allowSceneObjects = true ) where T : UnityEngine.Object {

            GUILayout.Space( 4 );
            GUILayout.BeginHorizontal();
            GUILayout.Label( label, pidiSkin2.label, GUILayout.Width( EditorGUIUtility.labelWidth ) );
            inputObject = (T)EditorGUILayout.ObjectField( inputObject, typeof( T ), allowSceneObjects );
            GUILayout.EndHorizontal();
            GUILayout.Space( 4 );
            return inputObject;
        }


        /// <summary>
        /// Draws a centered button in the standard PIDI 2020 editor style
        /// </summary>
        /// <param name="label"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public bool CenteredButton( string label, float width ) {
            GUILayout.Space( 2 );
            GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();
            var tempBool = GUILayout.Button( label, EditorGUIUtility.isProSkin ? pidiSkin2.button : EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector ).button, GUILayout.MaxWidth( width ) );
            GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();
            GUILayout.Space( 2 );
            return tempBool;
        }

        /// <summary>
        /// Draws a button in the standard PIDI 2020 editor style
        /// </summary>
        /// <param name="label"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public bool StandardButton( string label, float width ) {
            var tempBool = GUILayout.Button( label, EditorGUIUtility.isProSkin ? pidiSkin2.button : EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector ).button, GUILayout.MaxWidth( width ) );
            return tempBool;
        }


        /// <summary>
        /// Draws the asset's logo and its current version
        /// </summary>
        public void AssetLogoAndVersion() {

            GUILayout.BeginVertical( xfurStudioLogo, pidiSkin2 ? pidiSkin2.customStyles[1] : null );
            GUILayout.Space( 45 );
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label( xfur.Version, pidiSkin2.customStyles[2] );
            GUILayout.Space( 6 );
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Draws a label centered in the Editor window
        /// </summary>
        /// <param name="label"></param>
        public void CenteredLabel( string label, GUIStyle style = null ) {

            GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();
            GUILayout.Label( label, style == null ? EditorStyles.boldLabel : style );
            GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();

        }

        /// <summary>
        /// Begins a custom centered group similar to a foldout that can be expanded with a button
        /// </summary>
        /// <param name="label"></param>
        /// <param name="groupFoldState"></param>
        /// <returns></returns>
        public bool BeginCenteredGroup( string label, ref bool groupFoldState ) {

            if ( GUILayout.Button( label, EditorGUIUtility.isProSkin ? pidiSkin2.button : EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector ).button ) ) {
                groupFoldState = !groupFoldState;
            }
            GUILayout.BeginHorizontal(); GUILayout.Space( 18 );
            GUILayout.BeginVertical();
            return groupFoldState;
        }


        /// <summary>
        /// Finishes a centered group
        /// </summary>
        public void EndCenteredGroup() {
            GUILayout.EndVertical();
            GUILayout.Space( 18 );
            GUILayout.EndHorizontal();
        }



        /// <summary>
        /// Custom integer field following the PIDI 2020 editor skin
        /// </summary>
        /// <param name="label"></param>
        /// <param name="currentValue"></param>
        /// <returns></returns>
        public int IntField( GUIContent label, int currentValue ) {

            GUILayout.Space( 2 );
            GUILayout.BeginHorizontal();
            GUILayout.Label( label, pidiSkin2.label, GUILayout.Width( EditorGUIUtility.labelWidth ) );
            currentValue = EditorGUILayout.IntField( currentValue );
            GUILayout.EndHorizontal();
            GUILayout.Space( 2 );

            return currentValue;
        }

        /// <summary>
        /// Custom float field following the PIDI 2020 editor skin
        /// </summary>
        /// <param name="label"></param>
        /// <param name="currentValue"></param>
        /// <returns></returns>
        public float FloatField( GUIContent label, float currentValue ) {

            GUILayout.Space( 2 );
            GUILayout.BeginHorizontal();
            GUILayout.Label( label, GUILayout.Width( EditorGUIUtility.labelWidth ) );
            currentValue = EditorGUILayout.FloatField( currentValue );
            GUILayout.EndHorizontal();
            GUILayout.Space( 2 );

            return currentValue;
        }


        /// <summary>
        /// Custom text field following the PIDI 2020 editor skin
        /// </summary>
        /// <param name="label"></param>
        /// <param name="currentValue"></param>
        /// <returns></returns>
        public string TextField( GUIContent label, string currentValue ) {

            GUILayout.Space( 2 );
            GUILayout.BeginHorizontal();
            GUILayout.Label( label, pidiSkin2.label, GUILayout.Width( EditorGUIUtility.labelWidth ) );
            currentValue = EditorGUILayout.TextField( currentValue );
            GUILayout.EndHorizontal();
            GUILayout.Space( 2 );

            return currentValue;
        }


        public Vector2 Vector2Field( GUIContent label, Vector2 currentValue ) {

            GUILayout.Space( 2 );
            GUILayout.BeginHorizontal();
            GUILayout.Label( label, pidiSkin2.label, GUILayout.Width( EditorGUIUtility.labelWidth ) );
            currentValue.x = EditorGUILayout.FloatField( currentValue.x );
            GUILayout.Space( 8 );
            currentValue.y = EditorGUILayout.FloatField( currentValue.y );
            GUILayout.EndHorizontal();
            GUILayout.Space( 2 );

            return currentValue;

        }

        public Vector3 Vector3Field( GUIContent label, Vector3 currentValue ) {

            GUILayout.Space( 2 );
            GUILayout.BeginHorizontal();
            GUILayout.Label( label, pidiSkin2.label, GUILayout.Width( EditorGUIUtility.labelWidth ) );
            currentValue.x = EditorGUILayout.FloatField( currentValue.x );
            GUILayout.Space( 8 );
            currentValue.y = EditorGUILayout.FloatField( currentValue.y );
            GUILayout.Space( 8 );
            currentValue.z = EditorGUILayout.FloatField( currentValue.z );
            GUILayout.EndHorizontal();
            GUILayout.Space( 2 );

            return currentValue;

        }


        public Vector4 Vector4Field( GUIContent label, Vector4 currentValue ) {

            GUILayout.Space( 2 );
            GUILayout.BeginHorizontal();
            GUILayout.Label( label, pidiSkin2.label, GUILayout.Width( EditorGUIUtility.labelWidth ) );
            currentValue.x = EditorGUILayout.FloatField( currentValue.x );
            GUILayout.Space( 8 );
            currentValue.y = EditorGUILayout.FloatField( currentValue.y );
            GUILayout.Space( 8 );
            currentValue.z = EditorGUILayout.FloatField( currentValue.z );
            GUILayout.Space( 8 );
            currentValue.w = EditorGUILayout.FloatField( currentValue.w );
            GUILayout.EndHorizontal();
            GUILayout.Space( 2 );

            return currentValue;

        }


        /// <summary>
        /// Draw a custom popup field in the PIDI 2020 style
        /// </summary>
        /// <param name="label"></param>
        /// <param name="toggleValue"></param>
        /// <returns></returns>
        public int PopupField( GUIContent label, int selected, string[] options ) {


            GUILayout.Space( 2 );
            GUILayout.BeginHorizontal();
            GUILayout.Label( label, pidiSkin2.label, GUILayout.Width( EditorGUIUtility.labelWidth ) );
            selected = EditorGUILayout.Popup( selected, options, EditorGUIUtility.isProSkin ? pidiSkin2.button : EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector ).button );
            GUILayout.EndHorizontal();
            GUILayout.Space( 2 );
            return selected;
        }



        /// <summary>
        /// Draw a custom toggle that instead of using a check box uses an Enable/Disable drop down menu
        /// </summary>
        /// <param name="label"></param>
        /// <param name="toggleValue"></param>
        /// <returns></returns>
        public bool EnableDisableToggle( GUIContent label, bool toggleValue, bool trueFalseToggle = false, params GUILayoutOption[] options ) {

            int option = toggleValue ? 1 : 0;

            GUILayout.Space( 4 );

            if ( label != null ) {

                if ( trueFalseToggle ) {
                    option = EditorGUILayout.Popup( label, option, new GUIContent[] { new GUIContent( "False" ), new GUIContent( "True" ) }, EditorGUIUtility.isProSkin ? ( option == 0 ? pidiSkin2.customStyles[5] : pidiSkin2.customStyles[6] ) : EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector ).button );
                }
                else {
                    option = EditorGUILayout.Popup( label, option, new GUIContent[] { new GUIContent( "Disabled" ), new GUIContent( "Enabled" ) }, EditorGUIUtility.isProSkin ? ( option == 0 ? pidiSkin2.customStyles[5] : pidiSkin2.customStyles[6] ) : EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector ).button );
                }
            }
            else {
                if ( trueFalseToggle ) {
                    option = EditorGUILayout.Popup( option, new string[] { "False", "True" }, EditorGUIUtility.isProSkin ? ( option == 0 ? pidiSkin2.customStyles[5] : pidiSkin2.customStyles[6] ) : EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector ).button, options );
                }
                else {
                    option = EditorGUILayout.Popup( option, new string[] { "Disabled", "Enabled" }, EditorGUIUtility.isProSkin ? ( option == 0 ? pidiSkin2.customStyles[5] : pidiSkin2.customStyles[6] ) : EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector ).button, options );
                }
            }

            return option == 1;

        }




        /// <summary>
        /// Draw an enum field but changing the labels and names of the enum to Upper Case fields
        /// </summary>
        /// <param name="label"></param>
        /// <param name="userEnum"></param>
        /// <returns></returns>
        public int StandardEnumField( GUIContent label, System.Enum userEnum ) {

            var names = System.Enum.GetNames( userEnum.GetType() );

            for ( int i = 0; i < names.Length; i++ ) {
                names[i] = names[i].ToUpper();
            }

            GUILayout.Space( 2 );
            GUILayout.BeginHorizontal();
            GUILayout.Label( label, pidiSkin2.label, GUILayout.Width( EditorGUIUtility.labelWidth ) );
            var result = EditorGUILayout.Popup( System.Convert.ToInt32( userEnum ), names, EditorGUIUtility.isProSkin ? pidiSkin2.button : EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector ).button );
            GUILayout.EndHorizontal();
            GUILayout.Space( 2 );
            return result;
        }


        /// <summary>
        /// Draw a layer mask field in the PIDI 2020 style
        /// </summary>
        /// <param name="label"></param>
        /// <param name="selected"></param>
        public LayerMask LayerMaskField( GUIContent label, LayerMask selected ) {

            List<string> layers = null;
            string[] layerNames = null;

            if ( layers == null ) {
                layers = new List<string>();
                layerNames = new string[4];
            }
            else {
                layers.Clear();
            }

            int emptyLayers = 0;
            for ( int i = 0; i < 32; i++ ) {
                string layerName = LayerMask.LayerToName( i );

                if ( layerName != "" ) {

                    for ( ; emptyLayers > 0; emptyLayers-- ) layers.Add( "Layer " + ( i - emptyLayers ) );
                    layers.Add( layerName );
                }
                else {
                    emptyLayers++;
                }
            }

            if ( layerNames.Length != layers.Count ) {
                layerNames = new string[layers.Count];
            }
            for ( int i = 0; i < layerNames.Length; i++ ) layerNames[i] = layers[i];


            GUILayout.Space( 2 );
            GUILayout.BeginHorizontal();
            GUILayout.Label( label, pidiSkin2.label, GUILayout.Width( EditorGUIUtility.labelWidth ) );

            selected.value = EditorGUILayout.MaskField( selected.value, layerNames, EditorGUIUtility.isProSkin ? pidiSkin2.button : EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector ).button );

            GUILayout.EndHorizontal();
            GUILayout.Space( 2 );
            return selected;
        }



        #endregion

    }


}



