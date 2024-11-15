using Gabana.ORM.PoolDB;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gabana.ShareSource.Manage
{
    public class PoolManage
    {
        public async Task<District> GetZipcode(int AmphureId,int districtId)
        {
            try
            {
                using (var db = new PoolDB(DataCashingAll.Pathdbpool))
                {
                    var result = await db.Districts.Where(x => x.AmphuresId == AmphureId & x.DistrictsId == districtId)                                       
                                        .FirstOrDefaultAsync();
                    if (result == null)
                    {
                        return new District();
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new District();
            };
        }

        public async Task<List<District>> GetDistricts(int AmphureId)
        {
            var lstDistrict = new List<District>();
            try
            {
                using (var db = new PoolDB(DataCashingAll.Pathdbpool))
                {                    
                    var result = await db.Districts.Where(x => x.AmphuresId == AmphureId).OrderBy(x=>x.DistrictsNameTH).ToListAsync<District>();

                    if (result == null)
                    {
                        return new List<District>();
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<District>();
            };
        }       

        public async Task<List<Amphure>> GetAmphures(int ProvinceID)
        {            
            try
            {
                using (var db = new PoolDB(DataCashingAll.Pathdbpool))
                {                   
                    var result = await db.Amphures.Where(x => x.ProvincesId == ProvinceID).OrderBy(x=>x.AmphuresNameTH).ToListAsync<Amphure>();
                    if (result == null)
                    {
                        return  new List<Amphure>();
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<Amphure>();
            };
        }
               
        public async Task<List<Province>> GetProvinces()
        {           
            try
            {               
                using (var db = new PoolDB(DataCashingAll.Pathdbpool))
                {
                    List<Province>  lstProvince = await db.Provinces.OrderBy(x=>x.ProvincesNameTH).ToListAsync<Province>();                    
                    return lstProvince;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<Province>();
            };
        }
        public async Task<List<Province>> GetProvincesnew()
        {
            try
            {
                using (var db = new PoolDB(DataCashingAll.Pathdbpool))
                {
                    List<Province> lstProvince = await db.Provinces.ToListAsync<Province>();
                    return lstProvince;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<Province>();
            };
        }

        public async Task<List<DataBaseInfo>> GetDatabaseInfo(string KeyInfo)
        {
            try
            {
                using (var db = new PoolDB(DataCashingAll.Pathdbpool))
                {
                    var lstdbinfo = new List<DataBaseInfo>();
                    var result = await db.DataBaseInfo.Where(x => x.KeyDBInfo == KeyInfo).FirstOrDefaultAsync();
                    if (result != null)
                    {
                        lstdbinfo = await db.DataBaseInfo.ToListAsync<DataBaseInfo>();
                    }
                    return lstdbinfo;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

        public async Task<List<Geography>> GetGeography(int GeographyId)
        {
            try
            {
                using (var db = new PoolDB(DataCashingAll.Pathdbpool))
                {
                    var lstgeo = new List<Geography>();
                    var result = await db.Geographies.Where(x => x.GeographyId == GeographyId).FirstOrDefaultAsync();
                    if (result != null)
                    {
                        lstgeo = await db.Geographies.ToListAsync<Geography>();
                    }
                    return lstgeo;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            };
        }

     

    }
}
