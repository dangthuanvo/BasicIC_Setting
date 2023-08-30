using Common;
using BasicIC_Setting.Common;
using System;

namespace BasicIC_Setting.Models.Main
{
    public class BaseModel
    {
        public Guid id { get; set; }
        public string tenant_id { get; set; }
        public string create_by { get; set; }
        public string modify_by { get; set; }
        public DateTime? create_time { get; set; }
        public DateTime? modify_time { get; set; }

        public void AddInfo()
        {
            DateTime currentDateTime = DateTime.Now;
            id = Guid.NewGuid();
            tenant_id = tenant_id ?? SessionStore.Get(Constants.KEY_SESSION_TENANT_ID);
            create_by = SessionStore.Get(Constants.KEY_SESSION_EMAIL);
            modify_by = null;
            create_time = currentDateTime;
            modify_time = currentDateTime;
        }

        public void UpdateInfo(BaseModel baseData)
        {
            DateTime currentDateTime = DateTime.Now;
            modify_by = SessionStore.Get(Constants.KEY_SESSION_EMAIL);
            modify_time = currentDateTime;
            tenant_id = tenant_id ?? baseData.tenant_id;
            create_by = baseData.create_by;
            create_time = baseData.create_time;
        }
    }
}