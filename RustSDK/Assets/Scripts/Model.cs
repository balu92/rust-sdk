using UnityEngine;
using System.Collections;
using System.Linq;

public class Model : MonoBehaviour 
{
	public Transform rootBone;
	public Transform headBone;

	internal Transform[] bones;

	void Start () 
	{
		if ( rootBone == null ) 
			rootBone = transform;
	}
	
	public Transform[] GetBones()
	{
		if ( bones != null ) return bones;

		bones = rootBone.GetAllChildren().ToArray();
		return bones;
	}

	public Transform FindBone( string name )
	{
		var bones = GetBones();
		var bone = bones.FirstOrDefault( x => x.name.Equals( name, System.StringComparison.InvariantCultureIgnoreCase ) );
		if ( bone ) return bone;

		return rootBone;
	}

#if SERVER
	public void PoolTransformStrings()
	{
		rootBone.PoolTransformStrings();
	}
#endif
}
