using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Dal.Models
{
    public enum AdvertisementStatusEnum
    {
        [Description("Созданый")]
        Created ,
        [Description("Проверка")]
        InModeration ,
        [Description("К оплате")]
        ForPayment ,
        [Description("В ожидании")]        
        Waiting ,
        [Description("В ожидании")]
        Active ,
        [Description("Завершенный")]
        Finished
    }
}
