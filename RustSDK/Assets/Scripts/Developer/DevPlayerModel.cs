using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DevPlayerModel : MonoBehaviour 
{
	public PlayerModel playerModel;
	internal int editSlot = -1;

	void Start () 
	{
		playerModel.AlwaysAnimate( true );
		playerModel.censorshipEnabled = false;

		for (int i=0; i<10; i++)
		{
			if ( playerModel.multiMesh.parts.Count() <= i )
				playerModel.multiMesh.parts.Add( null );

			var partName = PlayerPrefs.GetString( "PlayerModelSlot" + i.ToString() );
			if ( !string.IsNullOrEmpty( partName ) )
			{
				playerModel.multiMesh.parts[i] = GameManager.FindPrefab( partName );
			}

			playerModel.Rebuild();
		}

	}

	float yPos = 0;

	float Slider( string strName, float value, float low, float high, float w )
	{
		GUI.Label( new Rect( 0, yPos, w * 0.2f, 20.0f ), strName );

		var val =  GUI.HorizontalSlider( new Rect( w * 0.25f, yPos + 5, w - w * 0.25f, 20.0f ), value, low, high );
		yPos += 20.0f;

		return val;
	}

	string TextArea( string strName, string value, float w )
	{
		GUI.Label( new Rect( 0, yPos, w * 0.2f, 20.0f ), strName );

		var val =  GUI.TextArea( new Rect( w * 0.25f, yPos + 5, w - w * 0.25f, 20.0f ), value );
		yPos += 20.0f;

		return val;
	}

	bool Checkbox( string strName, bool value, float w )
	{
		GUI.Label( new Rect( 0, yPos, w * 0.2f, 20.0f ), strName );

		var val =  GUI.Toggle( new Rect( w * 0.25f, yPos + 5, w - w * 0.25f, 20.0f ), value, "" );
		yPos += 20.0f;

		return val;
	}

	bool Button( string strName, float w )
	{
		var val =  GUI.Button( new Rect( w * 0.25f, yPos + 5, w - w * 0.25f, 20.0f ), strName );
		yPos += 20.0f;

		return val;
	}

	void OnGUI()
	{
		GUIPlayerSlots( 20.0f, Screen.height - 80.0f - 20.0f, 600.0f, 80.0f );
	}

	internal string[] clothingSlots = { "feet", "hands", "head", "legs", "torso" };

	void GUIPlayerSlots( float x, float y, float w, float h )
	{
		GUI.BeginGroup( new Rect( x, y, w, h ) );
			GUI.Box( new Rect( 0, 0, w, h ), "Clothing Slots" );

			h -= 30.0f;
			w -= 40.0f;

			GUI.BeginGroup( new Rect( 20.0f, 20.0f, w, h ) );

			var buttonWidth = w / 5.0f;
			for ( int i=0; i < playerModel.multiMesh.parts.Count; i++ )
			{
				var slot = playerModel.multiMesh.parts[i];
				if ( GUI.Button( new Rect( (i%5) * buttonWidth,  i < 5 ? 0 : 25.0f, buttonWidth - 5.0f, 20.0f ), slot ? slot.name : "None" ) )
				{
					if ( editSlot == i ) editSlot = -1;
					else editSlot = i;
				}
			}

			GUI.EndGroup();

			h += 30.0f;
			w += 40.0f;

		GUI.EndGroup();

		if ( editSlot >= 0 )
		{
			var folderName = "clothing/" + clothingSlots[( editSlot % 5 )];

			var prefabNames = GameManager.FindPrefabNames( folderName ).ToList();
			prefabNames.Insert( 0, "None" );

			var cleanedNames = prefabNames.ToArray();
			for ( int i=0; i< cleanedNames.Length; i++)
			{
				cleanedNames[i] = cleanedNames[i].Replace( folderName + "/", "" );
			}


			var rows = Mathf.CeilToInt( (float)prefabNames.Count() / 5.0f );

			y -= (rows * 40.0f ) + 20.0f;
			h = ( rows * 40.0f );

			GUI.BeginGroup( new Rect( x, y, w, h ) );

			int iSelection = GUI.SelectionGrid( new Rect( 0, 0, w, h ), -1, cleanedNames, 5 );
				if ( iSelection >= 0 )
				{
					if ( iSelection == 0 )
					{
						playerModel.multiMesh.parts[editSlot] = null;
						PlayerPrefs.SetString( "PlayerModelSlot" + editSlot.ToString(), "" );
					}
					else
					{
						playerModel.multiMesh.parts[editSlot] = GameManager.FindPrefab( prefabNames[iSelection] );
						PlayerPrefs.SetString( "PlayerModelSlot" + editSlot.ToString(), prefabNames[iSelection] );
					}

					playerModel.Rebuild();
					editSlot = -1;
				}

			GUI.EndGroup();
		}

		
	}

}
