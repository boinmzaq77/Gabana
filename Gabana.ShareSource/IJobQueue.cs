using System;
using System.Collections.Generic;
using System.Text;

namespace Gabana
{
    public interface IJobQueue
    {
        void AddJobSendItem(int merchantid, int SysItemID);
        void AddJobSendCatagory(int merchantid, int SysCategoryID);
        void AddJobSendTrans(int merchantid, int SysBranchID, string transNo);
        void AddJobSendCustomer(int merchantid, int SysCustomerID);
        void AddJobSendNoteCatagory(int merchantid, int SysNoteCatagoryID);
        void AddJobSendNote(int merchantid, int SysNoteID);
    }
}
