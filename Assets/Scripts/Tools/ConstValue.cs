using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstValue {

	public const float MAX_REG = 80.0F;
	public const float MIN_REG = 5.0F;

	public const float MIN_J4ROTATE = -180.0F;
	public const float MAX_J4ROTATE = 180.0F;

	public const float MIN_J6ROTATE = -360.0F;
	public const float MAX_J6ROTATE = 360.0F;

	public const float ABS_J1ROTATE = 170.0f;

	public const float LIMIT_J4ROTATE = 20F;

//	public const Vector3 ORIPOS = new Vector3 (241.265f, 0.000f, 186.323f);
//	public const Vector3 ORIWPR = new Vector3 (-180.00f, -85.190f, 0.000f);
//
//	public const Vector3 MAXPOS = new Vector3 (349.473f, 0.000f, 250.229f);
//	public const Vector3 MAXWPR = new Vector3 (-180.000f, -85.000f, 0.000f);

	public static Vector3 ORIPOS = new Vector3 (0.0f, -50.0f, -15.0f);
	public static Vector3 ORIWPR = new Vector3 (0.0f, 10.0f, 0.0f);

	public static Vector3 MAXPOS = new Vector3 (0, -15.0f, -15.0f);
	public static Vector3 MAXWPR = new Vector3 (0.0f, 10.0f, 0.0f);

	public const float NORMAL_MOVE_SPEED = 450.0F;

	public const float MIN_MOVE_SPEED = 100.0F;
	public const float SPEED_FACTOR = 12.5f;

	public const float J1_MOVE_SPEED = 0.05F;

}
