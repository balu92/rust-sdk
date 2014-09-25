using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public static class GameManager
{
	public static Dictionary<string, GameObject> prefabOverride = new Dictionary<string, GameObject>();

	//
	// Find a prefab by name, return its object
	//
	public static GameObject FindPrefab( string strPrefab )
	{
		Profiler.BeginSample( "FindPrefab" );

		//
		// Try to get an over-ridden version
		//
		GameObject obj;
		if ( prefabOverride.TryGetValue( strPrefab, out obj ) )
		{
			Profiler.EndSample();
			return obj;
		}

		Profiler.BeginSample( "Resources.Load/Prefabs/" );
		obj = Resources.Load<GameObject>( "Prefabs/" + strPrefab );
		if ( obj == null )
		{
			Profiler.EndSample();
			Profiler.EndSample();
			return null;
		}
		Profiler.EndSample();

		Profiler.EndSample();

		return obj;
	}

	//
	// Find all prefabs in a folder, return their objects
	//
	public static GameObject[] FindPrefabs( string strPrefab, bool useProbabilities = false )
	{
		//
		// NOTE: Because of processing, we want FindPrefab to be the single
		//		 access point to get an actual prefab GameObject.
		//

		var prefabs = FindPrefabNames( strPrefab, useProbabilities );
		var list = new List<GameObject>( prefabs.Length );

		foreach ( var pf in prefabs )
		{
			list.Add( FindPrefab( pf ) );
		}

		return list.ToArray();
	}

	//
	// Find all prefabs in a folder, return their names
	// Only returns the correct result if strPrefab is a folder
	//
	public static string[] FindPrefabNames( string strPrefab, bool useProbabilities = false )
	{
		var prefabObjects = Resources.LoadAll<GameObject>( "Prefabs/" + strPrefab );
		var prefabNames   = new List<string>( prefabObjects.Length );

		foreach ( var prefabObject in prefabObjects )
		{
			var prefabName = strPrefab + "/" + prefabObject.name;

			if ( !useProbabilities )
			{
				prefabNames.Add( prefabName );
			}
			else
			{
				var prefabCount = 1;

				for ( int i = 0; i < prefabCount; i++ )
				{
					prefabNames.Add( prefabName );
				}
			}
		}

		return prefabNames.ToArray();
	}

	//
	// Create a GameObject from a prefab name
	//
	public static GameObject CreatePrefab( string strPrefab, Vector3 pos = new Vector3(), Quaternion rot = new Quaternion(), bool active = true )
	{
		Profiler.BeginSample( "CreatePrefab" );

		var prefab = FindPrefab( strPrefab );
		if ( !prefab  )
		{
			Debug.LogWarning( "Couldn't find prefab \"" + strPrefab + "\"" );
			Profiler.EndSample();
			return null;
		}

		#if SERVER
		StringPool.Server.Add( strPrefab );
		#endif

		var pf = CreatePrefab( prefab, pos, rot, active );

		Profiler.EndSample();
		return pf;
	}

	//
	// Create a GameObject from a prefab
	//
	public static GameObject CreatePrefab( GameObject prefab, Vector3 pos = new Vector3(), Quaternion rot = new Quaternion(), bool active = true )
	{
		var go = GameObject.Instantiate( prefab, pos, rot ) as GameObject;
		go.SetActive( active );
		return go;
	}

}
