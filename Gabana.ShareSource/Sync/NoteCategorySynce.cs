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

    static public class NoteCategorySynce
    {
        static Gabana.ShareSource.Manage.NoteCategoryManage NoteCategoryManage = new Manage.NoteCategoryManage();
        static Gabana.ShareSource.Manage.NoteManage noteManage = new Manage.NoteManage();       

        static public async Task SentNoteCategory(int merchantid, int SysNoteCategoryID)
        {
            NoteCategory noteCategory = new NoteCategory(); 
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                noteCategory = await NoteCategoryManage.GetNoteCategory(merchantid, SysNoteCategoryID);
                if (noteCategory is null)
                {
                    return;
                }
                noteCategory.DateCreated = noteCategory.DateCreated;
                noteCategory.DateModified = noteCategory.DateModified;
                noteCategory.WaitSendingTime = noteCategory.WaitSendingTime;

                if (noteCategory.FWaitSending == 0)
                {
                    return;
                }

                if (noteCategory.DataStatus == 'N')
                {
                    return;
                }
                switch (noteCategory.DataStatus)
                {
                    case 'I':
                        InsertNoteCategory(noteCategory);
                        break;
                    case 'M':
                        UpdateNoteCategory(noteCategory);
                        break;
                    case 'D':
                        DeleteNoteCategory(noteCategory);
                        break;
                    default:
                        break;
                }
            }
            catch (WebException)
            {
                noteCategory = await NoteCategoryManage.GetNoteCategory(merchantid, SysNoteCategoryID);
                noteCategory.FWaitSending = 2;
                await NoteCategoryManage.UpdateNoteCategory(noteCategory);
            }
        }

        static public async Task SentNoteCategoryAndroid(int merchantid, int SysNoteCategoryID)
        {
            NoteCategory noteCategory = new NoteCategory();
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                noteCategory = await NoteCategoryManage.GetNoteCategoryAndroid(merchantid, SysNoteCategoryID);
                if (noteCategory is null)
                {
                    return;
                }
                noteCategory.DateCreated = noteCategory.DateCreated;
                noteCategory.DateModified = noteCategory.DateModified;
                noteCategory.WaitSendingTime = noteCategory.WaitSendingTime;

                if (noteCategory.FWaitSending == 0)
                {
                    return;
                }

                if (noteCategory.DataStatus == 'N')
                {
                    return;
                }
                switch (noteCategory.DataStatus)
                {
                    case 'I':
                        InsertNoteCategory(noteCategory);
                        break;
                    case 'M':
                        UpdateNoteCategory(noteCategory);
                        break;
                    case 'D':
                        DeleteNoteCategory(noteCategory);
                        break;
                    default:
                        break;
                }
            }
            catch (WebException)
            {
                noteCategory = await NoteCategoryManage.GetNoteCategory(merchantid, SysNoteCategoryID);
                noteCategory.FWaitSending = 2;
                await NoteCategoryManage.UpdateNoteCategory(noteCategory);
            }
        }

        private async static void DeleteNoteCategory(NoteCategory noteCategory)
        {
            try
            {
                var result = await GabanaAPI.DeleteDataNoteCategory((int)noteCategory.SysNoteCategoryID); //return UpdateLastRevisionNo
                if (result.Status)
                {
                    noteCategory.FWaitSending = 0;

                    //ลบข้อมูล Note ที่มี sysNoteCategory เป็นตัวที่ลบ
                    var deleteNote = await noteManage.DeleteAllNoteByNoteCategory((int)noteCategory.MerchantID, (int)noteCategory.SysNoteCategoryID);
                    
                    //Delete Item ที่ Local
                    var deleteItem = await NoteCategoryManage.DeleteNoteCategory((int)noteCategory.MerchantID, (int)noteCategory.SysNoteCategoryID);
                    if (!deleteItem)
                    {
                        noteCategory.FWaitSending = 2;
                    }
                }
                else
                {
                    noteCategory.FWaitSending = 2;
                }
                await NoteCategoryManage.UpdateNoteCategory(noteCategory);               
            }
            catch (WebException)
            {
                noteCategory = await NoteCategoryManage.GetNoteCategory(DataCashingAll.MerchantId, (int)noteCategory.SysNoteCategoryID);
                noteCategory.FWaitSending = 2;
                await NoteCategoryManage.UpdateNoteCategory(noteCategory);
            }
        }

        private async static void UpdateNoteCategory(NoteCategory noteCategory)
        {
            try
            {
                ORM.Master.NoteCategory JAMNoteCategory = new ORM.Master.NoteCategory();
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ORM.MerchantDB.NoteCategory, ORM.Master.NoteCategory>();
                });

                var Imapper = config.CreateMapper();
                JAMNoteCategory = Imapper.Map<ORM.MerchantDB.NoteCategory, ORM.Master.NoteCategory>(noteCategory);

                var result = await GabanaAPI.PutDataNoteCategory(JAMNoteCategory);
                if (result.Status)
                {
                    noteCategory.FWaitSending = 0;
                }
                else
                {
                    noteCategory.FWaitSending = 2;
                }
                await NoteCategoryManage.UpdateNoteCategory(noteCategory);
            }
            catch (WebException)
            {
                noteCategory = await NoteCategoryManage.GetNoteCategory(DataCashingAll.MerchantId, (int)noteCategory.SysNoteCategoryID);
                noteCategory.FWaitSending = 2;
                await NoteCategoryManage.UpdateNoteCategory(noteCategory);
            }
        }

        private async static void InsertNoteCategory(NoteCategory noteCategory)
        {
            try
            {
                ORM.Master.NoteCategory JAMNoteCategory = new ORM.Master.NoteCategory();
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ORM.MerchantDB.NoteCategory, ORM.Master.NoteCategory>();
                });

                var Imapper = config.CreateMapper();
                JAMNoteCategory = Imapper.Map<ORM.MerchantDB.NoteCategory, ORM.Master.NoteCategory>(noteCategory);

                var result = await GabanaAPI.PostDataNoteCategory(JAMNoteCategory);
                if (result.Status)
                {
                    noteCategory.FWaitSending = 0;
                }
                else
                {
                    noteCategory.FWaitSending = 2;
                }
                await NoteCategoryManage.UpdateNoteCategory(noteCategory);
            }
            catch (WebException)
            {
                noteCategory = await NoteCategoryManage.GetNoteCategory((int)noteCategory.MerchantID, (int)noteCategory.SysNoteCategoryID);
                noteCategory.FWaitSending = 2;
                await NoteCategoryManage.UpdateNoteCategory(noteCategory);
                ;
            }
        }

    }
}
