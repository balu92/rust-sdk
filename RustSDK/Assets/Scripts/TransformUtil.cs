using UnityEngine;
using System.Collections.Generic;
using System.Linq;

//
// Provides transform utility methods
//

public static class TransformUtil
{
	//
	// Get ground information as RaycastHit
	//

	public static bool GetGroundInfo( Vector3 startPos, out RaycastHit hit, Transform ignoreTransform )
	{
		return GetGroundInfo( startPos, out hit, 100f, ~0, ignoreTransform );
	}

	public static bool GetGroundInfo( Vector3 startPos, out RaycastHit hit, float range, Transform ignoreTransform )
	{
		return GetGroundInfo( startPos, out hit, range, ~0, ignoreTransform );
	}

	public static bool GetGroundInfo( Vector3 startPos, out RaycastHit hitOut, float range, LayerMask mask, Transform ignoreTransform )
	{
		Profiler.BeginSample( "TransformUtil.GetGroundInfo" );

		// Make sure the raycast doesn't clip through the ground
		startPos.y += 0.25f;
		range += 0.25f;
		hitOut = new RaycastHit();

		// Setup ray and let it do its magic
		Ray ray = new Ray( startPos, Vector3.down );
		var hits = Physics.RaycastAll( ray, range, mask );
		foreach ( var hit in hits )
		{
			if ( ignoreTransform != null && (hit.collider.transform == ignoreTransform || hit.collider.transform.IsChildOf( ignoreTransform )) )
				continue;

			hitOut = hit;
			Profiler.EndSample();
			return true;
		}

		Profiler.EndSample();
		return false;
	}

	//
	// Get ground position and normal
	//

	public static bool GetGroundInfo( Vector3 startPos, out Vector3 pos, out Vector3 normal, Transform ignoreTransform )
	{
		return GetGroundInfo( startPos, out pos, out normal, 100f, ~0, ignoreTransform );
	}

	public static bool GetGroundInfo( Vector3 startPos, out Vector3 pos, out Vector3 normal, float range, Transform ignoreTransform )
	{
		return GetGroundInfo( startPos, out pos, out normal, range, ~0, ignoreTransform );
	}

	public static bool GetGroundInfo( Vector3 startPos, out Vector3 pos, out Vector3 normal, float range, LayerMask mask, Transform ignoreTransform )
	{
		Profiler.BeginSample( "TransformUtil.GetGroundInfo (All)" );

		// Make sure the raycast doesn't clip through the ground
		startPos.y += 0.25f;
		range += 0.25f;

		// Setup ray and let it do its magic
		Ray ray = new Ray( startPos, Vector3.down );
		var hits = Physics.RaycastAll( ray, range, mask );
		foreach ( var hit in hits )
		{
			if ( ignoreTransform != null && ( hit.collider.transform == ignoreTransform || hit.collider.transform.IsChildOf( ignoreTransform ) ) )
				continue;

			pos = hit.point;
			normal = hit.normal;
			Profiler.EndSample();
			return true;
		}

		pos = startPos;
		normal = Vector3.up;
		Profiler.EndSample();
		return false;
	}

	//
	// Get ground position and normal, where ground has to be terrain
	//

	public static bool GetGroundInfoTerrainOnly( Vector3 startPos, out Vector3 pos, out Vector3 normal )
	{
		return GetGroundInfoTerrainOnly( startPos, out pos, out normal, 100f, ~0 );
	}

	public static bool GetGroundInfoTerrainOnly( Vector3 startPos, out Vector3 pos, out Vector3 normal, float range )
	{
		return GetGroundInfoTerrainOnly( startPos, out pos, out normal, range, ~0 );
	}

	public static bool GetGroundInfoTerrainOnly( Vector3 startPos, out Vector3 pos, out Vector3 normal, float range, LayerMask mask )
	{
		Profiler.BeginSample( "TransformUtil.GetGroundInfoTerrainOnly" );

		// Make sure the raycast doesn't clip through the ground
		startPos.y += 0.25f;
		range += 0.25f;

		// Setup ray and let it do its magic
		Ray ray = new Ray( startPos, Vector3.down );
		RaycastHit hit;
		if ( Physics.Raycast( ray, out hit, range, mask ) && hit.collider is TerrainCollider )
		{
			pos = hit.point;
			normal = hit.normal;
			Profiler.EndSample();
			return true;
		}

		pos = startPos;
		normal = Vector3.up;
		Profiler.EndSample();
		return false;
		
	}

	//
	// Similar to Quaternion.LookRotation(forward, up) if forward and up are equal or orthogonal
	// For two arbitrary vectors this method guarantees to maintain the up vector as-is
	//

	public static Quaternion LookRotationForcedUp( Vector3 forward, Vector3 up )
	{
		if ( forward == up )
		{
			return Quaternion.LookRotation( up );
		}

		// Calculate the right vector
		Vector3 right = Vector3.Cross( forward, up );

		// Recalculate forward vector
		forward = Vector3.Cross( up, right );

		// Logs an error if the new forward direction is zero
		return Quaternion.LookRotation( forward, up );
	}


	public static Transform[] GetRootObjects()
	{
		return GameObject.FindObjectsOfType<Transform>().Where( x => x.transform == x.transform.root ).ToArray();
	}
}
