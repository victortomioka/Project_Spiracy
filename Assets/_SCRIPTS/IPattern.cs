using UnityEngine;
using System.Collections;

public interface IPattern{

	void startPattern(Rigidbody[] subBullets,float spread,Transform muzzle,bool destroy);

	int getCount();

}
