using UnityEngine;
using System.Collections;

public class DrawSkeleton : MonoBehaviour
{

	void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		DrawTransform( transform );
	}

	void DrawTransform( Transform t )
	{
		for( int i=0; i<t.childCount; i++)
		{
			Gizmos.DrawLine( t.position, t.GetChild(i).position );

			DrawTransform( t.GetChild( i ) );
		}
	}
}
