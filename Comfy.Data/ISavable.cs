
namespace Comfy.Data
{
    public interface ISavable
    {
        void Save(System.Data.Common.DbTransaction tran);
    }
}
