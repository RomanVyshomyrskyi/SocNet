using System;

namespace My_SocNet_Win.Classes.DB;

public interface IDatabaseService
{
    void EnsureDatabaseCreated();
    void Connect();
    void Disconnect();
}
