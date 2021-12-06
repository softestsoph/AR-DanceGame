using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PoseTeacher
{

	public class TobitKalman
	{
		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		public const float DEFAULT_Q = 0.002f;
		public const float DEFAULT_R = 0.01f;

		public const float DEFAULT_P = 1;

		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		private float q;
		private float r;
		private float p = DEFAULT_P;
		private Vector3 x;
		private float k;

		//-----------------------------------------------------------------------------------------
		// Constructors:
		//-----------------------------------------------------------------------------------------

		// N.B. passing in DEFAULT_Q is necessary, even though we have the same value (as an optional parameter), because this
		// defines a parameterless constructor, allowing us to be new()'d in generics contexts.
		public TobitKalman() : this(DEFAULT_Q) { }

		public TobitKalman(float aQ = DEFAULT_Q, float aR = DEFAULT_R)
		{
			q = aQ;
			r = aR;
		}

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		public Vector3 Update(Vector3 measurement, float? newQ = null, float? newR = null)
		{

			// update values if supplied.
			if (newQ != null && q != newQ)
			{
				q = (float)newQ;
			}
			if (newR != null && r != newR)
			{
				r = (float)newR;
			}

			// update measurement.
			{
				k = (p + q) / (p + q + r);
				p = r * (p + q) / (r + p + q);
			}

			// filter result back into calculation.
			Vector3 res = x + (measurement - x) * k;

			Vector3 Tmax = x + new Vector3(0.31f, 0.18f, 0.31f);
			Vector3 Tmin = x - new Vector3(0.31f, 0.18f, 0.31f);

			Vector3 result = new Vector3(Mathf.Clamp(res.x, Tmin.x, Tmax.x), Mathf.Clamp(res.y, Tmin.y, Tmax.y), Mathf.Clamp(res.z, Tmin.z, Tmax.z));

			x = result;
			return result;
		}

		public Vector3 Update(List<Vector3> measurements, bool areMeasurementsNewestFirst = false, float? newQ = null, float? newR = null)
		{

			Vector3 result = Vector3.zero;
			int i = (areMeasurementsNewestFirst) ? measurements.Count - 1 : 0;

			while (i < measurements.Count && i >= 0)
			{

				// decrement or increment the counter.
				if (areMeasurementsNewestFirst)
				{
					--i;
				}
				else
				{
					++i;
				}

				result = Update(measurements[i], newQ, newR);
			}

			return result;
		}

		public void Reset()
		{
			p = 1;
			x = Vector3.zero;
			k = 0;
		}
	}

	public class TobitKalmanKinect
	{
		List<TobitKalman> filters = new List<TobitKalman>();

		public void update(PoseData pose)
        {
			for (int i = 0; i < pose.data.Length; i++)
            {
				if (filters.Count <= i)
                {
					filters.Add(new TobitKalman());
                }
				pose.data[i].Position = filters[i].Update(pose.data[i].Position);
            }
        }
		public void reset()
        {
			filters = new List<TobitKalman>();
        }
	}
}