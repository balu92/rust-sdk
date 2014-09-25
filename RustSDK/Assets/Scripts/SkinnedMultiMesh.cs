using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


//
// Generate multiple skinned meshes from one animation and one set of bones
//

public class SkinnedMultiMesh : MonoBehaviour 
{
	public Material materialOverride;

	public List<GameObject> parts;
	public Dictionary<string, Transform> boneDict = new Dictionary<string, Transform>();

	[System.NonSerialized]
	public List<GameObject> createdParts = new List<GameObject>();

	void Start()
	{
		RebuildModel();
	}

	//
	// Generates a fast bone-by-name lookup for us
	//
	public void BuildBoneDictionary( Transform t )
	{
		if ( t != transform && !boneDict.ContainsKey( t.name )  )
		{
			boneDict.Add( t.name, t );
		}

		for ( int i =0; i < t.childCount; i++ )
		{
			BuildBoneDictionary( t.GetChild( i ) );
		}
	}

	public void Clear()
	{
		foreach ( var p in createdParts )
		{
			Destroy( p.gameObject );
		}

		createdParts.Clear();
	}

	//
	// Call this after changing the parts
	//
	public void RebuildModel()
	{
		if ( boneDict.Count == 0 )
		{
			BuildBoneDictionary( transform );
		}

		Clear();

		foreach( var gop in parts )
		{
			if ( gop == null ) continue;

			var go = gameObject.InstantiateChild( gop );
			go.SetLayerRecursive( gameObject.layer );

			var renderers = go.GetComponentsInChildren<SkinnedMeshRenderer>();
			foreach ( var renderer in renderers )
			{
				var oldTransformRoot = renderer.rootBone;

				// Find the top of the skeleton
				while ( oldTransformRoot.parent != null && oldTransformRoot.parent != go.transform )
					oldTransformRoot = oldTransformRoot.parent;

				// Copy the target renderer
				{
					RemapBones( renderer );
				}

				// Dispose of the old root bone
				{
					GameObject.Destroy( oldTransformRoot.gameObject );
				}

				// We might be over-riding the material, do it here
				{
					DoMaterialOverride( renderer );
				}

				#if CLIENT
				if ( materialOverride == null )
					AmplifyMotionEffect.RegisterS( renderer.gameObject );
				#endif
			}

			go.SetActive( IsVisible );
			createdParts.Add( go );			
		}

		CreateLODGroup();

		// HACK: Update the 'BasedOnRenderers' animator if we have one, so it finds the new renderers (!)
		{
			var animator = GetComponent<Animator>();
			if ( animator != null && animator.cullingMode == AnimatorCullingMode.BasedOnRenderers )
			{
				animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
				animator.cullingMode = AnimatorCullingMode.BasedOnRenderers;
			}
		}
	}

	//
	// Remap Bones from the old renderer to the new one
	// This is just a case of finding the transforms on this object by name
	//
	void RemapBones( SkinnedMeshRenderer renderer )
	{
		var numBones = renderer.bones.Length;
		var bones = renderer.bones;

		for ( int i=0; i < numBones; i++ )
		{
			bones[i] = FindBone( renderer.bones[i].name );
		}

		renderer.bones = bones;
		renderer.rootBone = FindBone( renderer.rootBone.name );
	}

	//
	// Find a bone by name
	//
	public Transform FindBone( string strName )
	{
		Transform t;
		if ( boneDict.TryGetValue( strName, out t  ) )
			return t;

		Debug.LogWarning( "Couldn't find bone:" + strName, this );

		return transform;
	}

	public Transform[] GetBones()
	{
		var bones = new Transform[boneDict.Count];
		int i = 0;
		foreach( var b in boneDict )
		{
			bones[i] = b.Value;
			i++;
		}

		return bones;
	}

	internal void DoMaterialOverride( SkinnedMeshRenderer renderer )
	{
		if ( materialOverride == null ) return;

		var mats = renderer.sharedMaterials;

		for ( int i=0; i < mats.Length; i++ )
		{
			mats[i] = materialOverride;
		}

		renderer.sharedMaterials = mats;
	}


	internal bool IsVisible = true;

	public void SetVisible( bool bVisible )
	{
		if ( bVisible == IsVisible ) return;

		foreach ( var p in createdParts )
		{
			p.SetActive( IsVisible );
		}
	}

	public void CreateLODGroup()
	{
		var lodGroup = gameObject.GetComponent<LODGroup>();
		if ( lodGroup == null ) lodGroup = gameObject.AddComponent<LODGroup>();

		//
		// Kill all old LODGroups
		//
		{
			var emptyLOD = new LOD[1];
			foreach ( var lod in GetComponentsInChildren<LODGroup>() )
			{
				if ( lodGroup == lod ) continue;

				lod.SetLODS( emptyLOD );
				GameObject.Destroy( lod );
			}
		}

		var renderers = GetComponentsInChildren<Renderer>();

		var lods = new LOD[4];
		for ( int i=0; i < 4; i++ )
		{
			lods[i].renderers = renderers.Where( x => x.name.EndsWith( "_LOD" + i ) || x.name.EndsWith( "_LOD0" + i ) ).ToArray();
		}

		lods[0].screenRelativeTransitionHeight = 0.50f;
		lods[1].screenRelativeTransitionHeight = 0.25f;
		lods[2].screenRelativeTransitionHeight = 0.10f;
		lods[3].screenRelativeTransitionHeight = 0.02f;

		lodGroup.SetLODS( lods );
	}
	
}
