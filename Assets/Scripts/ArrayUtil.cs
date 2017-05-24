using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ArrayUtil
{
	public static void Swap<T>(this IList<T> arr, int a, int b)
	{
		T temp = arr[a];
		arr[a] = arr[b];
		arr[b] = temp;
	}


}
