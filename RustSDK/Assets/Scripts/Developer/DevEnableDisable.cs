using UnityEngine;
using System.Collections;

public class DevEnableDisable : MonoBehaviour 
{
	public GameObject[] Objects;
	public string CookieName = "Cookie";

	void Start () 
	{
		Invoke( "ApplyLastSettings", 0.5f );
	}

	void ApplyLastSettings()
	{
		foreach ( var obj in Objects )
		{
			int i = PlayerPrefs.GetInt( "DevEnable_" + CookieName + "_" + obj.name );
			if ( i > 0 )
			{
				obj.SetActive( i == 1 );
			}
		}
	}

	void OnGUI()
	{
		var y = 20.0f;

		foreach ( var obj in Objects )
		{
			bool bEnabled = obj.activeSelf;
			bool bEnable = GUI.Toggle( new Rect( 20, y += 25.0f, 200.0f, 20.0f ), bEnabled, obj.name );

			if ( bEnabled != bEnable )
			{
				obj.SetActive( bEnable );
				PlayerPrefs.SetInt( "DevEnable_" + CookieName + "_" + obj.name, bEnable ? 1: 2 );
			}
		}
	}

}
