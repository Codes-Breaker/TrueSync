#if UNITY_EDITOR

namespace XFurStudio3.Editor {

    using XFurStudio3.Core;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;


    [CustomEditor( typeof( XFurStudioDatabase ) )]
    public class XFurStudioDatabase_Editor : Editor {

        public GUISkin pidiSkin2;
        public Texture2D xfurStudioLogo;

        XFurStudioDatabase database;

        bool[] folds;
        bool profilesFold;
        bool mainFold;

        public void OnEnable() {
            database = (XFurStudioDatabase)target;

            database.LoadResources();

        }

        public override void OnInspectorGUI() {

            //database.LoadResources();

            if ( !pidiSkin2 ) {
                GUILayout.Space( 12 );
                EditorGUILayout.HelpBox( "The needed GUISkin for this asset has not been found or is corrupted. Please re-download the asset to try to fix this issue or contact support if it persists", MessageType.Error );
                GUILayout.Space( 12 );
                return;
            }

            pidiSkin2.label = new GUIStyle( EditorStyles.label );
            var lStyle = new GUIStyle( EditorStyles.label );

            GUI.color = EditorGUIUtility.isProSkin ? new Color( 0.1f, 0.1f, 0.15f, 1 ) : new Color( 0.5f, 0.5f, 0.6f );
            GUILayout.BeginVertical();
            GUI.color = Color.white;

            AssetLogoAndVersion();


            GUILayout.Space( 16 );
            CenteredLabel( "XFur Studio™ 3 Database, " + database.Version );
            GUILayout.Space( 16 );



            GUILayout.BeginHorizontal(); GUILayout.Space( 32 ); GUILayout.BeginVertical();

            database.RenderingMode = (XFurStudioDatabase.XFurRenderingMode)EditorGUILayout.EnumPopup( new GUIContent( "Rendering System" ), database.RenderingMode, EditorGUIUtility.isProSkin ? pidiSkin2.button : EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector ).button );


           

            if ( database.RenderingMode == XFurStudioDatabase.XFurRenderingMode.Universal ) {
                EditorGUILayout.PropertyField( serializedObject.FindProperty( "receiveURPShadows" ) );
            }

            if ( serializedObject.hasModifiedProperties ) {
                serializedObject.ApplyModifiedProperties();
                database.UpdateShadows();
            }

            GUILayout.Space( 16 );


            GUILayout.Label( "XFur Shells Renderer System : " + ( database.XFShellsReady ? "Ready" : "Not Found" ), pidiSkin2.label );
            GUILayout.Label( "Basic Shells System : " + ( database.BasicReady ? "Ready" : "Not Found" ), pidiSkin2.label );
            GUILayout.Label( "URP System : " + ( database.URPReady ? "Ready" : "Not Found" ), pidiSkin2.label );
            GUILayout.Label( "HDRP System : " + ( database.HDRPReady ? "Ready" : "Not Found" ), pidiSkin2.label );

            GUILayout.Space( 16 );

            if ( !database.XFShellsReady && database.RenderingMode == XFurStudioDatabase.XFurRenderingMode.Standard ) {
                EditorGUILayout.HelpBox( "The XFShells shaders for the Built-in renderer could not be found. Importing the Legacy Pipeline folder included with this asset is necessary to locate and load these shaders and use XFur Studio with the Built-in renderer.", MessageType.Warning );
            }

            if ( !database.BasicReady && database.RenderingMode == XFurStudioDatabase.XFurRenderingMode.Standard ) {
                EditorGUILayout.HelpBox( "The Basic Shells shaders for the Built-in renderer could not be found. Importing the Legacy Pipeline folder included with this asset is necessary to locate and load these shaders and use XFur Studio with the Built-in renderer.", MessageType.Warning );
            }

            if ( !database.URPReady && database.RenderingMode == XFurStudioDatabase.XFurRenderingMode.Universal ) {
                EditorGUILayout.HelpBox( "The URP specific shaders could not be found. Unpacking the Universal RP unity package included with this asset is necessary to locate and load these shaders and use XFur Studio with Universal RP.", MessageType.Warning );
            }
            else if ( database.RenderingMode == XFurStudioDatabase.XFurRenderingMode.Universal ) {
                EditorGUILayout.HelpBox( "Please remember that, while XFur Studio™ 2 may work with newer Universal RP releases, official support for Universal RP is limited to Universal RP 7.x in Unity 2019.4", MessageType.Info );
            }

            if ( !database.HDRPReady && database.RenderingMode == XFurStudioDatabase.XFurRenderingMode.HighDefinition ) {
                EditorGUILayout.HelpBox( "The HDRP specific shaders could not be found. Unpacking the High Definition RP unity package included with this asset is necessary, in order to locate and load these shaders and use XFur Studio with the High Definition RP.", MessageType.Warning );
            }
            else if ( database.RenderingMode == XFurStudioDatabase.XFurRenderingMode.HighDefinition ) {
                EditorGUILayout.HelpBox( "Please remember that, while XFur Studio™ 2 may work with newer High Definition RP releases, official support for High Definition RP is limited to HDRP 7.x in Unity 2019.4", MessageType.Info );

            }

            GUILayout.Space( 8 );

            database.UseNormalmaps = EditorGUILayout.Toggle( "Use Normalmaps", database.UseNormalmaps );

            database.UpdateNormalmapUse();

            GUILayout.Space( 8 );

            GUILayout.EndVertical(); GUILayout.Space( 32 ); GUILayout.EndHorizontal();

            GUILayout.Space( 16 );


            EditorUtility.SetDirty( database );

            GUILayout.Space( 16 );

            GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();

            lStyle = new GUIStyle( EditorStyles.label );
            lStyle.fontStyle = FontStyle.Italic;
            lStyle.fontSize = 8;

            GUILayout.Label( "Copyright© 2017-2023,   Jorge Pinal N.", lStyle );

            GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();

            GUILayout.Space( 24 );
            GUILayout.EndVertical();


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
            GUILayout.Label( database.Version, pidiSkin2.customStyles[2] );
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



    class XFurPostProcessor : AssetPostprocessor {
        static void OnPostprocessAllAssets( string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths ) {

            foreach ( string str in importedAssets ) {
                if ( AssetDatabase.GetMainAssetTypeAtPath( str ) == typeof( XFurStudioDatabase ) ) {
                    AssetDatabase.LoadAssetAtPath<XFurStudioDatabase>( str ).LoadResources();
                }
            }
        }
    }


}

#endif