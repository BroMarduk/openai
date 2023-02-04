﻿using System.IO;
using System.Threading.Tasks;
using OpenAI.GPT3.DotNetStandard.ObjectModels.RequestModels;
using OpenAI.GPT3.DotNetStandard.ObjectModels.ResponseModels.FineTuneResponseModels;

namespace OpenAI.GPT3.DotNetStandard.Interfaces
{
    /// <summary>
    ///     Manage fine-tuning jobs to tailor a model to your specific training data.
    ///     Related guide: <a href="https://beta.openai.com/docs/guides/fine-tuning">Fine-tune models</a>
    /// </summary>
    public interface IFineTuneService
    {
        /// <summary>
        ///     Creates a job that fine-tunes a specified model from a given dataset.
        ///     Response includes details of the enqueued job including job status and the name of the fine-tuned models once
        ///     complete.
        /// </summary>
        /// <param name="createFineTuneRequest"></param>
        /// <returns></returns>
        Task<FineTuneResponse> CreateFineTune(FineTuneCreateRequest createFineTuneRequest);

        /// <summary>
        ///     List your organization's fine-tuning jobs
        /// </summary>
        /// <returns></returns>
        Task<FineTuneListResponse> ListFineTunes();

        /// <summary>
        ///     Gets info about the fine-tune job.
        /// </summary>
        /// <param name="fineTuneId">The ID of the fine-tune job</param>
        /// <returns></returns>
        Task<FineTuneResponse> RetrieveFineTune(string fineTuneId);

        /// <summary>
        ///     Immediately cancel a fine-tune job.
        /// </summary>
        /// <param name="fineTuneId">The ID of the fine-tune job to cancel</param>
        /// <returns></returns>
        Task<FineTuneResponse> CancelFineTune(string fineTuneId);

        /// <summary>
        ///     Get fine-grained status updates for a fine-tune job.
        /// </summary>
        /// <param name="fineTuneId">The ID of the fine-tune job to get events for.</param>
        /// <param name="stream">
        ///     Whether to stream events for the fine-tune job. If set to true, events will be sent as data-only server-sent events
        ///     as they become available. The stream will terminate with a data: [DONE] message when the job is finished
        ///     (succeeded, cancelled, or failed).
        ///     If set to false, only events generated so far will be returned.
        /// </param>
        /// <returns></returns>
        Task<Stream> ListFineTuneEvents(string fineTuneId, bool? stream = null);

        Task DeleteFineTune(string fineTuneId);
    }
}