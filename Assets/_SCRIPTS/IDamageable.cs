using UnityEngine;
using System.Collections;

public interface IDamageable{

	void DealDamage (int dmg);

	void HealDamage (int heal);

	void FlamesStart (int fdmg, float flamesTime);

}
