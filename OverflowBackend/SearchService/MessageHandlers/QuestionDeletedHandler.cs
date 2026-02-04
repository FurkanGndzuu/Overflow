using SearchService.Models;
using System.Net.Sockets;
using Typesense;

namespace SearchService.MessageHandlers
{
    public class QuestionDeletedHandler(ITypesenseClient client)
    {
        public async Task HandleAsync(Contracts.QuestionDeleted message, CancellationToken cancellationToken)
        {
            await client.DeleteDocument<SearchQuestion>("questions", message.QuestionId);
        }
    }
}
