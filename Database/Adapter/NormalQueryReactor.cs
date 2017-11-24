using System;

using Cloud.Database.Interfaces;

namespace Cloud.Database.Adapter
{
    public class NormalQueryReactor : QueryAdapter, IQueryAdapter, IRegularQueryAdapter, IDisposable
    {
        public NormalQueryReactor(IDatabaseClient Client)
            : base(Client)
        {
            base.command = Client.createNewCommand();
        }

        public void Dispose()
        {
            base.command.Dispose();
            base.client.reportDone();
            GC.SuppressFinalize(this);
        }
    }
}