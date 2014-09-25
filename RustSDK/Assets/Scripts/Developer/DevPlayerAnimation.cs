using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DevPlayerAnimation : MonoBehaviour
{
	public PlayerModel playerModel;
	public ModelState modelState = new ModelState();

	internal Vector3 viewOffset = new Vector3();
	internal string holdType = "";
	internal string[] holdTypes = { "", "rock", "bow", "melee", "rifle", "torch", "grenade", "pickaxe", "pistol", "thompson" };
	internal int holdTypesIndex = 0;
	internal string flinchType = "";
	internal string[] flinchTypes = { "head", "torso1", "torso2", "torso3", "right_arm", "right_hand", "right_leg", "left_leg", "left_hand", "left_arm" };
	internal int flinchTypesIndex = 0;
	internal bool aimAtCamera = false;
	internal int modelLOD = -1;

	void Start()
	{
		playerModel.AlwaysAnimate( true );
	}

	void Update()
	{
		playerModel.FrameUpdate();

		if ( aimAtCamera )
			playerModel.aimAt = Camera.main.transform.position;
		else
			playerModel.aimAt = transform.position + transform.forward * 5.0f + playerModel.transform.localToWorldMatrix.MultiplyPoint( viewOffset );

		playerModel.SetHoldType( holdType );
		playerModel.UpdateModelState( modelState );
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
		var w = Screen.width * 0.2f;

		GUI.BeginGroup( new Rect( Screen.width - w - 20.0f, 20.0f, w, 550.0f ) );
		
			GUI.Box( new Rect( 0, 0, w, 550 ), "Player Animation" );

			w -= 40.0f;
			GUI.BeginGroup( new Rect( 20.0f, 30.0f, w, 500.0f ) );

				yPos = 0.0f;
				playerModel.speedOverride.z		= Slider( "Forward", playerModel.speedOverride.z, -10.0f, 10.0f, w );
				playerModel.speedOverride.x		= Slider( "Sideward", playerModel.speedOverride.x, -10.0f, 10.0f, w );
				viewOffset.y					= Slider( "Look Up", viewOffset.y, -10.0f, 10.0f, w );
				viewOffset.x					= Slider( "Look L/R", viewOffset.x, -10.0f, 10.0f, w );

				yPos += 5.0f;
				int newHoldIndex				= (int)Slider( "HoldType", (int)holdTypesIndex, 0, (float)holdTypes.Length - 1, w );
				holdType						= TextArea( "", holdType, w );

				modelState.ducked				= Checkbox( "Ducked", modelState.ducked, w );
				modelState.onground				= Checkbox( "Grounded", modelState.onground, w );
				modelState.sleeping				= Checkbox( "Sleeping", modelState.sleeping, w );
				modelState.waterLevel			= Slider( "WaterLevel", modelState.waterLevel, 0.0f, 1.0f, w );
				aimAtCamera						= Checkbox( "Aim @ Cam", aimAtCamera, w );
		
				if ( Button( "Deploy", w ) )
				{
					playerModel.Deploy();
				}
				if ( Button( "Attack", w ) )
				{
					playerModel.Attack();
				}
				if ( Button( "Reload", w ) )
				{
					playerModel.Reload();
				}
				if ( Button( "Holster", w ) )
				{
					playerModel.Holster();
				}
				
				int newFlinchTypeIndex		= (int)Slider( "Flinch Location", (int)flinchTypesIndex, 0.0f, 9.0f, w );
				flinchType					= TextArea ( "", flinchType, w );
				if ( Button( "Flinch", w ) )
				{
					playerModel.Flinch();
				}

				int newModelLOD				= (int)Slider( "LOD", (int)modelLOD, -1, 3, w );
				



			GUI.EndGroup();

		GUI.EndGroup();


		if ( newFlinchTypeIndex != flinchTypesIndex )
		{
			flinchType = flinchTypes[newFlinchTypeIndex];
			flinchTypesIndex = newFlinchTypeIndex;
			modelState.flinchLocation = (uint)flinchTypesIndex;
		}

		if ( newHoldIndex != holdTypesIndex )
		{
			holdType = holdTypes[newHoldIndex];
			holdTypesIndex = newHoldIndex;
		}

		if ( newModelLOD != modelLOD )
		{
			playerModel.GetComponent<LODGroup>().ForceLOD( newModelLOD );
			modelLOD = newModelLOD;
		}

	}

}
