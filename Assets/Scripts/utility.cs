using System.Collections;

public static class utility {
	
	/* This function performs a Fisher-Yates shuffle on the obstacle array
	 * The array a of n elements (indices 0..n-1) is shuffled:
	 * for i from n-1 downto 1 do
	 * 		j (random int such that 0 <= j <= i)
	 * 		swap a[j] and a[i]
	 * 
	 */

	public static T[] fisherYatesShuffle<T>(T[] array, int seed) {
		System.Random prng = new System.Random (seed);

		for (int i =0; i < array.Length -1; i ++) {
			int randomIndex = prng.Next(i,array.Length);
			T tempItem = array[randomIndex];
			array[randomIndex] = array[i];
			array[i] = tempItem;
		}

		return array;
	}

}
