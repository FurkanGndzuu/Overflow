using Typesense;

namespace SearchService.MessageHandlers
{
    public class AnswerCountUpdatedHandler(ITypesenseClient client)
    {
        public async Task HandleAsync(Contracts.UpdatedAnswerCount message, CancellationToken cancellationToken)
        {
            await client.UpdateDocument("questions", message.QuestionId, new
            {
                AnswerCount = message.NewAnswerCount
            });
            Console.WriteLine($"Updated answer count for question with id {message.QuestionId} to {message.NewAnswerCount}");
        }
    }
}
