using UnityEngine;
using System.Collections;
using System.Linq;

public class PlayerModel : MonoBehaviour
{
	protected Animator animator;

	public SkinnedMultiMesh multiMesh;
	protected Vector3 positionLast;
	protected Vector3 velocitySmoothed;

	protected int speed			= Animator.StringToHash( "speed" );
	protected int forward		= Animator.StringToHash( "forward" );
	protected int right			= Animator.StringToHash( "right" );
	protected int wake_up		= Animator.StringToHash( "wake_up" );
	protected int ducked		= Animator.StringToHash( "ducked" );
	protected int grounded		= Animator.StringToHash( "grounded" );
	protected int waterlevel	= Animator.StringToHash( "waterlevel" );
	protected int attack		= Animator.StringToHash( "attack" );
	protected int sleeping		= Animator.StringToHash( "sleeping" );
	protected int deploy		= Animator.StringToHash( "deploy" );
	protected int reload		= Animator.StringToHash( "reload" );
	protected int holster		= Animator.StringToHash( "holster" );
	protected int flinch		= Animator.StringToHash( "flinch" );
	protected int flinch_location = Animator.StringToHash ("flinch_location");

	public bool censorshipEnabled = true;
	public GameObject censorshipCube;

	public bool visible = true;

	public Vector3 speedOverride = new Vector3();
	public Vector3 position		= new Vector3();
	public Quaternion rotation	= new Quaternion();
	public Vector3 aimAt		= new Vector3();

	protected string holdType		= "";
	protected string worldModel		= "";
	protected GameObject worldModelObject;
	protected ModelState modelState	= null;

	public bool drawShadowOnly = false;
	public Material shadowMaterial;

	void OnEnable()
	{
		position = transform.position;
		rotation = transform.rotation;

		aimAt = position + transform.forward * 1000.0f;

		multiMesh = GetComponent<SkinnedMultiMesh>();
		animator = GetComponent<Animator>();

		if ( animator != null )
		{
			animator.SetLookAtPosition( Vector3.zero );
			animator.SetLookAtWeight( 1.0f, 0.0f, 1.0f, 1.0f, 0.5f );
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawLine( position, aimAt );
	}

	public void Rebuild()
	{
		multiMesh.materialOverride = drawShadowOnly ? shadowMaterial : null;
		multiMesh.RebuildModel();

		// Should the censorship cube be enabled
		{
			var clothes = GetComponentsInChildren<ClothingItem>();
			var censored = clothes.Any( x => x.showCensorshipCube == true );

			if ( censorshipCube != null )
				censorshipCube.SetActive( !drawShadowOnly && censored );
		}

		//
		// If we have a world model, replace its material too
		//
		if ( worldModelObject )
		{
			if ( multiMesh.materialOverride )
				MaterialReplacement.ReplaceRecursive( worldModelObject, multiMesh.materialOverride );
			else 
				MaterialReplacement.Reset( worldModelObject );
		}
	}

	public void UpdateModelState( ModelState ms )
	{
		modelState = ms;
	}

	public void Attack()
	{
		//animator.SetTrigger( attack );
		animator.Play( attack, -1, 0.0f );
	}
	public void Deploy()
	{
		animator.SetTrigger( deploy );
	}
	public void Reload()
	{
		animator.SetTrigger( reload );
	}
	public void Holster()
	{
		animator.SetTrigger( holster );
	}
	public void Flinch()
	{
		animator.SetTrigger( flinch );
	}

	public void SetHoldType( string strType )
	{
		if ( holdType == strType ) return;
		holdType = strType;

		animator.SetLayersWeight( "hold_", 0.0f );
		animator.SetLayersWeight( "overlay_", 0.0f );

		if ( !string.IsNullOrEmpty( holdType ) )
		{
			animator.SetLayersWeight( "hold_" + holdType, 1.0f );
			animator.SetLayersWeight( "overlay_" + holdType, 1.0f );
		}
	}

	internal void UpdateRotation( Vector3 MoveDelta )
	{
		var targetAng = Quaternion.Euler( new Vector3( 0, rotation.eulerAngles.y, 0 ) );

		// We're moving - face the direction
		/*
		if ( MoveDelta.x != 0.0f || MoveDelta.z != 0.0f )
		{
			MoveDelta.y = 0;
			MoveDelta = MoveDelta.normalized;
			if ( MoveDelta.magnitude > 0.5f )
			{
				var MoveRot = Quaternion.LookRotation( MoveDelta.normalized * -1.0f, Vector3.up );
				var angle = Quaternion.Angle( MoveRot, targetAng );
				if ( angle < 120.0f )
				{
					transform.rotation = Quaternion.Lerp( transform.rotation, MoveRot, 0.05f );
					return;
				}
			}
		}
		 * */

		// We're just chilling
		{
		//	var angle = Quaternion.Angle( transform.rotation, targetAng );
		//	if ( angle > 45.0f )
			{
				transform.rotation = Quaternion.Lerp( transform.rotation, targetAng, Time.deltaTime * 4.0f );
			}
		}
	}

	public void FrameUpdate()
	{
		if ( !gameObject.activeSelf ) return;
		if ( animator == null ) return;

		multiMesh.SetVisible( visible );

		if ( !visible )
			return;

		var MoveDelta = transform.position - position;
		var GroundSpeed = transform.InverseTransformDirection( MoveDelta ) / Time.deltaTime;

		if ( MoveDelta.magnitude > 0.0f )
		{
			transform.position = position;
		}

		UpdateRotation( MoveDelta );

		float smoothTime = 1.0f / 10.0f;

		if ( speedOverride.magnitude != 0.0f )
		{
			GroundSpeed = speedOverride;
		}

		if ( float.IsNaN( GroundSpeed.x ) || float.IsNaN( GroundSpeed.y ) || float.IsNaN( GroundSpeed.z ) )
			return;

		animator.SetFloat( speed, GroundSpeed.magnitude, smoothTime, Time.deltaTime );
		animator.SetFloat( forward, GroundSpeed.z, smoothTime, Time.deltaTime );
		animator.SetFloat( right, GroundSpeed.x, smoothTime, Time.deltaTime );

		if ( modelState != null )
		{
			animator.SetBool( ducked, modelState.ducked );
			animator.SetBool( grounded, modelState.onground );
			animator.SetFloat( waterlevel, modelState.waterLevel );
			animator.SetBool( sleeping, modelState.sleeping );
			animator.SetInteger( flinch_location, (int)modelState.flinchLocation );

			if ( modelState.sleeping )
			{
				SetHoldType( "" );
			}
			else
			{
				SetHoldType( modelState.holdType );
			}
		}
	}

	void OnAnimatorIK( int layer )
	{
		if ( !visible ) return;
		if ( animator == null ) return;
		if ( layer != 0 ) return;

		animator.SetLookAtPosition( aimAt );

		var ikWeight = animator.GetFloat( "ikWeight" );

		if ( string.IsNullOrEmpty( holdType ) ) // Not holding a weapon
		{
			animator.SetLookAtWeight( ikWeight, 0.3f, 0.6f, 1.0f, 0.3f );
		}
		else
		{
			animator.SetLookAtWeight( ikWeight, 1.0f, 1.0f, 1.0f, 0.5f );
		}		
	}


	public void AlwaysAnimate( bool b )
	{
		if ( b )
			animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		else
			animator.cullingMode = AnimatorCullingMode.BasedOnRenderers;
	}

	public Transform FindBone( string strName )
	{
		return multiMesh.FindBone( strName );
	}

	public void Clear()
	{
		multiMesh.Clear();
		multiMesh.parts.Clear();

		AddPart( "head/skin_head" );
		AddPart( "feet/skin_feet" );
		AddPart( "legs/skin_legs" );
		AddPart( "torso/skin_torso" );
		AddPart( "hands/skin_hands" );

		if ( censorshipCube != null )
			censorshipCube.SetActive( true );
	}

	public void AddPart( string strName )
	{
		var go = GameManager.FindPrefab( "clothing/" + strName );
		if ( !go )
		{
			Debug.LogWarning( "Couldn't find clothes part " + strName );
			return;
		}

		go.SetActive( true );
		multiMesh.parts.Add( go );
	}

	public void RemovePart( string strName )
	{
		multiMesh.parts.RemoveAll( x => x.name == strName );
	}

	public void UpdateFrom( PlayerModel mdl )
	{
		multiMesh.parts = mdl.multiMesh.parts;
		Rebuild();
	}
}

public partial class ModelState
{
	public bool		ducked = false;
	public bool		jumped = false;
	public bool		onground = true;
	public bool		sleeping = false;
	public float	waterLevel = 0.0f;
	public uint		holdTypeID;
	public uint		flinchLocation;


	// Not networked etc
	public GameObject heldItem;
	public string	holdType;

	public static bool Equal( ModelState a, ModelState b )
	{
		if ( System.Object.ReferenceEquals( a, b ) ) return true;
		if ( ( (object)a == null ) || ( (object)b == null ) ) return false;

		if ( a.ducked != b.ducked ) return false;
		if ( a.jumped != b.jumped ) return false;
		if ( a.onground != b.onground ) return false;
		if ( a.sleeping != b.sleeping ) return false;
		if ( a.waterLevel != b.waterLevel ) return false;
		if ( a.flinchLocation != b.flinchLocation ) return false;

		return true;
	}
}