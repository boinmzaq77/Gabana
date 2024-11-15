using AutoMapper;
using Gabana.ORM.MerchantDB;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Gabana.ShareSource.Sync
{
    static public class NoteSync
    {
        static Gabana.ShareSource.Manage.NoteManage noteManage = new Manage.NoteManage();
        static public async Task SentNote(int merchantid, int SysnoteID)
        {
            Note note = new Note(); 
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                note = await noteManage.GetNote(merchantid, SysnoteID);
                note.LastDateModified = note.LastDateModified;
                note.WaitSendingTime = note.WaitSendingTime;

                if (note is null)
                {
                    return;
                }
                if (note.FWaitSending == 0)
                {
                    return;
                }

                if (note.DataStatus == 'N')
                {
                    return;
                }
                switch (note.DataStatus)
                {
                    case 'I':
                        InsertNote(note);
                        break;
                    case 'M':
                        UpdateNote(note);
                        break;
                    case 'D':
                        DeleteNote(note);
                        break;
                    default:
                        break;
                }
            }
            catch (WebException ex)
            {
                note = await noteManage.GetNote(merchantid, SysnoteID);
                note.FWaitSending = 2;
                await noteManage.UpdateNote(note);
            }
        }

        static public async Task SentNoteAndroid(int merchantid, int SysnoteID)
        {
            Note note = new Note();
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                note = await noteManage.GetNoteAndroid(merchantid, SysnoteID);
                note.LastDateModified = note.LastDateModified;
                note.WaitSendingTime = note.WaitSendingTime;

                if (note is null)
                {
                    return;
                }
                if (note.FWaitSending == 0)
                {
                    return;
                }

                if (note.DataStatus == 'N')
                {
                    return;
                }
                switch (note.DataStatus)
                {
                    case 'I':
                        InsertNote(note);
                        break;
                    case 'M':
                        UpdateNote(note);
                        break;
                    case 'D':
                        DeleteNote(note);
                        break;
                    default:
                        break;
                }
            }
            catch (WebException ex)
            {
                note = await noteManage.GetNote(merchantid, SysnoteID);
                note.FWaitSending = 2;
                await noteManage.UpdateNote(note);
            }
        }

        private async static void DeleteNote(Note note)
        {
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            try
            {
                var result = await GabanaAPI.DeleteDataNotes((int)note.SysNoteID, DataCashingAll.DeviceNo); //return UpdateLastRevisionNo
                if (result.Status)
                {
                    note.FWaitSending = 0;

                    //Delete Item ที่ Local
                    var deleteItem = await noteManage.DeleteNote((int)note.MerchantID, (int)note.SysNoteID);
                    if (!deleteItem)
                    {
                        note.FWaitSending = 2;
                        await noteManage.UpdateNote(note);
                    }
                }
                else
                {
                    note.FWaitSending = 2;
                }
                await noteManage.UpdateNote(note);                
            }
            catch (WebException ex)
            {
                note = await noteManage.GetNote((int)note.MerchantID,(int)note.SysNoteID);
                note.FWaitSending = 2;
                await noteManage.UpdateNote(note);
            }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
        }

        private async static void UpdateNote(Note note)
        {
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            try
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ORM.MerchantDB.Note, ORM.Master.Note>();
                });

                var Imapper = config.CreateMapper();
                var JAMNote = Imapper.Map<ORM.MerchantDB.Note, ORM.Master.Note>(note);


                var NoteWithNoteStatus = new Gabana3.JAM.Notes.NoteWithNoteStatus() {
                    DeviceNo = DataCashingAll.DeviceNo,
                    note = JAMNote,
                    DataStatus = 'M'
                };

                var result = await GabanaAPI.PutDataNotes(NoteWithNoteStatus);
                if (result.Status)
                {
                    note.FWaitSending = 0;
                }
                else
                {
                    note.FWaitSending = 2;
                }
                await noteManage.UpdateNote(note);
            }
            catch (WebException ex)
            {
                note = await noteManage.GetNote((int)note.MerchantID,(int)note.SysNoteID);
                note.FWaitSending = 2;
                await noteManage.UpdateNote(note);
            }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
        }

        private async static void InsertNote(Note note)
        {
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            try
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ORM.MerchantDB.Note, ORM.Master.Note>();
                });

                var Imapper = config.CreateMapper();
                var JAMNote = Imapper.Map<ORM.MerchantDB.Note, ORM.Master.Note>(note);

                var  NoteWithNoteStatus = new Gabana3.JAM.Notes.NoteWithNoteStatus()
                {
                    DeviceNo = DataCashingAll.DeviceNo,
                    note = JAMNote,
                    DataStatus = 'I'
                };

                var result = await GabanaAPI.PostDataNotes(NoteWithNoteStatus);
                if (result.Status)
                {
                    note.FWaitSending = 0;
                }
                else
                {
                    note.FWaitSending = 2;
                }
                await noteManage.UpdateNote(note);
            }
            catch (WebException ex)
            {
                note = await noteManage.GetNote((int)note.MerchantID, (int)note.SysNoteID);
                note.FWaitSending = 2;
                await noteManage.UpdateNote(note);
            }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
        }
    }
   
}
