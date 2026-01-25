using Microsoft.Extensions.Options;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using RemTech.SharedKernel.Configurations;

namespace RemTech.SharedKernel.NN;

public sealed class EmbeddingsProvider(IOptions<EmbeddingsProviderOptions> options)
{
	private Lazy<InferenceSession> TokenizerSessionLazy { get; } = new(MakeTokenizerSession(options.Value));
	private Lazy<InferenceSession> ModelSessionLazy { get; } = new(MakeModelSession(options.Value));

	private InferenceSession Tokenizer => TokenizerSessionLazy.Value;
	private InferenceSession Model => ModelSessionLazy.Value;
	private bool Disposed { get; set; }

	public ReadOnlyMemory<float> Generate(string text)
	{
		DenseTensor<string> stringTensor = new([1]);
		stringTensor[0] = text;

		NamedOnnxValue[] tokenizerInputs = [NamedOnnxValue.CreateFromTensor("inputs", stringTensor)];
		EmbeddingData embeddingData = EmbeddingData.Create(tokenizerInputs, Tokenizer);

		int len = embeddingData.Length;
		DenseTensor<long> inputIdsTensor = new([1, len]);
		DenseTensor<long> attentionMaskTensor = new([1, len]);
		Span<int> sortedTokens = len <= 256 ? stackalloc int[len] : new int[len];
		embeddingData.CopySortedTokensTo(sortedTokens);

		for (int i = 0; i < len; i++)
		{
			inputIdsTensor[0, i] = sortedTokens[i];
			attentionMaskTensor[0, i] = 1;
		}

		NamedOnnxValue[] modelInputs =
		[
			NamedOnnxValue.CreateFromTensor("input_ids", inputIdsTensor),
			NamedOnnxValue.CreateFromTensor("attention_mask", attentionMaskTensor),
		];

		return GetEmbeddings(modelInputs, Model);
	}

	public void Dispose()
	{
		Dispose(true);
		// GC.SuppressFinalize(this);
	}

	public IReadOnlyList<ReadOnlyMemory<float>> GenerateBatch(IReadOnlyList<string> texts)
	{
		ArgumentNullException.ThrowIfNull(texts);
		if (texts.Count == 0)
			return [];

		int batchSize = texts.Count;
		int[][] tokenArrays = new int[batchSize][];
		int[] lengths = new int[batchSize];
		int maxLen = 0;

		for (int i = 0; i < batchSize; i++)
		{
			EmbeddingData embeddingData = TokenizeSingle(texts[i]);

			int len = embeddingData.Length;
			lengths[i] = len;
			if (len > maxLen)
				maxLen = len;

			int[] sortedTokens = new int[len];
			embeddingData.CopySortedTokensTo(sortedTokens);
			tokenArrays[i] = sortedTokens;
		}

		DenseTensor<long> inputIdsTensor = new([batchSize, maxLen]);
		DenseTensor<long> attentionMaskTensor = new([batchSize, maxLen]);

		const long padTokenId = 0;
		for (int b = 0; b < batchSize; b++)
		{
			int len = lengths[b];
			int[] tokens = tokenArrays[b];

			for (int i = 0; i < len; i++)
			{
				inputIdsTensor[b, i] = tokens[i];
				attentionMaskTensor[b, i] = 1;
			}

			for (int i = len; i < maxLen; i++)
			{
				inputIdsTensor[b, i] = padTokenId;
				attentionMaskTensor[b, i] = 0;
			}
		}

		NamedOnnxValue[] modelInputs =
		[
			NamedOnnxValue.CreateFromTensor("input_ids", inputIdsTensor),
			NamedOnnxValue.CreateFromTensor("attention_mask", attentionMaskTensor),
		];

		using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> modelResults = Model.Run(modelInputs);
		Tensor<float> outputTensor = modelResults[1].AsTensor<float>();

		if (outputTensor.Dimensions.Length != 2)
		{
			throw new NotSupportedException(
				$"Expected 2D output [batch, hiddenDim], got rank {outputTensor.Dimensions.Length}"
			);
		}

		int outBatch = outputTensor.Dimensions[0];
		int hiddenDim = outputTensor.Dimensions[1];

		if (outBatch != batchSize)
			throw new InvalidOperationException($"Model output batch size {outBatch} != input batch size {batchSize}");

		ReadOnlyMemory<float>[] result = new ReadOnlyMemory<float>[batchSize];

		for (int b = 0; b < batchSize; b++)
		{
			float[] arr = new float[hiddenDim];
			for (int h = 0; h < hiddenDim; h++)
				arr[h] = outputTensor[b, h];

			result[b] = arr;
		}

		return result;
	}

	private EmbeddingData TokenizeSingle(string text)
	{
		DenseTensor<string> stringTensor = new([1]);
		stringTensor[0] = text;
		NamedOnnxValue[] tokenizerInputs = [NamedOnnxValue.CreateFromTensor("inputs", stringTensor)];
		return EmbeddingData.Create(tokenizerInputs, Tokenizer);
	}

	private static ReadOnlyMemory<float> GetEmbeddings(
		IReadOnlyList<NamedOnnxValue> modelInputs,
		InferenceSession modelSession
	)
	{
		using IDisposableReadOnlyCollection<DisposableNamedOnnxValue>? modelResults = modelSession.Run(modelInputs);
		Tensor<float>? tensor = modelResults[1].AsTensor<float>();
		return new ReadOnlyMemory<float>([.. tensor]);
	}

	private static ReadOnlyMemory<float> GetEmbeddings(List<NamedOnnxValue> modelInputs, InferenceSession modelSession)
	{
		using IDisposableReadOnlyCollection<DisposableNamedOnnxValue>? modelResults = modelSession.Run(modelInputs);
		return modelResults[1].AsTensor<float>().ToArray();
	}

	private void Dispose(bool disposing)
	{
		if (!Disposed)
		{
			if (disposing)
			{
				TokenizerSessionLazy.Value.Dispose();
				ModelSessionLazy.Value.Dispose();
			}
			Disposed = true;
		}
	}

	~EmbeddingsProvider()
	{
		Dispose(false);
	}

	private static InferenceSession MakeTokenizerSession(EmbeddingsProviderOptions options)
	{
		options.Validate();
		SessionOptions tokenizerOptions = new();
		tokenizerOptions.RegisterOrtExtensions();
		return new InferenceSession(options.TokenizerPath, tokenizerOptions);
	}

	private static InferenceSession MakeModelSession(EmbeddingsProviderOptions options)
	{
		options.Validate();
		SessionOptions modelOptions = new() { GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL };
		return new InferenceSession(options.ModelPath, modelOptions);
	}
}
