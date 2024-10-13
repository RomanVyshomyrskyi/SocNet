using System;

namespace My_SocNet_Win.Classes.DB;

public interface IDatabaseService
{
    void Connect();
    void Disconnect();
}
