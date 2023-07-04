#if UNITY_EDITOR

namespace XFurStudio3.Editor {

    using XFurStudio3.Core;
    using XFurStudio3.Modules;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using XFurStudio3.Designer;

    [CanEditMultipleObjects]
    [CustomEditor( typeof( XFurStudioInstance ) )]
    public class XFurStudioInstance_Editor : Editor {


        public GUISkin pidiSkin2;
        public Texture2D xfurStudioLogo;

        private XFurStudioInstance xfur;


        XFurStudioInstance.XFurRenderingMode renderingMode;
        XFurStudioDatabase databaseAsset;

        XF3_Random randomModule;
        XF3_LOD lodModule;
        XF3_Physics physicsModule;
        XF3_VFX vfxModule;
        XF3_Decals decalsModule;


        //Inspector Data

        [SerializeField] protected int inspectorMode = -1;
        [SerializeField] protected int editFurProfile = 0;
        [SerializeField] protected bool inEditMode;

        [SerializeField] protected XFurStudio3Designer designer;

        XFurStudioStrandsAsset defaultStrands;
        XFurStudioDatabase defaultDatabase;

        private void OnEnable() {

            xfur = (XFurStudioInstance)target;

            randomModule = xfur.RandomizationModule;
            lodModule = xfur.LODModule;
            physicsModule = xfur.PhysicsModule;
            vfxModule = xfur.VFXModule;
            decalsModule = xfur.DecalsModule;

            if ( Application.isPlaying ) {
                return;
            }

            xfur.InitialSetup();

            var results = AssetDatabase.FindAssets( "t:XFurStudioStrandsAsset" );
            
            if ( results.Length > 0 ) {
                defaultStrands = (XFurStudioStrandsAsset)AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( results[0] ), typeof( XFurStudioStrandsAsset ) );
            }


            results = AssetDatabase.FindAssets( "t:XFurStudioDatabase" );
            
            if ( results.Length > 0 ) {
                defaultDatabase = (XFurStudioDatabase)AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( results[0] ), typeof( XFurStudioDatabase ) );
            }


            if ( !xfur.FurDatabase && defaultDatabase ) {
                xfur.FurDatabase = defaultDatabase;
            }



            //if ( !Application.isPlaying )
            //EditorApplication.update += xfur.RenderFur;

        }

        public void OnDisable() {
            //EditorApplication.update -= xfur.RenderFur;

            decalsModule.Unload();

        }



        void MainSettings() {

            GUILayout.Space( 12 );

            Undo.RecordObject( xfur, "XFurInstance_" + GetInstanceID() );

            xfur.FurDatabase = (XFurStudioDatabase)EditorGUILayout.ObjectField( new GUIContent( "XFur Database Asset", "The Xfur Studio Database Asset containing the references to all the shaders, models and templates required by this project" ), xfur.FurDatabase, typeof( XFurStudioDatabase ), false );


            var FurRenderCopy = xfur.MainRenderer;

            if ( !FurRenderCopy.renderer ) {
                if ( xfur.GetComponent<Renderer>() ) {
                    FurRenderCopy.AssignRenderer( xfur.GetComponent<Renderer>() );
                }
                else {
                    var rends = new List<Renderer>();
                    foreach ( Transform t in xfur.transform ) {
                        if ( t.GetComponent<Renderer>() ) {
                            rends.Add( t.GetComponent<Renderer>() );
                        }
                    }

                    if ( rends.Count == 1 ) {
                        FurRenderCopy.AssignRenderer( rends[0] );
                    }
                }
            }



            var tempRender = FurRenderCopy.renderer;

            GUILayout.Space( 8 );
            tempRender = (Renderer)EditorGUILayout.ObjectField( new GUIContent( "Main Renderer", "The main renderer component that displays the mesh for this XFur instance or the highest LOD (LOD0) in mesh with multiple levels of detail" ), tempRender, typeof( Renderer ), true );

            if ( tempRender != xfur.MainRenderer.renderer ) {
                if ( xfur.MainRenderer.renderer == null || EditorUtility.DisplayDialog( "WARNING", "Changing the main renderer of this XFur Studio Instance may destroy some settings and profiles assigned to it if the new renderer does not have the same amount of materials and a similar configuration. Do you want to continue?", "Continue", "Cancel" ) ) {
                    FurRenderCopy.AssignRenderer( tempRender );
                }
            }


            xfur.MainRenderer = FurRenderCopy;

            if ( xfur.FurDatabase != null && xfur.MainRenderer.renderer != null ) {

                GUILayout.Space( 16 );


                if ( xfur.FurDatabase.RenderingMode == XFurStudioDatabase.XFurRenderingMode.Standard ) {
                    xfur.RenderingMode = (XFurStudioInstance.XFurRenderingMode)EditorGUILayout.EnumPopup( new GUIContent( "Rendering Mode", "The rendering mode to be used for this XFur Instance, either XShells or Basuc Shells.\n\nXShells : Allows for extremely high sample counts, full shadow control, per-vertex animation driven physics and much more. They work on all platforms and most devices, including mid to high-end mobile phones. It uses CPU skinning so it may be slow when used in many instances or with very high polygon counts.\n\nBasic Shells have some limited features and only allow either 4 or 8 samples. However, they work mostly on the GPU allowing many instances to be rendered with a smaller performance impact. They do not support animation driven physics, shadowing is limited in Forward Mode, among other restrictions" ), xfur.RenderingMode, EditorGUIUtility.isProSkin ? pidiSkin2.button : EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector ).button );
                }
                else {
                    xfur.RenderingMode = XFurStudioInstance.XFurRenderingMode.XFShells;
                    xfur.FullForwardMode = false;
                }

                if ( xfur.FurDatabase.RenderingMode == XFurStudioDatabase.XFurRenderingMode.Standard && xfur.RenderingMode == XFurStudioInstance.XFurRenderingMode.XFShells ) {

                    xfur.FullForwardMode = EnableDisableToggle( new GUIContent( "Forward Add Mode", "Enables the Forward Add setup (additional pixel lights, point lights, etc ) to the XFShells method when using Forward Rendering, but adds some performance impact" ), xfur.FullForwardMode );

                }
                else if ( xfur.FurDatabase.RenderingMode != XFurStudioDatabase.XFurRenderingMode.Standard ) {
                    xfur.FullForwardMode = EnableDisableToggle( new GUIContent( "Compatibility Mode", "Enables the High compatibility mode (necessary for some older devices) to the XFShells method by disabling some of the advanced graphical optimizations, but adds some performance impact" ), xfur.FullForwardMode );
                }

                GUILayout.Space( 8 );

                xfur.AutoUpdateMaterials = EnableDisableToggle( new GUIContent( "Auto-Update Materials", "Automatically update the fur materials every certain time, allowing runtime changes to length, thickness, textures etc. to be instantly applied" ), xfur.AutoUpdateMaterials, true );

                if ( xfur.AutoUpdateMaterials ) {
                    xfur.AutoAdjustForScale = EnableDisableToggle( new GUIContent( "Auto-Adjust for Scale", "Automatically adjusts all fur parameters to be scale-relative" ), xfur.AutoAdjustForScale, true );
                    GUILayout.Space( 4 );                    
                    xfur.UpdateFrequency = EditorGUILayout.FloatField( new GUIContent( "Update Frequency (s)", "The update frequency of the fur material properties (in seconds). You can disable auto-updates entirely if you do not plan to modify the fur properties at runtime" ), xfur.UpdateFrequency );
                }

                GUILayout.Space( 8 );

                xfur.UseLossyScale = EnableDisableToggle( new GUIContent( "Force Lossy Scale", "Applies internal adjustments to the scale of the mesh that may solve some issues with scaling present in certain meshes" ), xfur.UseLossyScale, true );

                GUILayout.Space( 16 );

                GUILayout.BeginHorizontal( EditorStyles.helpBox ); GUILayout.Space( 16 );
                GUILayout.BeginVertical();

                GUILayout.Space( 16 );

                CenteredLabel( "Fur Based Materials" );

                GUILayout.Space( 16 );

                if ( xfur.MainRenderer.isFurMaterial.Length == xfur.FurDataProfiles.Length ) {

                    for ( int i = 0; i < xfur.MainRenderer.isFurMaterial.Length; ++i ) {

                        if ( !xfur.MainRenderer.materials[i] ) {
                            var rend = xfur.MainRenderer;
                            rend.materials[i] = rend.renderer.sharedMaterials[i];
                            xfur.MainRenderer = rend;
                        }

                        xfur.MainRenderer.isFurMaterial[i] = EnableDisableToggle( new GUIContent( xfur.MainRenderer.materials[i].name, "Set whether this fur material will render fur or not." ), xfur.MainRenderer.isFurMaterial[i] );

                        if ( !xfur.FurDataProfiles[i].FurStrandsAsset ) {
                            xfur.FurDataProfiles[i].FurStrandsAsset = defaultStrands;
                        }
                    }

                }
                GUILayout.Space( 16 );
                GUILayout.EndVertical();
                GUILayout.Space( 16 ); GUILayout.EndHorizontal();
            }

        }



        void ModuleSettings() {

            Undo.RecordObject( xfur, "XFurInstance_" + GetInstanceID() );

            GUILayout.Space( 12 );

            GUILayout.BeginHorizontal( EditorStyles.helpBox ); GUILayout.Space( 16 );
            GUILayout.BeginVertical();
            GUILayout.Space( 16 );
            CenteredLabel( "Built-In Modules" );
            GUILayout.Space( 16 );


            GUILayout.BeginHorizontal();
            GUILayout.Label( randomModule.moduleName + ", v" + randomModule.version, GUILayout.Width( 140 ) );
            GUILayout.Space( 64 );
            GUILayout.Label( " Status : " + randomModule.ModuleStatus );
            GUILayout.FlexibleSpace();

            var prop = serializedObject.FindProperty( "randomModule.isEnabled" );
            prop.boolValue = EnableDisableToggle( null, prop.boolValue, false, GUILayout.MaxWidth( EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth - 200 ) ) && !randomModule.criticalError;
            GUILayout.EndHorizontal();

            GUILayout.Space( 4 );

            GUILayout.BeginHorizontal();
            GUILayout.Label( lodModule.moduleName + ", v" + lodModule.version, GUILayout.Width( 140 ) );
            GUILayout.Space( 64 );
            GUILayout.Label( " Status : " + lodModule.ModuleStatus );
            GUILayout.FlexibleSpace();

            prop = serializedObject.FindProperty( "lodModule.isEnabled" );
            prop.boolValue = EnableDisableToggle( null, prop.boolValue, false, GUILayout.MaxWidth( EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth - 200 ) ) && !lodModule.criticalError;
            GUILayout.EndHorizontal();

            GUILayout.Space( 4 );

            GUILayout.BeginHorizontal();
            GUILayout.Label( physicsModule.moduleName + ", v" + physicsModule.version, GUILayout.Width( 140 ) );
            GUILayout.Space( 64 );
            GUILayout.Label( " Status : " + physicsModule.ModuleStatus );
            GUILayout.FlexibleSpace();

            prop = serializedObject.FindProperty( "physicsModule.isEnabled" );
            prop.boolValue = EnableDisableToggle( null, prop.boolValue, false, GUILayout.MaxWidth( EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth - 200 ) ) && !physicsModule.criticalError;
            GUILayout.EndHorizontal();

            GUILayout.Space( 4 );

            GUILayout.BeginHorizontal();
            GUILayout.Label( vfxModule.moduleName + ", v" + vfxModule.version, GUILayout.Width( 140 ) );
            GUILayout.Space( 64 );
            GUILayout.Label( " Status : " + vfxModule.ModuleStatus );
            GUILayout.FlexibleSpace();

            prop = serializedObject.FindProperty( "vfxModule.isEnabled" );
            prop.boolValue = EnableDisableToggle( null, prop.boolValue, false, GUILayout.MaxWidth( EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth - 200 ) ) && !vfxModule.criticalError;
            GUILayout.EndHorizontal();


            GUILayout.Space( 4 );


            GUILayout.BeginHorizontal();
            GUILayout.Label( decalsModule.moduleName + ", v" + decalsModule.version, GUILayout.Width( 140 ) );
            GUILayout.Space( 64 );
            GUILayout.Label( " Status : " + decalsModule.ModuleStatus );
            GUILayout.FlexibleSpace();

            prop = serializedObject.FindProperty( "decalsModule.isEnabled" );
            prop.boolValue = EnableDisableToggle( null, prop.boolValue, false, GUILayout.MaxWidth( EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth - 200 ) ) && !decalsModule.criticalError;
            GUILayout.EndHorizontal();


            GUILayout.Space( 16 );

            GUILayout.EndVertical();
            GUILayout.Space( 16 ); GUILayout.EndHorizontal();


            GUILayout.Space( 16 );

            if ( randomModule.enabled ) {
                if ( BeginCenteredGroup( "Randomization", ref xfur.folds[2] ) ) {
                    randomModule.ModuleUI();
                }
                EndCenteredGroup();
            }

            if ( lodModule.enabled ) {
                if ( BeginCenteredGroup( "Dynamic LOD", ref xfur.folds[3] ) ) {
                    lodModule.ModuleUI();
                }
                EndCenteredGroup();
            }

            if ( physicsModule.enabled ) {
                if ( BeginCenteredGroup( "Physics", ref xfur.folds[4] ) ) {
                    physicsModule.ModuleUI();
                }
                EndCenteredGroup();
            }

            if ( vfxModule.enabled ) {
                if ( BeginCenteredGroup( "VFX & Weather", ref xfur.folds[5] ) ) {
                    vfxModule.ModuleUI();
                }
                EndCenteredGroup();
            }

            if ( decalsModule.enabled ) {
                if ( BeginCenteredGroup( "UV Based Decals", ref xfur.folds[6] ) ) {
                    decalsModule.ModuleUI();
                }
                EndCenteredGroup();
            }


        }



        void XFurDesigner() {

            GUILayout.Space( 12 );

            string[] profNames = new string[xfur.FurDataProfiles.Length];

            for (int i = 0; i < profNames.Length; i++ ) {
                if ( !xfur.MainRenderer.materials[i] ) {
                    var rend = xfur.MainRenderer;
                    rend.materials[i] = rend.renderer.sharedMaterials[i];
                    xfur.MainRenderer = rend;
                }
                profNames[i] = xfur.MainRenderer.materials[i].name;
            }



            GUILayout.Space( 16 );

            if ( xfur.MainRenderer.furProfiles != null && xfur.MainRenderer.furProfiles.Length > 0 ) {

                xfur.editFurProfile = EditorGUILayout.Popup( "Active Fur Material", xfur.editFurProfile, profNames );

                xfur.editFurProfile = Mathf.Clamp( xfur.editFurProfile, 0, xfur.FurDataProfiles.Length-1 );

                GUILayout.Space( 8 );

                if ( xfur.MainRenderer.isFurMaterial[xfur.editFurProfile] ) {
                    if ( CenteredButton( inEditMode ? "Save & Exit" : "Enter Edit Mode", 256 ) ) {
                        inEditMode = !inEditMode;

                        if ( inEditMode ) {
                            designer = CreateInstance<XFurStudio3Designer>();
                            designer.xfur = xfur;
                            designer.editFurProfile = xfur.editFurProfile;
                            designer.titleContent = new GUIContent( "XFur Studio Designer" );
                            designer.Show();
                        }
                        else {
                            if ( designer ) {
                                designer.Close();
                                designer = null;
                            }
                        }

                    }
                }
            }
            else {
                EditorGUILayout.HelpBox( "There are no fur enabled materials on this XFur Studio Instance", MessageType.Warning );
            }

        }



        void XFurInspector3() {

            GUILayout.BeginHorizontal(); GUILayout.Space( 20 );
            GUILayout.BeginVertical();

            AssetLogoAndVersion();

            var lStyle = new GUIStyle();

            GUILayout.Space( 8 );

            if ( serializedObject.isEditingMultipleObjects ) {

                HelpBox( "XFur Studio depends on per-instance behavior and data sets. Editing multiple instances is not allowed. If you need to share properties across multiple instances, use XFur Templates or Unity Prefabs instead", MessageType.Warning );

                GUILayout.Space( 24 );

                GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();

                lStyle = new GUIStyle( EditorStyles.label );
                lStyle.fontStyle = FontStyle.Italic;
                lStyle.fontSize = 8;

                GUILayout.Label( "Copyright© 2017-2023,   Jorge Pinal N.", lStyle );

                GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();

                GUILayout.Space( 24 );
                GUILayout.EndVertical();
                GUILayout.Space( 20 );
                GUILayout.EndHorizontal();
                return;
            }

            GUILayout.BeginHorizontal();


            if ( GUILayout.Button( "Settings", inspectorMode == 0 ? pidiSkin2.customStyles[6] : pidiSkin2.customStyles[5] ) ) {
                inspectorMode = inspectorMode == 0 ? -1 : 0;
            }

            if ( GUILayout.Button( "Modules", inspectorMode == 1 ? pidiSkin2.customStyles[6] : pidiSkin2.customStyles[5] ) ) {
                inspectorMode = inspectorMode == 1 ? -1 : 1;
            }

            if ( GUILayout.Button( "Fur Designer", inspectorMode == 2 ? pidiSkin2.customStyles[6] : pidiSkin2.customStyles[5] ) ) {
                inspectorMode = inspectorMode == 2 ? -1 : 2;
            }

            GUILayout.EndHorizontal();



            switch ( inspectorMode ) {

                case 0:
                    MainSettings();
                    break;

                case 1:
                    ModuleSettings();
                    break;

                case 2:
                    XFurDesigner();
                    break;

            }

            lStyle = new GUIStyle( EditorStyles.label );
            lStyle.fontStyle = FontStyle.Italic;
            lStyle.fontSize = 8;

            GUILayout.Space( 24 );

            GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();

            GUILayout.Label( "Copyright© 2017-2023,   Jorge Pinal N.", lStyle );

            GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();

            GUILayout.Space( 24 );
            GUILayout.EndVertical();
            GUILayout.Space( 20 ); GUILayout.EndHorizontal();

        }



        void Separator( string label ) {


            GUILayout.BeginHorizontal( pidiSkin2.box );
            GUILayout.FlexibleSpace();

            GUILayout.Label( label );

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


        }



        public override void OnInspectorGUI() {

            //SceneView.RepaintAll();


            //Repaint();
            

            XFurInspector3();
          
            if ( serializedObject.hasModifiedProperties ) {
                serializedObject.ApplyModifiedProperties();
            }

        }








        #region PIDI 2020 EDITOR

        public void XFurModuleStatus( XFurStudioModule module ) {
            GUILayout.BeginHorizontal();
            GUILayout.Label( module.moduleName + ", v" + module.version, pidiSkin2.label, GUILayout.Width( 140 ) );
            GUILayout.Space( 64 );
            GUILayout.Label( " Status : " + module.ModuleStatus, pidiSkin2.label );
            GUILayout.FlexibleSpace();
            var t = EnableDisableToggle( null, module.enabled, false, GUILayout.MaxWidth( EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth - 200 ) ) && !module.criticalError;
            if ( t ) {
                module.Enable();
            }
            else {
                module.Disable();
            }
            GUILayout.EndHorizontal();
        }


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
        public void CenteredLabel( string label ) {

            GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();
            GUILayout.Label( label, EditorStyles.boldLabel );
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
            currentValue = EditorGUILayout.IntField( currentValue, EditorGUIUtility.isProSkin ? pidiSkin2.customStyles[4] : EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector ).textField );
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
            currentValue = EditorGUILayout.TextField( currentValue, EditorGUIUtility.isProSkin ? pidiSkin2.customStyles[4] : EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector ).textField );
            GUILayout.EndHorizontal();
            GUILayout.Space( 2 );

            return currentValue;
        }


        public Vector2 Vector2Field( GUIContent label, Vector2 currentValue ) {

            GUILayout.Space( 2 );
            GUILayout.BeginHorizontal();
            GUILayout.Label( label, pidiSkin2.label, GUILayout.Width( EditorGUIUtility.labelWidth ) );
            currentValue.x = EditorGUILayout.FloatField( currentValue.x, EditorGUIUtility.isProSkin ? pidiSkin2.customStyles[4] : EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector ).textField );
            GUILayout.Space( 8 );
            currentValue.y = EditorGUILayout.FloatField( currentValue.y, EditorGUIUtility.isProSkin ? pidiSkin2.customStyles[4] : EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector ).textField );
            GUILayout.EndHorizontal();
            GUILayout.Space( 2 );

            return currentValue;

        }

        public Vector3 Vector3Field( GUIContent label, Vector3 currentValue ) {

            GUILayout.Space( 2 );
            GUILayout.BeginHorizontal();
            GUILayout.Label( label, pidiSkin2.label, GUILayout.Width( EditorGUIUtility.labelWidth ) );
            currentValue.x = EditorGUILayout.FloatField( currentValue.x, EditorGUIUtility.isProSkin ? pidiSkin2.customStyles[4] : EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector ).textField );
            GUILayout.Space( 8 );
            currentValue.y = EditorGUILayout.FloatField( currentValue.y, EditorGUIUtility.isProSkin ? pidiSkin2.customStyles[4] : EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector ).textField );
            GUILayout.Space( 8 );
            currentValue.z = EditorGUILayout.FloatField( currentValue.z, EditorGUIUtility.isProSkin ? pidiSkin2.customStyles[4] : EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector ).textField );
            GUILayout.EndHorizontal();
            GUILayout.Space( 2 );

            return currentValue;

        }


        public Vector4 Vector4Field( GUIContent label, Vector4 currentValue ) {

            GUILayout.Space( 2 );
            GUILayout.BeginHorizontal();
            GUILayout.Label( label, pidiSkin2.label, GUILayout.Width( EditorGUIUtility.labelWidth ) );
            currentValue.x = EditorGUILayout.FloatField( currentValue.x, EditorGUIUtility.isProSkin ? pidiSkin2.customStyles[4] : EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector ).textField );
            GUILayout.Space( 8 );
            currentValue.y = EditorGUILayout.FloatField( currentValue.y, EditorGUIUtility.isProSkin ? pidiSkin2.customStyles[4] : EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector ).textField );
            GUILayout.Space( 8 );
            currentValue.z = EditorGUILayout.FloatField( currentValue.z, EditorGUIUtility.isProSkin ? pidiSkin2.customStyles[4] : EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector ).textField );
            GUILayout.Space( 8 );
            currentValue.w = EditorGUILayout.FloatField( currentValue.w, EditorGUIUtility.isProSkin ? pidiSkin2.customStyles[4] : EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector ).textField );
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
                    option = EditorGUILayout.Popup( option, new string[] { "False", "True" }, EditorGUIUtility.isProSkin ? ( option==0?pidiSkin2.customStyles[5]: pidiSkin2.customStyles[6] ) : EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector ).button, options );
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

#endif