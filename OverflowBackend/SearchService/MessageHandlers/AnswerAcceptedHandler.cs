using Typesense;

namespace SearchService.MessageHandlers
{
    public class AnswerAcceptedHandler(ITypesenseClient client)
    {
        public async Task HandleAsync(Contracts.AnswerAccepted message, CancellationToken cancellationToken)
        {
            await client.UpdateDocument("questions", message.QuestionId, new
            {
                HasAcceptedAnswer = true
            });
            Console.WriteLine($"Marked question with id {message.QuestionId} as having an accepted answer");
        }
    }
}
