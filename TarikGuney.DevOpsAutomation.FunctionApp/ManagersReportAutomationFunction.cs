using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using TarikGuney.DevOpsAutomation.DataFlow;
using TarikGuney.DevOpsAutomation.SettingsModels;

namespace TarikGuney.DevOpsAutomation
{
	public static class ManagersReportAutomationFunction
	{
		[FunctionName("ManagersReportAutomation")]
		public static async Task RunAsync([TimerTrigger("0 30 9 * * 1-5")] TimerInfo myTimer,
			ILogger log, ExecutionContext context)
		{
			// Sets the settings model defined as static properties in the class.
			await Config.SetSharedSettings(context);

			var iterationWorkItemsTransformBlock = IterationWorkItemsRetrieverTransform.Block;
			var managersGoogleMessageSenderActionBlock = ManagersGoogleChatMessageSenderAction.Block;

			var passedDueWorkItemsTransformBlock = PassedDueWorkItemsTransform.Block;

			var broadcastBlock = new BroadcastBlock<List<JObject>>(null);
			iterationWorkItemsTransformBlock.LinkTo(broadcastBlock);

			var batchBlock = new BatchBlock<string>(1);

			broadcastBlock.LinkTo(passedDueWorkItemsTransformBlock);
			passedDueWorkItemsTransformBlock.LinkTo(batchBlock);

			batchBlock.LinkTo(managersGoogleMessageSenderActionBlock);

			iterationWorkItemsTransformBlock.Post(IterationTimeFrame.Current);
			iterationWorkItemsTransformBlock.Complete();
			await managersGoogleMessageSenderActionBlock.Completion;
		}
	}
}
