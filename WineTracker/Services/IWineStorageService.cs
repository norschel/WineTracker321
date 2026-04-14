using System.Collections.Generic;
using WineTracker.Models;

namespace WineTracker.Services
{
    public interface IWineStorageService
    {
        IEnumerable<Wine> LoadAll();
        void Save(IEnumerable<Wine> wines);
    }
}
