namespace SimpleVR.Utils
{
	public static class ArrayExt
{
	public static int FindMinIndex(this int[] array)
	{
		int idx = 0;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] < array[idx]) { idx = i; }
		}
		return idx;
	}

	public static int FindMinIndex(this float[] array)
	{
		int idx = 0;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] < array[idx]) { idx = i; }
		}
		return idx;
	}

	public static int FindMaxIndex(this int[] array)
	{
		int idx = 0;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] > array[idx]) { idx = i; }
		}
		return idx;
	}

	public static int FindMaxIndex(this float[] array)
	{
		int idx = 0;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] > array[idx]) { idx = i; }
		}
		return idx;
	}
}
}
