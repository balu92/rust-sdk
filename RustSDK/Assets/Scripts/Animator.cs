using UnityEngine;
using System.Collections;

namespace UnityEngine
{
	public static class AnimatorEx
	{
		public static void SetLayersWeight( this Animator anim, string strNamePartial, float fWeight )
		{
			for ( int i=0; i < anim.layerCount; i++ )
			{
				string strName = anim.GetLayerName( i );
				if ( !strName.StartsWith( strNamePartial ) ) continue;

				anim.SetLayerWeight( i, fWeight );
			}
		}

		public static float GetLayersWeight( this Animator anim, string strNamePartial )
		{
			for ( int i=0; i < anim.layerCount; i++ )
			{
				string strName = anim.GetLayerName( i );
				if ( !strName.StartsWith( strNamePartial ) ) continue;

				return anim.GetLayerWeight( i );
			}
			return 0.0f;
		}
	}
}