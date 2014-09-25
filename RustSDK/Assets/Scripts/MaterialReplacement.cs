using UnityEngine;
using System.Collections;
using System.Linq;


public class MaterialReplacement : MonoBehaviour
{

	public static void ReplaceRecursive( GameObject go, Material mat )
	{
		var renderer = go.transform.GetComponentsInChildren<Renderer>( true );
		foreach( var r in renderer )
		{
			if ( r is ParticleRenderer ) continue;
			if ( r is ParticleSystemRenderer ) continue;

			var matreplace = r.gameObject.AddComponent<MaterialReplacement>();
			matreplace.Replace( mat );
		}
	}

	public static void Reset( GameObject go )
	{
		var replacement = go.GetComponentsInChildren<MaterialReplacement>( true );
		foreach ( var r in replacement )
		{
			GameObject.Destroy( r );
		}
	}

	public Material replacement;
	protected Material previous;

	void OnEnable()
	{
		if ( replacement )
			Replace( replacement );
	}

	void Replace( Material mat )
	{
		replacement = mat;

		var r = GetComponent<Renderer>();
		if ( r )
		{
			previous = r.sharedMaterial;
			r.sharedMaterial = replacement;
		}
	}

	void OnDisable()
	{
		var r = GetComponent<Renderer>();
		if ( r )
		{
			r.sharedMaterial = previous;
		}
	}

}
