using UnityEngine;
using UnityEngine.UI;

public class FingerDebug : MonoBehaviour 
{
    public RawImage FingerIcon;
    public bool ShowGUI = false;
    public Rect GuiRect = new Rect(5, 5, 500, 500);
    RawImage[] icons;
    float distance = -1;
    
    void Start()
    {
        if( !FingerGestures.Instance )
        {
            Debug.LogError( "FG instance not present" );
            enabled = false;
            return;
        }

        icons = new RawImage[FingerGestures.Instance.MaxFingers];
        for( int i = 0; i < icons.Length; ++i )
        {
            RawImage icon = Instantiate( FingerIcon );
            icon.transform.SetParent(this.transform, false);
            icon.enabled = false;
            icons[i] = icon;
        }

        FingerIcon.enabled = false;
    }

    void Update()
    {
        if( !FingerGestures.Instance )
            return;
        
        if( FingerGestures.Touches.Count >= 2 )
        {
            distance = Vector2.Distance( FingerGestures.Touches[0].Position, FingerGestures.Touches[1].Position );
        }
        else
        {
            distance = -1;
        }

        int i = 0;
        for( ; i < FingerGestures.Touches.Count; ++i )
        {
            FingerGestures.Finger finger = FingerGestures.Touches[i];

            Rect inset = icons[i].uvRect;
            inset.x = finger.Position.x - inset.width/2;
            inset.y = finger.Position.y - inset.height/2;
            icons[i].uvRect = inset;
            icons[i].enabled = true;
        }

        for( ; i < icons.Length; ++i )
            icons[i].enabled = false;
    }

    void OnGUI()
    {
        if( !ShowGUI )
            return;

        if( !FingerGestures.Instance )
            return;

        GUILayout.BeginArea( GuiRect );
        GUILayout.BeginVertical();

        GUILayout.Label( "Input.Touches: " + Input.touchCount );
        GUILayout.Label( "FingerGestures: " + FingerGestures.Touches.Count );

        foreach( FingerGestures.Finger touch in FingerGestures.Touches )
        {
            GUILayout.Label( string.Format( "{0} moving:{1}", touch, touch.IsMoving ) );

            foreach( GestureRecognizer recognizer in touch.GestureRecognizers )
            {
                GUILayout.Label( touch.ToString() + ": " + recognizer );
            }
        }

        if( distance >= 0 )
            GUILayout.Label( "Finger[0->1] Distance: " + distance.ToString( "N0" ) );

        GUILayout.Space( 5 );

        GUILayout.Label( "Clusters: " + FingerGestures.DefaultClusterManager.Clusters.Count + " [Pool: " + FingerGestures.DefaultClusterManager.GetClustersPool().Count + "]" );

        foreach( FingerClusterManager.Cluster cluster in FingerGestures.DefaultClusterManager.Clusters )
            GUILayout.Label( "  -> Cluster #" + cluster.Id + ": " + cluster.Fingers.Count + " fingers" );
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
