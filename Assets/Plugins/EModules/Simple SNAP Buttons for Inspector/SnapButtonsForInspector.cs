#if UNITY_EDITOR

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace EModules.Snap
{
  ////////
  //////// ICONS ////////
  ////////
  abstract class IconBehaviour
  {
    protected abstract string _mData { get; }

    public static implicit operator Texture2D(IconBehaviour ib)
    {
      var result = new Texture2D( 1 , 1 , TextureFormat.ARGB32 , false , true );
      ImageConversion.LoadImage( result, Convert.FromBase64String( ib._mData ) );
      result.filterMode = FilterMode.Point;
      result.wrapMode = TextureWrapMode.Clamp;
      result.alphaIsTransparency = true;
      result.Apply();

      return result;
    }
  }

  class Icon_SnapButtonsForInspectorSurface : IconBehaviour
  {
    protected override string _mData { get { return "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAADAUlEQVQ4jT3QOW8cZQDG8f87OzO7nrV3djc+dr1JCEEQ2UvACRilMVIKOuhAogyIgh4kPgfwGZAQDVIKGkSDAFN5E0VObPARC4+vtbPX3PMeFEZUT/dIv79YXV2dq7eKr64tR/c6N5KsVrXNbMPlasdm2i8RjhRHgeRskDOMpAj2qs7h5tSPUWh9s/7rhrJX3o0+++jz8As1ehPHLVNxffypGVy3jOM4tGYM882QMAvJiwmDlQvSD/bWjJgcr6/xvdVXY1s2ary1fI/5ueu43jT9NMG260SZ4NlZwFBKyl6NVzq3uX/nPUSzzp45bwHYJy90chB43L1R5u/jQ1KZMYoHtKbrjNMJ2yeb1CpNGlWfWa/OVrDPo50JgUEB2EIgLHLS/IK/znYBTVqk5CpGqYxhkhDlfcJ8QHdxhj/2f+Nc7yD3Fg0cYSsFSR5ilzKWFhbRgFQSu6TwXM1yaw4scEsaYwbcbnc4tbd59l3VBrALYxHJiJLIea29AFhgBCUro1Qq6HauINAIIQDDrfYsfw59ikrmAthGgy0swnzIDxsbGKMI85gPV15nHKf8tPWUiu3S9Dze797i4eNtno5HlJ1ZDWBLZZHLCK1DUpUg0OQ6By1RRqGMRhuF1gZjFEobUgVa41wSMksU5CgdM0gjAAZJQqEluSo4jWKmXQ+rlKEoGGYJoxyM4bIBfpENownTZcOnby9hBAhj4TkufsXmy7V3EEIghEYIw8d3Xubk0T6/JMUl4flyoPS4RipTnhwPcLCQBlYWG2RKst0PEUDZNnRbPpunEw7H4ApxGTHLEFrEJKWQXrBPlBj+Gcd0mm8QvIj5dn2Hm77L1XqVV9sWfdnnLAP+J2RUH48kT6IeN1+CRMJCDHn1gikK7i9BrZpTmcrZkgOa8w7xLpynWfXy4JCt3yfQT+CaD+06NKvQCwIaHnTa8HwIgz78vAcHBwW7J3AttY8ARPdul0zkDw68808KbySpaIMHNIApIAaG/21qCSf27evJlYc1NfN1r9eT/wKb0ZEkj3mB4AAAAABJRU5ErkJggg=="; } }
  }

  class Icon_SnapButtonsForInspectorNormal : IconBehaviour
  {
    protected override string _mData { get { return "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAABkklEQVQ4jY2TMWtUQRSFvzu77rqJMRixECQEtBEs7dQu/0AbOxO0SJEqdQoFIVpaGBAtgoVgZ2PjD5CtDSimiGACEjQhu7Nx33sz71js8nDd9xIvnGbmnHPv3HsHSZQB/A3wAr8GHvClPBuQx8Os9xm4AgiYBX5Ik2M8VyFeAC4BXSAFXpdmqTIAbgMvgAA8AZIqg6r3A/4i+D3wc8f1oKqC/45Sg2GzNHo2UWUgRlFo7W/il82fC29ebY6NzDbWt2V1yHNwaqFah4ij02nx6PFZnj7sUmtmhaBhLe4uXjAb1u6O0t00yiFz9BqRM2lO+8F94soSN/efEZbv8W15hcwSpvunyBTYeLmv9sdfqwCWSdSTnDun53WNwBQ1mvymSYspGkyQMklOnwaRlPblW8yureL2djk/c7jIwdZX+wD6BHrLnL5zbrCgcCySMBy5onhnvN8BxRNEYxgohGKCQOFfgpm0/hylPRRELo+uXm8X9/3u0CBE1DsgK8T1o34WSjevQJIjcxr9jTUnYj4y/5PiD6S6SmBNvkhIAAAAAElFTkSuQmCC"; } }
  }

  class Icon_SnapButtonsForInspectorImage : IconBehaviour
  {
    protected override string _mData { get { return "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAACiklEQVQ4ja2TS2sUaRiFn+9S9+5UuqNtvMGo0Y0XxJFB8B8YHf0ByjhbQUaXrgXXIoiCq+wUXGhQXHhhRMHViMYrjjEq6hiT7rJNp7qrvqr+XDRkfoAeeLfPew6cAz8oAbDw9esR4thTr1966tAWV+elJ6vCQ1mPHJfNvzbb2/bcSW7eudU5dJTKf2+xs59Yf/IMGkC57lkHsK6D45UDrGcHLzygaBE/uXwijsz16fbs0VRVZiJjAJAAfhCgASeMoM/gBOAABki+QTgCq+vjGx6cmwq+TI+1128FGDh4NjWVs6xRVt+/MUHiF/3FniHVBYK45hQVTzWh9QVqK8ATlXUfbt945Nc2LgHuX5tc1wmjcmXWy0cXaqafZsbJXGN7ac0ZHto7Vs6dH3VnfISE5avAvBtbe+XMfuDqIIIQnzR2VtsycZK5jttOssB2+1WnaNaXDU987K453Jk2sDAPzXmoNBip2fElB9V6HRGERFlItHMn1pSoZA5fQBRHyEJeStvBRKU171G44AUg0pVLgI07tmPdACUEcvduRBQSnz5FOjlJvusAYq6j9a1/JSlgm6A16LiE7gAgLVhAOg5m9SrsUAXZaJC9nUfcvYfbt8dDkTmgsL0S8e493U2/vYHPA4Cx7MvLslStVh49/sco3ze91zN54YuqfvHq91rYPeaPKIpcomUJOSQfOxNLEYYbo5NZfQT/6RPcP/9A5BbRcAiXhwQiJwgVvVwidR8WC9qjv1xcGNv+mL+fDwDadXGrQwRRiI40EoNQGuUIrNDkBrS0qMWcThQ9bO4aP6ji+v9F6qVd7GIHm+WE2kfoEmk1RdciNUjZx0AvUfGFbn3NX8RVK135ozv8SfoOUdQDgPzMkmcAAAAASUVORK5CYII="; } }
  }




  ////////
  //////// SETTINGS WINDOW ////////
  ////////
  class SnapWindow : EditorWindow
  {
    static EditorWindow w;

    public static void Open()
    {
      if (w) w.Close();
      w = GetWindow( typeof( SnapWindow ), false, "Snap Settings...", true );
      w.Show();
    }
    public void OnGUI()
    {
      if (!w) w = this;

      DrawToogle( Snap.SNAP_AUTOAPPLY );
      GUILayout.Space( 10 );
      GUILayout.Label( "Grid Snapping" );
      DrawVector( Snap.SNAP_POS );
      GUILayout.Space( 5 );
      DrawVector( Snap.SNAP_ROT );
      GUILayout.Space( 5 );
      DrawVector( Snap.SNAP_SCALE );
      GUILayout.Space( 10 );
      GUILayout.Label( "Surface Placement" );
      DrawToogle( Snap.PLACE_ON_SURFACE_ENABLE );
      var e = GUI.enabled;
      GUI.enabled = false;
      var i = EditorGUILayout.Popup( Snap.PLACE_ON_SURFACE_ALIGNBYMOUSE ? 0 : 1 , Snap.ALIGN_BY );
      if (i != (Snap.PLACE_ON_SURFACE_ALIGNBYMOUSE ? 0 : 1)) Snap.PLACE_ON_SURFACE_ALIGNBYMOUSE.Set( i == 1 );
      GUI.enabled = e;
      DrawToogle( Snap.PLACE_ON_SURFACE_BOUNDS );
      GUILayout.Space( 10 );
      DrawToogle( Snap.ALIGN_BY_NORMAL );
      i = EditorGUILayout.Popup( Snap.ALIGN_UP_VECTOR.name, Snap.ALIGN_UP_VECTOR, Snap.VECTORS_STRING );
      if (i != Snap.ALIGN_UP_VECTOR) Snap.ALIGN_UP_VECTOR.Set( i );

      if (GUI.changed) UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
    }

    void DrawVector(VectorPref pref)
    {
      if (GUILayout.Button( pref.ENABLE.name )) pref.ENABLE.Set( !pref.ENABLE );
      if (Event.current.type == EventType.Repaint && pref.ENABLE) GUI.skin.button.Draw( GUILayoutUtility.GetLastRect(), pref.ENABLE.name, true, true, false, true );

      GUILayout.BeginHorizontal();
      if (GUILayout.Button( "X", EditorStyles.miniButtonLeft, new GUILayoutOption[0] )) pref.X.Set( !pref.X );
      if (Event.current.type == EventType.Repaint && pref.X) GUI.skin.button.Draw( GUILayoutUtility.GetLastRect(), pref.X.name, true, true, false, true );
      if (GUILayout.Button( "Y", EditorStyles.miniButtonMid, new GUILayoutOption[0] )) pref.Y.Set( !pref.Y );
      if (Event.current.type == EventType.Repaint && pref.Y) GUI.skin.button.Draw( GUILayoutUtility.GetLastRect(), pref.Y.name, true, true, false, true );
      if (GUILayout.Button( "Z", EditorStyles.miniButtonRight, new GUILayoutOption[0] )) pref.Z.Set( !pref.Z );
      if (Event.current.type == EventType.Repaint && pref.Z) GUI.skin.button.Draw( GUILayoutUtility.GetLastRect(), pref.Z.name, true, true, false, true );
      GUILayout.EndHorizontal();
    }

    void DrawToogle(CachedPref pref)
    {
      var b = EditorGUILayout.ToggleLeft( pref.name , pref );
      if (b != pref) pref.Set( b );
    }

  }






  [InitializeOnLoad]
  class Snap
  {
    ////////
    //////// SETTINGS PARAMS ////////
    ////////

    public static VectorPref SNAP_POS = new VectorPref( "SNAP_TOOGLES/POS" , enableName: "Enable Position Snapping %#k" );
    public static VectorPref SNAP_ROT = new VectorPref( "SNAP_TOOGLES/ROT" , enableName: "Enable Rotation Snapping" );
    public static VectorPref SNAP_SCALE = new VectorPref( "SNAP_TOOGLES/SCALE" , true , enableName: "Enable Scale Snapping" );
    public static CachedPref SNAP_AUTOAPPLY = new CachedPref( "SNAP_TOOGLES/SNAP_AUTOAPPLY" , false ) { name = "Auto-Apply Snap" };

    public static CachedPref PLACE_ON_SURFACE_ENABLE = new CachedPref( "SNAP_TOOGLES/PLACE_ON_SURFACE_ENABLE" , false ) { name = "Enable Surface Placement %#l" };
    public static CachedPref PLACE_ON_SURFACE_ALIGNBYMOUSE = new CachedPref( "SNAP_TOOGLES/PLACE_ON_SURFACE_ALIGNBYMOUSE" , true ) { name = "" };
    public static CachedPref PLACE_ON_SURFACE_BOUNDS = new CachedPref( "SNAP_TOOGLES/PLACE_ON_SURFACE_BOUNDS" , true ) { name = "Calc Bounds Offset" };

    public static CachedPref ALIGN_BY_NORMAL = new CachedPref( "SNAP_TOOGLES/ALIGN_BY_NORMAL" , false ) { name = "Enable Align by Surface Normal" };
    public static CachedPref ALIGN_UP_VECTOR = new CachedPref( "SNAP_TOOGLES/ALIGN_UP_VECTOR" , 0 ) { name = "Auto-Apply Snap" };

    ////////
    //////// UPDATING ////////
    ////////

    // vars
    static Vector3 p, v;
    static bool b;
    static System.Reflection.PropertyInfo dragActive;
    // strings
    static string[] POS_PREFKEYS = { "MoveSnapX" , "MoveSnapY" , "MoveSnapZ" };
    static string[] ROT_PREFKEYS = { "RotationSnap" , "RotationSnap" , "RotationSnap" };
    static string[] SCALE_PREFKEYS = { "ScaleSnap" , "ScaleSnap" , "ScaleSnap" };
    static string[] UNDO_TEXT = { "Move" , "Rotate" , "Scale" };
    public static Vector3[] VECTORS = { Vector3.up , Vector3.forward , Vector3.down , Vector3.right , Vector3.left , Vector3.back };
    public static string[] VECTORS_STRING = { "Look Up" , "Look Forward" , "Look Down" , "Look Right" , "Look Left" , "Look Back" };
    public static string[] ALIGN_BY = { "Alignment by Mouse Position" , "Alignment by Camera Projection" };

    // initialization
    static Snap()
    {
      EditorApplication.update -= EditorApplication_UPDATESNAPPING;
      EditorApplication.update += EditorApplication_UPDATESNAPPING;
      SceneView.onSceneGUIDelegate -= SceneView_PLACEONSURFACE;
      SceneView.onSceneGUIDelegate += SceneView_PLACEONSURFACE;

      dragActive = typeof( Editor ).Assembly.GetType( "UnityEditor.TransformManipulator" ).GetProperty( "active" );
      typeof( Editor ).Assembly.GetType( "UnityEditor.SnapSettings" ).GetMethod( "Initialize", (System.Reflection.BindingFlags)int.MaxValue ).Invoke( null, null );
    }

    // save object params
    static void SET_DIRTY(Transform t)
    {
      EditorUtility.SetDirty( t );
      if (!t.gameObject.scene.isDirty) UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty( t.gameObject.scene );
    }





    static void EditorApplication_UPDATESNAPPING()
    {
      was = false;

      if (!SNAP_POS.ENABLE && !SNAP_ROT.ENABLE && !SNAP_SCALE.ENABLE || !SNAP_AUTOAPPLY && !(bool)dragActive.GetValue( null, null )) return;


      foreach (var t in Selection.GetTransforms( SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable ))
      {
        PosSnappingUpdater( t );
        RotSnappingUpdater( t );
        ScaleSnappingUpdater( t );
      }
    }

    static void PosSnappingUpdater(Transform t)
    {
      if (!SNAP_POS.ENABLE) return;

      v = SNAP_POS.USE_LOCAL ? t.localPosition : t.position;
      if (DO_SNAPACTION( t, SNAP_POS, ref POS_PREFKEYS, 1 ))
      {
        if (SNAP_POS.USE_LOCAL) t.localPosition = v;
        else t.position = v;
        SET_DIRTY( t );
      }
    }
    static void RotSnappingUpdater(Transform t)
    {
      if (!SNAP_ROT.ENABLE) return;

      v = SNAP_ROT.USE_LOCAL ? t.localRotation.eulerAngles : t.rotation.eulerAngles;
      if (DO_SNAPACTION( t, SNAP_ROT, ref ROT_PREFKEYS, 1 ))
      {
        if (SNAP_ROT.USE_LOCAL) t.localRotation = Quaternion.Euler( v );
        else t.rotation = Quaternion.Euler( v );
        SET_DIRTY( t );
      }
    }
    static void ScaleSnappingUpdater(Transform t)
    {
      if (!SNAP_SCALE.ENABLE) return;

      v = t.localScale;
      if (DO_SNAPACTION( t, SNAP_SCALE, ref SCALE_PREFKEYS, 1 ))
      {
        t.localScale = v;
        SET_DIRTY( t );
      }
    }


    static bool DO_SNAPACTION(Transform t, VectorPref pref, ref string[] prefKeys, int undoTextIndex)
    {
      p = v;

      if (pref.X) v.x = (float)Math.Round( v.x / EditorPrefs.GetFloat( prefKeys[0] ), 0 ) * EditorPrefs.GetFloat( prefKeys[0] );
      if (pref.Y) v.y = (float)Math.Round( v.y / EditorPrefs.GetFloat( prefKeys[1] ), 0 ) * EditorPrefs.GetFloat( prefKeys[1] );
      if (pref.Z) v.z = (float)Math.Round( v.z / EditorPrefs.GetFloat( prefKeys[2] ), 0 ) * EditorPrefs.GetFloat( prefKeys[2] );

      if (float.IsNaN( v.x )) v.x = p.x;
      if (float.IsNaN( v.y )) v.y = p.y;
      if (float.IsNaN( v.z )) v.z = p.z;

      b = p != v;
      if (b) Undo.RecordObject( t, UNDO_TEXT[undoTextIndex] );
      return b;
    }






    static bool was = false;
    // static Vector2 mouseTunning;
    static Vector2[] mouseTunning;
    private static void SceneView_PLACEONSURFACE(SceneView sceneView)
    {
      if (!sceneView) return;

      if (Event.current.type == EventType.KeyDown && Event.current.control && Event.current.shift)
      {
        if (Event.current.keyCode == KeyCode.L) PLACE_ON_SURFACE_ENABLE.Set( !PLACE_ON_SURFACE_ENABLE );
        if (Event.current.keyCode == KeyCode.K) SNAP_POS.ENABLE.Set( !SNAP_POS.ENABLE );
        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
      }

      if (!PLACE_ON_SURFACE_ENABLE) return;

      if (Event.current.type == EventType.MouseDown && (Selection.GetTransforms( SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable ).Length > 0))
      {
        /* if ( PLACE_ON_SURFACE_ALIGNBYMOUSE )
                     mouseTunning = new[] { (HandleUtility.WorldToGUIPoint( Tools.handlePosition ) - Event.current.mousePosition) };
                 else*/
        mouseTunning = Selection.GetTransforms( SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable ).Select( t => HandleUtility.WorldToGUIPoint( t.position ) - Event.current.mousePosition ).ToArray();
      }

      if (Event.current.rawType == EventType.Repaint) was = false;


      if (!(bool)dragActive.GetValue( null, null ) && !was) return;

      was = true;
      var selection = Selection.GetTransforms( SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable );
      var excludeList = CreateExcludeList( selection );


      for (int i = 0 ; i < selection.Length ; i++)
      {
        var t = selection[i];

        PosSnappingUpdater( t );

        p = t.position;
        /* var ray = PLACE_ON_SURFACE_ALIGNBYMOUSE ?
                       HandleUtility.GUIPointToWorldRay( Event.current.mousePosition + mouseTunning[Math.Min( mouseTunning.Length - 1, i )] ) :
                       HandleUtility.GUIPointToWorldRay( HandleUtility.WorldToGUIPoint( p ) );*/
        var ray = HandleUtility.GUIPointToWorldRay( Event.current.mousePosition + mouseTunning[Math.Min( mouseTunning.Length - 1 , i )] );

        SNAP_VARIANT_2( t, ray, excludeList );

        t.position = v;
        SET_DIRTY( t );


      }

      EditorApplication_UPDATESNAPPING();

    }




    static void SNAP_VARIANT_2(Transform t, Ray ray, Dictionary<int, int> excludeList)
    {
      object obj = null;
      float mindist = float.MaxValue;
      foreach (var hit in Physics.RaycastAll( ray ))
      {
        if (hit.collider == null || excludeList.ContainsKey( hit.collider.transform.GetInstanceID() )) continue;
        if (Vector3.SqrMagnitude( hit.point - ray.origin ) > mindist && Vector3.SqrMagnitude( hit.point - ray.origin ) != float.PositiveInfinity) continue;
        mindist = Vector3.SqrMagnitude( hit.point - ray.origin );
        obj = hit;
      }

      if (obj != null)
      {
        Undo.RecordObject( t, UNDO_TEXT[0] );

        RaycastHit raycastHit = (RaycastHit)obj;
        v = raycastHit.point;

        t.position = v;


        if (PLACE_ON_SURFACE_BOUNDS && CalcBoundsForGameObjectHierarchy( t.gameObject ))
        {
          var plane = new Plane( raycastHit.normal , raycastHit.point );
          var offset = 0f;

          for (int i = 0 ; i < boundsVertices.Length ; i++)
          {

            var projection = plane.ClosestPointOnPlane( boundsVertices[i] );
            var difference = projection - boundsVertices[i];

            if (Vector2.Dot( difference.normalized, raycastHit.normal ) > 0)
            {
              if (difference.sqrMagnitude > offset)
                offset = difference.sqrMagnitude;
            }
          }

          if (offset != 0)
          {
            offset = (float)Math.Sqrt( offset );
            v += raycastHit.normal * offset;
          }
        }

        if (ALIGN_BY_NORMAL)
        {
          Undo.RecordObject( t, UNDO_TEXT[1] );
          switch (ALIGN_UP_VECTOR)
          {
            case 0: t.up = raycastHit.normal; break;
            case 1: t.forward = raycastHit.normal; break;
            case 2: t.up = -raycastHit.normal; break;
            case 3: t.right = raycastHit.normal; break;
            case 4: t.right = -raycastHit.normal; break;
            case 5: t.forward = -raycastHit.normal; break;
          }
          SET_DIRTY( t );
        }
      }
      else
      {
        v = t.position;
      }
    }


    static Dictionary<int, int> CreateExcludeList(Transform[] t)
    {
      var result = new Dictionary<int , int>();
      foreach (var transform in t)
      {
        foreach (var componentsInChild in transform.GetComponentsInChildren<Transform>())
          if (!result.ContainsKey( componentsInChild.GetInstanceID() )) result.Add( componentsInChild.GetInstanceID(), 0 );
        if (!result.ContainsKey( transform.GetInstanceID() )) result.Add( transform.GetInstanceID(), 0 );
      }
      return result;
    }

    static Vector3[] MIN_MAX = new Vector3[2];
    static Vector3[] boundsVertices = new Vector3[8];

    // BASED ON MESH VERTICES
    static bool CalcBoundsForGameObjectHierarchy(GameObject go)
    {

      MIN_MAX[0] = Vector3.zero;
      MIN_MAX[1] = Vector3.zero;

      foreach (var mf in go.GetComponentsInChildren<MeshFilter>())
      {
        if (!mf.sharedMesh) continue;
        var vertices = mf.sharedMesh.vertices;
        for (int i = 0 ; i < vertices.Length ; i++)
        {
          for (int DIR = 0 ; DIR < 3 ; DIR++)
          {
            if (vertices[i][DIR] < MIN_MAX[0][DIR]) MIN_MAX[0][DIR] = vertices[i][DIR];
            if (vertices[i][DIR] > MIN_MAX[1][DIR]) MIN_MAX[1][DIR] = vertices[i][DIR];
          }
        }
      }

      var result = MIN_MAX[0] != Vector3.zero || MIN_MAX[1] != Vector3.zero;

      MIN_MAX[0] = go.transform.TransformPoint( MIN_MAX[0] );
      MIN_MAX[1] = go.transform.TransformPoint( MIN_MAX[1] );

      for (int i = 0 ; i < 8 ; i++) boundsVertices[i].Set( MIN_MAX[(i % 2) / 1].x, MIN_MAX[(i % 4) / 2].y, MIN_MAX[(i % 8) / 4].z );

      return result;

    }

    // BASED ON GAMEOBJECT BOUNDS
    /*    static bool CalcBoundsForGameObjectHierarchy( GameObject go )
            {

                Bounds bounds = new Bounds( );
                bounds.center = go.transform.position;
                bounds.extents = Vector3.zero;

                if ( go.GetComponent<Renderer>( ) )
                {
                    bounds = go.GetComponent<Renderer>( ).bounds;
                }

                foreach ( Renderer renderer in go.GetComponentsInChildren<Renderer>( ) )
                {
                    if ( !renderer ) continue;
                    bounds.Encapsulate( renderer.bounds );
                }

                MIN_MAX[0] = (bounds.min);
                MIN_MAX[1] = (bounds.max);
                for ( int i = 0; i < 8; i++ ) boundsVertices[i].Set( MIN_MAX[(i % 2) / 1].x, MIN_MAX[(i % 4) / 2].y, MIN_MAX[(i % 8) / 4].z );

                return bounds.extents != Vector3.zero;

            }*/
  }









  [CanEditMultipleObjects, CustomEditor( typeof( Transform ) )]
  public class SnapButtonsForInspectorGUI : Editor
  {
    //params
    bool DRAW_SURFACE_BUTTON = true;


    ////////
    //////// INSPECTOR DECORATOR ////////
    ////////

    // inspector vars
    static Type decoratedEditorType;
    Editor EDITOR_INSTANCE;
    void OnDisable() { if (EDITOR_INSTANCE) DestroyImmediate( EDITOR_INSTANCE ); }



    public override void OnInspectorGUI()
    {
      if (targets == null || targets.Length == 0) return;
      if (decoratedEditorType == null) decoratedEditorType = System.Reflection.Assembly.GetAssembly( typeof( Editor ) ).GetType( "UnityEditor.TransformInspector", true );
      if (!EDITOR_INSTANCE) EDITOR_INSTANCE = Editor.CreateEditor( targets, decoratedEditorType );
      if (buttonStyle == null || !snapContent.image) InitStyles();


      var start_y = EditorGUILayout.GetControlRect( GUILayout.Height( 0 ) , GUILayout.ExpandHeight( false ) ).y;
      var window_width = GUILayoutUtility.GetLastRect().x + GUILayoutUtility.GetLastRect().width;


      //////// BUTTONS ////////
      CUSTOM_GUI( start_y, window_width );

      // RIGHT PADDING
      GUILayout.BeginHorizontal(); GUILayout.BeginVertical();

      //////// INTERNAL GUI ////////
      EDITOR_INSTANCE.OnInspectorGUI();

      // RIGHT PADDING
      GUILayout.EndVertical(); GUILayout.Space( buttonRectWidth ); GUILayout.EndHorizontal();

    }







    ////////
    //////// GUI BUTTONS DRAWING ////////
    ////////

    // buttons vars
    const float BUT_SIZE = 16;

    float buttonRectWidth { get { return (DRAW_SURFACE_BUTTON ? BUT_SIZE * 2 : BUT_SIZE) - 3; } }
    Color enableColor = new Color( 0.9f , 0.6f , 0.3f , 1f );
    GUIStyle buttonStyle;
    Rect r = new Rect();
    GUIContent snapContent = new GUIContent( "" , "- Left CLICK to enable/disable snap\n- Right CLICK to select axises\n- Use CTRL to use Unity internal snapping" );
    GUIContent raycastContent = new GUIContent( "" , "- Left CLICK to enable/disable surface placement raycast\n- Right CLICK to select align mode\n- Use CTRL+SHIFT to use Unity surface snapping" );
    GUIContent normalContent = new GUIContent( "" , "- Align Object by surface normal if used surface placement raycast\n- Right CLICK to choose up Vector" );

    void InitStyles()
    {
      /*var s = ScriptableObject.CreateInstance<SnapButtonsForInspector>();
      snapContent.image = s.SnapImage ?? Texture2D.blackTexture;
      raycastContent.image = s.SurfaceImage ?? Texture2D.blackTexture;
      normalContent.image = s.NormalImage ?? Texture2D.blackTexture;*/

      snapContent.image = new Icon_SnapButtonsForInspectorImage();
      raycastContent.image = new Icon_SnapButtonsForInspectorSurface();
      normalContent.image = new Icon_SnapButtonsForInspectorNormal();

      buttonStyle = new GUIStyle( GUI.skin.button );
      buttonStyle.padding = new RectOffset( 3, 3, 3, 3 );
    }

    void CUSTOM_GUI(float start_y, float window_width)
    {
      r.x = window_width - buttonRectWidth;
      r.y = start_y + 1;
      r.width = BUT_SIZE;
      r.height = BUT_SIZE;


      DO_SNAP_BUTTON( r, Snap.SNAP_POS );
      if (DRAW_SURFACE_BUTTON) DO_SURFACE_BUTTON( new Rect( r.x + BUT_SIZE, r.y, r.width, r.height ) );
      r.y += r.height + 2;
      DO_SNAP_BUTTON( r, Snap.SNAP_ROT );
      if (DRAW_SURFACE_BUTTON) DO_NORMAL_BUTTON( new Rect( r.x + BUT_SIZE, r.y, r.width, r.height ) );
      r.y += r.height + 2;
      DO_SNAP_BUTTON( r, Snap.SNAP_SCALE );

    }

    void DO_SNAP_BUTTON(Rect r, VectorPref pref)
    {
      if (GUI.Button( r, snapContent, buttonStyle ))
      {
        if (Event.current.button == 0) pref.ENABLE.Set( !pref.ENABLE );
        else
        {
          var menu = new GenericMenu();
          menu.AddItem( new GUIContent( pref.ENABLE.name ), pref.ENABLE, () => pref.X.Set( !pref.X ) );
          menu.AddSeparator( "" );
          menu.AddItem( new GUIContent( Snap.SNAP_AUTOAPPLY.name ), Snap.SNAP_AUTOAPPLY, () => Snap.SNAP_AUTOAPPLY.Set( !Snap.SNAP_AUTOAPPLY ) );
          menu.AddSeparator( "" );
          menu.AddItem( new GUIContent( pref.X.name ), pref.X, () => pref.X.Set( !pref.X ) );
          menu.AddItem( new GUIContent( pref.Y.name ), pref.Y, () => pref.Y.Set( !pref.Y ) );
          menu.AddItem( new GUIContent( pref.Z.name ), pref.Z, () => pref.Z.Set( !pref.Z ) );
          menu.AddSeparator( "" );
          menu.AddItem( new GUIContent( pref.USE_LOCAL.name ), pref.USE_LOCAL, () => pref.USE_LOCAL.Set( !pref.USE_LOCAL ) );
          menu.AddSeparator( "" );
          menu.AddItem( new GUIContent( "Open Unity Snap Settings" ), false, () => EditorApplication.ExecuteMenuItem( "Edit/Snap Settings..." ) );
          menu.AddItem( new GUIContent( "Open Plugin Snap Settings" ), false, SnapWindow.Open );
          menu.ShowAsContext();
        }
      }
      if (Event.current.type == EventType.Repaint && pref.ENABLE)
      {
        var oldColor = GUI.color;
        GUI.color *= enableColor;
        buttonStyle.Draw( r, snapContent, true, true, false, true );
        GUI.color = oldColor;
      }
    }

    void DO_SURFACE_BUTTON(Rect r)
    {
      if (GUI.Button( r, raycastContent, buttonStyle ))
      {
        if (Event.current.button == 0)
        {
          Snap.PLACE_ON_SURFACE_ENABLE.Set( !Snap.PLACE_ON_SURFACE_ENABLE );
        }
        else
        {
          var menu = new GenericMenu();
          menu.AddItem( new GUIContent( Snap.PLACE_ON_SURFACE_ENABLE.name ), Snap.PLACE_ON_SURFACE_ENABLE, () => Snap.PLACE_ON_SURFACE_ENABLE.Set( !Snap.PLACE_ON_SURFACE_ENABLE ) );
          menu.AddSeparator( "" );
          menu.AddItem( new GUIContent( Snap.ALIGN_BY[0] ), Snap.PLACE_ON_SURFACE_ALIGNBYMOUSE, () => Snap.PLACE_ON_SURFACE_ALIGNBYMOUSE.Set( true ) );
          // menu.AddItem( new GUIContent( Snap.ALIGN_BY[1] ), !Snap.PLACE_ON_SURFACE_ALIGNBYMOUSE, () => Snap.PLACE_ON_SURFACE_ALIGNBYMOUSE.Set( false ) );
          menu.AddDisabledItem( new GUIContent( Snap.ALIGN_BY[1] ) );
          menu.AddSeparator( "" );
          menu.AddItem( new GUIContent( Snap.PLACE_ON_SURFACE_BOUNDS.name ), Snap.PLACE_ON_SURFACE_BOUNDS, () => Snap.PLACE_ON_SURFACE_BOUNDS.Set( !Snap.PLACE_ON_SURFACE_BOUNDS ) );
          menu.AddSeparator( "" );
          menu.AddItem( new GUIContent( "Open Plugin Snap Settings" ), false, SnapWindow.Open );
          menu.ShowAsContext();
        }
      }
      if (Event.current.type == EventType.Repaint && Snap.PLACE_ON_SURFACE_ENABLE)
      {
        var oldColor = GUI.color;
        GUI.color *= enableColor;
        buttonStyle.Draw( r, raycastContent, true, true, false, true );
        GUI.color = oldColor;
      }
    }

    void DO_NORMAL_BUTTON(Rect r)
    {
      var en = GUI.enabled;
      GUI.enabled = Snap.PLACE_ON_SURFACE_ENABLE;
      if (GUI.Button( r, normalContent, buttonStyle ))
      {
        if (Event.current.button == 0)
        {
          Snap.ALIGN_BY_NORMAL.Set( !Snap.ALIGN_BY_NORMAL );
        }
        else
        {
          var menu = new GenericMenu();
          menu.AddItem( new GUIContent( Snap.ALIGN_BY_NORMAL.name ), Snap.ALIGN_BY_NORMAL, () => Snap.ALIGN_BY_NORMAL.Set( !Snap.ALIGN_BY_NORMAL ) );
          menu.AddSeparator( "" );
          for (int i = 0 ; i < Snap.VECTORS.Length ; i++)
          {
            var captureI = i;
            menu.AddItem( new GUIContent( Snap.VECTORS_STRING[i] ), Snap.ALIGN_UP_VECTOR == i, () => Snap.ALIGN_UP_VECTOR.Set( captureI ) );
          }
          menu.AddSeparator( "" );
          menu.AddItem( new GUIContent( "Open Plugin Snap Settings" ), false, SnapWindow.Open );
          menu.ShowAsContext();
        }
      }
      if (GUI.enabled && Event.current.type == EventType.Repaint && Snap.ALIGN_BY_NORMAL)
      {
        var oldColor = GUI.color;
        GUI.color *= enableColor;
        buttonStyle.Draw( r, normalContent, true, true, false, true );
        GUI.color = oldColor;
      }
      GUI.enabled = en;
    }


  }








  ////////
  //////// PREFS CLASSES ////////
  ////////

  class VectorPref
  {
    public CachedPref ENABLE;
    public CachedPref X;
    public CachedPref Y;
    public CachedPref Z;
    public CachedPref USE_LOCAL;

    public VectorPref(string keyPrefix, bool defaultUseLocalValue = false, bool lockUseLocalValue = false, string enableName = null)
    {
      ENABLE = new CachedPref( keyPrefix + "_ENABLE", false ) { name = enableName ?? "Enable" };
      X = new CachedPref( keyPrefix + "_X", true ) { name = "Snap X-axis" };
      Y = new CachedPref( keyPrefix + "_Y", true ) { name = "Snap Y-axis" };
      Z = new CachedPref( keyPrefix + "_Z", true ) { name = "Snap Z-axis" };

      USE_LOCAL = new CachedPref( keyPrefix + "_USE_LOCAL", defaultUseLocalValue, lockUseLocalValue ) { name = "Use Local Space" };
    }
  }

  class CachedPref
  {
    public CachedPref(string registryKey, bool defaulValue, bool lockValue = false)
    {
      this.m_registryKey = registryKey;
      this.m_boolDefaultValue = defaulValue;
      this.m_lock = lockValue;
    }

    public CachedPref(string registryKey, int defaulValue, bool lockValue = false)
    {
      this.m_registryKey = registryKey;
      this.m_intDefaultValue = defaulValue;
      this.m_lock = lockValue;
    }

    public static implicit operator bool(CachedPref d) { return d.m_boolValue; }
    public void Set(bool value)
    {
      if (m_lock) return;
      m_boolValue = value;
    }

    public static implicit operator int(CachedPref d) { return d.m_intValue; }
    public void Set(int value)
    {
      if (m_lock) return;
      m_intValue = value;
    }


    public string name;

    string m_registryKey;
    bool m_lock;

    bool m_boolDefaultValue;
    bool? m_boolCache;
    bool m_boolValue
    {
      get { return m_boolCache ?? (m_boolCache = EditorPrefs.GetBool( m_registryKey, m_boolDefaultValue )).Value; }
      set { EditorPrefs.SetBool( m_registryKey, (m_boolCache = value).Value ); }
    }

    int m_intDefaultValue;
    int? m_intCache;
    int m_intValue
    {
      get { return m_intCache ?? (m_intCache = EditorPrefs.GetInt( m_registryKey, m_intDefaultValue )).Value; }
      set { EditorPrefs.SetInt( m_registryKey, (m_intCache = value).Value ); }
    }
  }
}
#endif









//http://wiki.unity3d.com/index.php/PlaceSelectionOnSurface
/*[MenuItem ("GameObject/Place Selection On Surface")]
static void CreateWizard ()
{
Transform[] transforms = Selection.GetTransforms(SelectionMode.Deep |
                                                 SelectionMode.ExcludePrefab | SelectionMode.OnlyUserModifiable);
 
    if (transforms.Length > 0 && EditorUtility.DisplayDialog("Place Selection On Surface?",
"Are you sure you want to place " + transforms.Length +
((transforms.Length > 1) ? " objects" : " object") +
" on the surface in the -Y direction?", "Place", "Do Not Place"))
{
    foreach (Transform transform in transforms)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            transform.position = hit.point;
            Vector3 randomized = Random.onUnitSphere;
            randomized = new Vector3(randomized.x, 0F, randomized.z);
            transform.rotation = Quaternion.LookRotation(randomized, hit.normal);
        }
    }
}
}*/




