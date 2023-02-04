﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using OpenAI.GPT3.DotNetStandard.Extensions;
using OpenAI.GPT3.DotNetStandard.Interfaces;
using OpenAI.GPT3.DotNetStandard.ObjectModels;
using OpenAI.GPT3.DotNetStandard.ObjectModels.RequestModels;
using OpenAI.GPT3.DotNetStandard.ObjectModels.ResponseModels;

namespace OpenAI.GPT3.DotNetStandard.Managers;


public partial class OpenAIService : ICompletionService
{
    /// <inheritdoc />
    public async Task<CompletionCreateResponse> CreateCompletion(CompletionCreateRequest createCompletionRequest, string? modelId = null)
    {
        createCompletionRequest.ProcessModelId(modelId, _defaultModelId);
        return await _httpClient.PostAndReadAsAsync<CompletionCreateResponse>(_endpointProvider.CompletionCreate(), createCompletionRequest);
    }
    
    /// <inheritdoc />
    public async IAsyncEnumerable<CompletionCreateResponse> CreateCompletionAsStream(CompletionCreateRequest createCompletionRequest, string? modelId = null)
    {
        // Mark the request as streaming
        createCompletionRequest.Stream = true;

        // Send the request to the CompletionCreate endpoint
        createCompletionRequest.ProcessModelId(modelId, _defaultModelId);

        using var response = _httpClient.PostAsStreamAsync(_endpointProvider.CompletionCreate(), createCompletionRequest);
        await using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);
        // Continuously read the stream until the end of it
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            // Skip empty lines
            if (string.IsNullOrEmpty(line)) continue;

            line = line.RemoveIfStartWith("data: ");

            // Exit the loop if the stream is done
            if (line.StartsWith("[DONE]")) break;

            CompletionCreateResponse? block;
            try
            {
                // When the response is good, each line is a serializable CompletionCreateRequest
                block = JsonSerializer.Deserialize<CompletionCreateResponse>(line);
            }
            catch (Exception)
            {
                // When the API returns an error, it does not come back as a block, it returns a single character of text ("{").
                // In this instance, read through the rest of the response, which should be a complete object to parse.
                line += await reader.ReadToEndAsync();
                block = JsonSerializer.Deserialize<CompletionCreateResponse>(line);
            }

            if (null != block) yield return block;
        }
    }

    public Task<CompletionCreateResponse> Create(CompletionCreateRequest createCompletionModel, Models.Model modelId)
    {
        return CreateCompletion(createCompletionModel, modelId.EnumToString());
    }
}