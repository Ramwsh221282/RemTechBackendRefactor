using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace RemTech.SharedKernel.NN;

internal readonly ref struct EmbeddingData
{
	public int Length => Tokens.Length;
	private ReadOnlySpan<int> Tokens { get; init; }
	private ReadOnlySpan<int> Indices { get; init; }

	public static EmbeddingData Create(NamedOnnxValue[] tokenizerInputs, InferenceSession tokenizerSession)
	{
		using IDisposableReadOnlyCollection<DisposableNamedOnnxValue>? tokenizerResults = tokenizerSession.Run(
			tokenizerInputs
		);
		Tensor<int>? tokensTensor = tokenizerResults[0].AsTensor<int>();
		Tensor<int>? indicesTensor = tokenizerResults[2].AsTensor<int>();
		int[] tokens = [.. tokensTensor];
		int[] indices = [.. indicesTensor];

		return new EmbeddingData { Tokens = tokens, Indices = indices };
	}

	public void CopySortedTokensTo(Span<int> destination)
	{
		if (destination.Length < Tokens.Length)
			throw new ArgumentException("Destination span is too small", nameof(destination));

		int len = Tokens.Length;
		if (len == 0)
			return;

		Span<int> pos = len <= 128 ? stackalloc int[len] : new int[len];

		for (int i = 0; i < len; i++)
			pos[i] = i;

		QuickSort(pos, Indices);
		for (int i = 0; i < len; i++)
			destination[i] = Tokens[pos[i]];
	}

	public ReadOnlySpan<int> TokenPairs()
	{
		(int token, int index)[] pairs = new (int token, int index)[Tokens.Length];
		for (int i = 0; i < Tokens.Length; i++)
		{
			pairs[i] = (Tokens[i], Indices[i]);
		}

		Array.Sort(pairs, (a, b) => a.index.CompareTo(b.index));
		int[] result = new int[pairs.Length];
		for (int i = 0; i < pairs.Length; i++)
		{
			result[i] = pairs[i].token;
		}
		return result;
	}

	private static void QuickSort(Span<int> idx, ReadOnlySpan<int> indices)
	{
		if (idx.Length <= 1)
			return;

		int left = 0;
		int right = idx.Length - 1;
		int pivot = indices[idx[idx.Length / 2]];

		while (left <= right)
		{
			while (indices[idx[left]] < pivot)
				left++;
			while (indices[idx[right]] > pivot)
				right--;

			if (left <= right)
			{
				(idx[left], idx[right]) = (idx[right], idx[left]);
				left++;
				right--;
			}
		}

		if (right > 0)
			QuickSort(idx[..(right + 1)], indices);
		if (left < idx.Length - 1)
			QuickSort(idx[left..], indices);
	}
}
