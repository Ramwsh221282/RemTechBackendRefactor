using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace RemTech.Ner.VehicleParameters;

internal sealed record InputWords(IReadOnlyList<string> Words)
{
    private const int MaxLength = 512;
    
    public WordsTokenization Tokenize(VehicleNerModelMetadata metadata)
    {
        List<string> tokens = new() { "[CLS]" };
        List<int?> wordIds = new() { null };
        
        for (int wordIdx = 0; wordIdx < Words.Count; wordIdx++)
        {
            string[] pieceTokens = WordPieceTokenize(Words[wordIdx], metadata);
            if (pieceTokens.Length == 0) pieceTokens = ["[UNK]"];
            tokens.AddRange(pieceTokens);
            wordIds.AddRange(Enumerable.Repeat<int?>(wordIdx, pieceTokens.Length));
        }

        if (tokens.Count > MaxLength - 1)
        {
            tokens.RemoveRange(MaxLength - 1, tokens.Count - (MaxLength - 1));
            wordIds.RemoveRange(MaxLength - 1, wordIds.Count - (MaxLength - 1));
        }
        
        tokens.Add("[SEP]");
        wordIds.Add(null);
        
        int padLength = MaxLength - tokens.Count;
        tokens.AddRange(Enumerable.Repeat("[PAD]", padLength));
        wordIds.AddRange(Enumerable.Repeat<int?>(null, padLength));

        long[] inputIds = new long[tokens.Count];
        long[] attentionMask = new long[tokens.Count];
        for (int i = 0; i < tokens.Count; i++)
        {
            attentionMask[i] = tokens[i] == "[PAD]" ? 0L : 1L;
            inputIds[i] = metadata.Vocab.GetValueOrDefault(tokens[i], metadata.Vocab["[UNK]"]);
        }
        
        int?[] wordIdsArr = new int?[wordIds.Count];
        for (int i = 0; i < wordIds.Count; i++)
        {
            wordIdsArr[i] = wordIds[i];
        }
        
        Memory<int?> wordIdsSpan = new(wordIdsArr);
        DenseTensor<long> inputIdsTensor = new(inputIds, [1, MaxLength]);
        DenseTensor<long> attentionMaskTensor = new(attentionMask, [1, MaxLength]);
        
        IReadOnlyCollection<NamedOnnxValue> inputs =
        [
            NamedOnnxValue.CreateFromTensor("input_ids", inputIdsTensor),
            NamedOnnxValue.CreateFromTensor("attention_mask", attentionMaskTensor),
        ];
        
        return new WordsTokenization(inputs, wordIdsSpan);
    }

    private string[] WordPieceTokenize(string word, VehicleNerModelMetadata metadata)
    {
        IReadOnlyDictionary<string, int> vocab = metadata.Vocab;
        if (vocab.ContainsKey(word)) return [word];

        List<string> tokens = [];
        int start = 0;
        while (start < word.Length)
        {
            int end = word.Length;
            string? token = null;
            while (end > start)
            {
                string subStr = word.Substring(start, end - start);
                if (start > 0) subStr = "##" + subStr;
                if (vocab.ContainsKey(subStr))
                {
                    token = subStr;
                    break;
                }

                end--;
            }

            if (token == null) return [];
            tokens.Add(token);
            start = end;
        }
        
        return tokens.ToArray();
    }
}