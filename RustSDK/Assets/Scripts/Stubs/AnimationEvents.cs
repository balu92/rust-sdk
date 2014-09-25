using UnityEngine;
using System.Collections;

public class AnimationEvents : MonoBehaviour
{
	public void Hide( string childName )
	{
		var ob = transform.FindChild( childName );
		if ( !ob ) return;

		ob.gameObject.GetComponent<Renderer>().enabled = false;
	}

	public void Show( string childName )
	{
		var ob = transform.FindChild( childName );
		if ( !ob ) return;

		ob.gameObject.GetComponent<Renderer>().enabled = true;
	}

	public void DoEffect( string strEvent )
	{
	}

	public void Broadcast( string strEvent )
	{
	}

	public void Message( string strEvent )
	{
	}

	public void Strike()
	{
	}
}
