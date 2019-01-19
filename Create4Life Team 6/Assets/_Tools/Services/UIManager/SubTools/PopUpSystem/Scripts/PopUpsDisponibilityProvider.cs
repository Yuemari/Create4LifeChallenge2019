using UnityEngine;
using System.Collections;

public class PopUpsDisponibilityProvider : MonoBehaviour
{
	public virtual bool CanShowNonSpecialPopUps()
	{
		return true;
	}
}
